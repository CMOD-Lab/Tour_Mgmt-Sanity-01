@echo off
setlocal enabledelayedexpansion

REM =============================================================================
REM build-push.bat — Build and push the Tour_Management Docker image (Windows)
REM Supports: AWS ECR and Docker Hub
REM Usage: scripts\build-push.bat  (run from repository root)
REM =============================================================================

set "PROJECT_NAME=tour-management"
set "DOCKERFILE_PATH=DotNetFrameworkProject_CE040_CE087\Tour_Management\Dockerfile"

echo ==============================================
echo   Tour_Management - Docker Build ^& Push
echo ==============================================
echo.

REM ── Prompt for image tag ──────────────────────────────────────────────────
set /p "IMAGE_TAG_INPUT=Enter image tag [latest]: "
if "!IMAGE_TAG_INPUT!"=="" set "IMAGE_TAG_INPUT=latest"

REM Sanitize tag using PowerShell
for /f "delims=" %%i in ('powershell -Command "('!IMAGE_TAG_INPUT!'.ToLower() -replace '[^a-z0-9._-]','-').Trim('-')"') do set "IMAGE_TAG=%%i"
if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
echo Using tag: !IMAGE_TAG!
echo.

REM ── Registry selection ────────────────────────────────────────────────────
echo Select container registry:
echo   1) AWS ECR
echo   2) Docker Hub
set /p "REGISTRY_CHOICE=Enter choice [1]: "
if "!REGISTRY_CHOICE!"=="" set "REGISTRY_CHOICE=1"

if "!REGISTRY_CHOICE!"=="1" goto :ecr_setup
if "!REGISTRY_CHOICE!"=="2" goto :dockerhub_setup
echo ERROR: Invalid choice. Please enter 1 or 2.
exit /b 1

REM =============================================================================
:ecr_setup
REM =============================================================================
echo.
echo --- AWS ECR Configuration ---
set /p "AWS_REGION=Enter AWS Region [us-east-1]: "
if "!AWS_REGION!"=="" set "AWS_REGION=us-east-1"

set /p "AWS_ACCOUNT_ID=Enter AWS Account ID: "
if "!AWS_ACCOUNT_ID!"=="" (
    echo ERROR: AWS Account ID is required.
    exit /b 1
)

set /p "ECR_REPO_INPUT=Enter ECR repository name [!PROJECT_NAME!]: "
if "!ECR_REPO_INPUT!"=="" (
    set "ECR_REPO=!PROJECT_NAME!"
) else (
    set "ECR_REPO=!ECR_REPO_INPUT!"
)

set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo.
echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed.
    exit /b 1
)
echo ECR login successful.

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
echo ECR repository ready.
goto :build

REM =============================================================================
:dockerhub_setup
REM =============================================================================
echo.
echo --- Docker Hub Configuration ---
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

set /p "DH_REPO_INPUT=Enter Docker Hub repository name [!PROJECT_NAME!]: "
if "!DH_REPO_INPUT!"=="" (
    set "DH_REPO=!PROJECT_NAME!"
) else (
    set "DH_REPO=!DH_REPO_INPUT!"
)

set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!DH_REPO!:!IMAGE_TAG!"

echo.
echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
echo Docker Hub login successful.
goto :build

REM =============================================================================
:build
REM =============================================================================
echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo Dockerfile: !DOCKERFILE_PATH!
echo Build context: . (repository root)
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)
echo Build successful.

REM =============================================================================
:push
REM =============================================================================
echo.
echo Pushing image: !FULL_IMAGE_NAME!
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo   Image pushed successfully!
echo   !FULL_IMAGE_NAME!
echo ==============================================

endlocal
exit /b 0
