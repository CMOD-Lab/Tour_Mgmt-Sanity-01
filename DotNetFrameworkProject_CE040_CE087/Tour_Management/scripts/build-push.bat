@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: build-push.bat - Build and Push Docker Image for Tour_Management
:: ASP.NET Web Forms (.NET Framework 4.7.2) on Windows Containers
:: Target: AWS EKS (Windows node groups)
:: =============================================================

set "PROJECT_NAME=tour-management"
set "DOCKERFILE_PATH=Tour_Management\Dockerfile"

echo ==============================================
echo  Tour_Management - Docker Build ^& Push Script
echo ==============================================
echo.

:: ---- Tag Sanitization via PowerShell ----
for /f "delims=" %%i in ('powershell -Command "$n = 'tour-management'; $n = $n.ToLower() -replace '[^a-z0-9]','-'; $n = $n.Trim('-'); Write-Output $n"') do set "IMAGE_NAME=%%i"

echo Select container registry:
echo   1. AWS ECR
echo   2. Docker Hub
echo.
set /p "REGISTRY_CHOICE=Enter choice [1 or 2]: "

if "!REGISTRY_CHOICE!"=="1" goto ECR_SETUP
if "!REGISTRY_CHOICE!"=="2" goto DOCKERHUB_SETUP
echo [ERROR] Invalid choice. Please enter 1 or 2.
exit /b 1

:ECR_SETUP
echo.
set /p "AWS_REGION=Enter AWS Region (e.g. us-east-1): "
set /p "AWS_ACCOUNT_ID=Enter AWS Account ID: "
set /p "ECR_REPO=Enter ECR Repository name [default: !IMAGE_NAME!]: "
if "!ECR_REPO!"=="" set "ECR_REPO=!IMAGE_NAME!"
for /f "delims=" %%i in ('powershell -Command "$r = '!ECR_REPO!'.ToLower() -replace '[^a-z0-9/_.-]','-'; $r = $r.Trim('-'); Write-Output $r"') do set "ECR_REPO=%%i"
set /p "IMAGE_TAG=Enter image tag [default: latest]: "
if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
for /f "delims=" %%i in ('powershell -Command "$t = '!IMAGE_TAG!'.ToLower() -replace '[^a-z0-9._-]','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set "IMAGE_TAG=%%i"

set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo.
echo [INFO] Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo [ERROR] ECR login failed.
    exit /b 1
)

echo [INFO] Checking if ECR repository exists...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo [INFO] Creating ECR repository: !ECR_REPO!
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo [ERROR] Failed to create ECR repository.
        exit /b 1
    )
)
echo [INFO] ECR repository ready: !ECR_REPO!
goto BUILD

:DOCKERHUB_SETUP
echo.
set /p "DOCKER_USERNAME=Enter Docker Hub username: "
set /p "DOCKER_PASSWORD=Enter Docker Hub password/token: "
set /p "DOCKER_REPO=Enter Docker Hub repository name [default: !IMAGE_NAME!]: "
if "!DOCKER_REPO!"=="" set "DOCKER_REPO=!IMAGE_NAME!"
for /f "delims=" %%i in ('powershell -Command "$r = '!DOCKER_REPO!'.ToLower() -replace '[^a-z0-9/_.-]','-'; $r = $r.Trim('-'); Write-Output $r"') do set "DOCKER_REPO=%%i"
set /p "IMAGE_TAG=Enter image tag [default: latest]: "
if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
for /f "delims=" %%i in ('powershell -Command "$t = '!IMAGE_TAG!'.ToLower() -replace '[^a-z0-9._-]','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set "IMAGE_TAG=%%i"

set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!DOCKER_REPO!:!IMAGE_TAG!"

echo.
echo [INFO] Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Docker Hub login failed.
    exit /b 1
)
goto BUILD

:BUILD
echo.
echo [INFO] Building Docker image: !FULL_IMAGE_NAME!
echo [INFO] Dockerfile: !DOCKERFILE_PATH!
echo [INFO] Build context: . (repository root)
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Docker build failed.
    exit /b 1
)
echo [INFO] Docker build succeeded.

echo.
echo [INFO] Pushing image: !FULL_IMAGE_NAME!
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo [SUCCESS] Image pushed: !FULL_IMAGE_NAME!
echo ==============================================
echo.
echo Use this image URI in deploy-image.bat:
echo   !FULL_IMAGE_NAME!

endlocal
