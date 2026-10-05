@echo off
setlocal enabledelayedexpansion

REM ============================================================
REM deploy-image.bat - Deploy Tour_Management to AWS EKS (Windows)
REM Application: Tour_Management (.NET Framework 4.7.2 / Windows Containers)
REM Target: AWS EKS with Windows node groups
REM ============================================================

set "NAMESPACE=tour-management"
set "APP_NAME=tour-management"
set "K8S_DIR=DotNetFrameworkProject_CE040_CE087\Tour_Management\kubernetes"

echo ============================================================
echo  Tour_Management - Deploy to AWS EKS
echo ============================================================
echo.

REM Prompt for AWS region
set /p AWS_REGION="Enter AWS Region [us-east-1]: "
if "!AWS_REGION!"=="" set "AWS_REGION=us-east-1"

REM Prompt for EKS cluster name
set /p CLUSTER_NAME="Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

REM Prompt for Docker image URI
set /p IMAGE_URI="Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

REM Prompt for application environment variables
echo.
echo --- Application Environment Variables ---
echo (Press Enter to skip any variable)
echo.

set /p DB_HOST="Enter DB_HOST (SQL Server host): "
set /p DB_PORT="Enter DB_PORT [1433]: "
if "!DB_PORT!"=="" set "DB_PORT=1433"
set /p DB_NAME="Enter DB_NAME (database name): "
set /p DB_USER="Enter DB_USER (database user): "
set /p DB_PASSWORD="Enter DB_PASSWORD (database password): "
set /p S3_BUCKET_NAME="Enter S3_BUCKET_NAME (AWS S3 bucket for file uploads): "

echo.
echo ============================================================
echo  Configuring kubectl for EKS cluster: !CLUSTER_NAME!
echo ============================================================
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl for EKS cluster.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

echo.
echo ============================================================
echo  Updating Kubernetes manifests with deployment values...
echo ============================================================

REM Create temp directory for manifest copies
set "TEMP_DIR=%TEMP%\tour-management-deploy"
if exist "!TEMP_DIR!" rmdir /s /q "!TEMP_DIR!"
mkdir "!TEMP_DIR!"

copy "!K8S_DIR!\namespace.yaml"  "!TEMP_DIR!\namespace.yaml"  >nul
copy "!K8S_DIR!\deployment.yaml" "!TEMP_DIR!\deployment.yaml" >nul
copy "!K8S_DIR!\service.yaml"    "!TEMP_DIR!\service.yaml"    >nul
copy "!K8S_DIR!\ingress.yaml"    "!TEMP_DIR!\ingress.yaml"    >nul

REM Replace placeholders using PowerShell
powershell -Command ^
  "$d = Get-Content '!TEMP_DIR!\deployment.yaml' -Raw;" ^
  "$d = $d -replace '{{IMAGE_URI}}','!IMAGE_URI!';" ^
  "$d = $d -replace '{{DB_HOST}}','!DB_HOST!';" ^
  "$d = $d -replace '{{DB_PORT}}','!DB_PORT!';" ^
  "$d = $d -replace '{{DB_NAME}}','!DB_NAME!';" ^
  "$d = $d -replace '{{DB_USER}}','!DB_USER!';" ^
  "$d = $d -replace '{{DB_PASSWORD}}','!DB_PASSWORD!';" ^
  "$d = $d -replace '{{AWS_REGION}}','!AWS_REGION!';" ^
  "$d = $d -replace '{{S3_BUCKET_NAME}}','!S3_BUCKET_NAME!';" ^
  "Set-Content '!TEMP_DIR!\deployment.yaml' $d"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    exit /b 1
)

echo.
echo ============================================================
echo  Applying Kubernetes manifests...
echo ============================================================

echo 1/4 Applying namespace...
kubectl apply -f "!TEMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo 2/4 Applying deployment...
kubectl apply -f "!TEMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo 3/4 Applying service...
kubectl apply -f "!TEMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo 4/4 Applying ingress...
kubectl apply -f "!TEMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

REM Clean up temp files
rmdir /s /q "!TEMP_DIR!"

echo.
echo ============================================================
echo  Waiting for deployment rollout...
echo ============================================================
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo WARNING: Rollout did not complete within timeout. Check pod status.
)

echo.
echo ============================================================
echo  Verifying deployed resources...
echo ============================================================
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ============================================================
echo  Deployment complete!
echo  Application URL: http://tour-management.example.com
echo  Health Check:    http://tour-management.example.com/health.ashx
echo.
echo  To rollback if needed:
echo    kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo ============================================================

endlocal
