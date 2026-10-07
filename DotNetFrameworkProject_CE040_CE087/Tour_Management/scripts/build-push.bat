@echo off
setlocal enabledelayedexpansion

REM =============================================================
REM build-push.bat — Build and push Tour_Management Docker image
REM Target: AWS EKS (Windows node groups)
REM =============================================================

set "PROJECT_NAME=tour-management"
set "DOCKERFILE_PATH=DotNetFrameworkProject_CE040_CE087\Tour_Management\Dockerfile"

echo ==============================================
echo  Tour_Management - Docker Build ^& Push
echo ==============================================
echo.

REM Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -NoProfile -Command "$n = 'tour-management'; $n = $n.ToLower() -replace '[^a-z0-9]','-'; $n = $n.Trim('-'); Write-Output $n"') do set "IMAGE_NAME=%%i"

REM Prompt for image tag
set /p "IMAGE_TAG_INPUT=Enter image tag [latest]: "
if "!IMAGE_TAG_INPUT!"=="" (
    set "IMAGE_TAG=latest"
) else (
    for /f "delims=" %%t in ('powershell -NoProfile -Command "$t = '!IMAGE_TAG_INPUT!'; $t = $t.ToLower() -replace '[^a-z0-9._-]','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set "IMAGE_TAG=%%t"
)

echo.
echo Select container registry:
echo   1. AWS ECR
echo   2. Docker Hub
set /p "REGISTRY_CHOICE=Enter choice [1 or 2]: "
echo.

if "!REGISTRY_CHOICE!"=="1" goto :ecr_setup
if "!REGISTRY_CHOICE!"=="2" goto :dockerhub_setup
echo ERROR: Invalid registry choice. Please enter 1 or 2.
exit /b 1

:ecr_setup
set /p "AWS_REGION_INPUT=Enter AWS region [us-east-1]: "
if "!AWS_REGION_INPUT!"=="" (
    set "AWS_REGION=us-east-1"
) else (
    set "AWS_REGION=!AWS_REGION_INPUT!"
)

set /p "AWS_ACCOUNT_ID=Enter AWS account ID: "
if "!AWS_ACCOUNT_ID!"=="" (
    echo ERROR: AWS account ID is required.
    exit /b 1
)

set "ECR_REPO=!IMAGE_NAME!"
set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed.
    exit /b 1
)

echo Checking ECR repository '!ECR_REPO!'...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository '!ECR_REPO!'...
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository.
        exit /b 1
    )
)
goto :build

:dockerhub_setup
set /p "DOCKER_USERNAME=Enter Docker Hub username: "
if "!DOCKER_USERNAME!"=="" (
    echo ERROR: Docker Hub username is required.
    exit /b 1
)

set /p "DOCKER_PASSWORD=Enter Docker Hub password/token: "
if "!DOCKER_PASSWORD!"=="" (
    echo ERROR: Docker Hub password is required.
    exit /b 1
)

set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
goto :build

:build
echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo Dockerfile: !DOCKERFILE_PATH!
echo Build context: . (repository root)
echo.

REM NOTE: Build context is the repository root; Dockerfile path is relative to it.
REM Windows containers require --platform windows/amd64
docker build --platform windows/amd64 -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" .
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
exit /b 0
