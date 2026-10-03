@echo off
setlocal enabledelayedexpansion

REM =============================================================
REM build-push.bat - Build and Push Docker Image for Tour_Management
REM Target: AWS EKS (Windows containers)
REM =============================================================

set PROJECT_NAME=tour-management
set DOCKERFILE_PATH=DotNetFrameworkProject_CE040_CE087\Tour_Management\Dockerfile
set BUILD_CONTEXT=.

echo ==============================================
echo  Tour_Management - Docker Build ^& Push Script
echo ==============================================
echo.

REM Sanitize image name (PowerShell-based)
for /f "delims=" %%i in ('powershell -Command "$n = 'tour-management' -replace '[^a-z0-9]','-'; $n = $n.ToLower().Trim('-'); while($n -match '--') { $n = $n -replace '--','-' }; $n"') do set IMAGE_NAME=%%i

REM Prompt for image tag
set /p IMAGE_TAG_INPUT="Enter image tag [latest]: "
if "!IMAGE_TAG_INPUT!"=="" set IMAGE_TAG_INPUT=latest
for /f "delims=" %%i in ('powershell -Command "$t = '!IMAGE_TAG_INPUT!' -replace '[^a-z0-9._-]','-'; $t = $t.ToLower().Trim('-'); if($t -eq '') { $t = 'latest' }; $t"') do set IMAGE_TAG=%%i
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

echo.
echo Select container registry:
echo   1. AWS ECR
echo   2. Docker Hub
set /p REGISTRY_CHOICE="Enter choice [1 or 2]: "

if "!REGISTRY_CHOICE!"=="1" goto ECR_SETUP
if "!REGISTRY_CHOICE!"=="2" goto DOCKERHUB_SETUP
echo ERROR: Invalid choice. Please enter 1 or 2.
exit /b 1

:ECR_SETUP
set /p AWS_REGION_INPUT="Enter AWS Region [us-east-1]: "
if "!AWS_REGION_INPUT!"=="" set AWS_REGION_INPUT=us-east-1
set AWS_REGION=!AWS_REGION_INPUT!

set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
if "!AWS_ACCOUNT_ID!"=="" (
    echo ERROR: AWS Account ID is required.
    exit /b 1
)

set ECR_REPO=!IMAGE_NAME!
set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!

echo.
echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed.
    exit /b 1
)

echo Checking/creating ECR repository: !ECR_REPO!...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository...
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository.
        exit /b 1
    )
)
goto BUILD

:DOCKERHUB_SETUP
set /p DOCKER_USERNAME="Enter Docker Hub username: "
if "!DOCKER_USERNAME!"=="" (
    echo ERROR: Docker Hub username is required.
    exit /b 1
)

set /p DOCKER_PASSWORD="Enter Docker Hub password/token: "
if "!DOCKER_PASSWORD!"=="" (
    echo ERROR: Docker Hub password is required.
    exit /b 1
)

set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!

echo.
echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
goto BUILD

:BUILD
echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo Dockerfile: !DOCKERFILE_PATH!
echo Build context: !BUILD_CONTEXT!
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" "!BUILD_CONTEXT!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Pushing image: !FULL_IMAGE_NAME!
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo  SUCCESS: Image pushed successfully!
echo  Image: !FULL_IMAGE_NAME!
echo ==============================================

endlocal
