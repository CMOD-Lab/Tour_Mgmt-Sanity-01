@echo off
setlocal enabledelayedexpansion

REM =============================================================
REM deploy-image.bat - Deploy Tour_Management to AWS EKS
REM Target Platform: AWS EKS (Windows node groups)
REM =============================================================

set APP_NAME=tour-management
set NAMESPACE=tour-management
set MANIFESTS_DIR=%~dp0..\kubernetes

echo ==============================================
echo  Tour_Management - AWS EKS Deployment Script
echo ==============================================
echo.

REM Prompt for AWS region
set /p AWS_REGION_INPUT="Enter AWS Region [us-east-1]: "
if "!AWS_REGION_INPUT!"=="" set AWS_REGION_INPUT=us-east-1
set AWS_REGION=!AWS_REGION_INPUT!

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

echo.
echo --- Application Environment Variables ---
echo Press Enter to skip any variable.
echo.

set /p DB_CONNECTION_STRING="Enter DB_CONNECTION_STRING (SQL Server connection string): "
set /p DB_HOST="Enter DB_HOST (SQL Server host): "
set /p DB_PORT_INPUT="Enter DB_PORT [1433]: "
if "!DB_PORT_INPUT!"=="" set DB_PORT_INPUT=1433
set DB_PORT=!DB_PORT_INPUT!
set /p DB_NAME="Enter DB_NAME (database name): "
set /p DB_USER="Enter DB_USER (database user): "
set /p DB_PASSWORD="Enter DB_PASSWORD (database password): "
set /p S3_AWS_REGION_INPUT="Enter AWS_REGION for S3 [!AWS_REGION!]: "
if "!S3_AWS_REGION_INPUT!"=="" set S3_AWS_REGION_INPUT=!AWS_REGION!
set S3_AWS_REGION=!S3_AWS_REGION_INPUT!
set /p S3_BUCKET_NAME="Enter S3_BUCKET_NAME (for tour image uploads): "

echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl for EKS cluster.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to Kubernetes cluster.
    exit /b 1
)

echo.
echo Updating Kubernetes manifests with deployment values...

REM Copy deployment manifest for editing
copy "!MANIFESTS_DIR!\deployment.yaml" "!MANIFESTS_DIR!\deployment.yaml.bak" >nul

REM Replace placeholders using PowerShell
powershell -Command ^
    "$content = Get-Content '!MANIFESTS_DIR!\deployment.yaml' -Raw;" ^
    "$content = $content -replace '{{IMAGE_URI}}', '!IMAGE_URI!';" ^
    "$content = $content -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!';" ^
    "$content = $content -replace '{{DB_HOST}}', '!DB_HOST!';" ^
    "$content = $content -replace '{{DB_PORT}}', '!DB_PORT!';" ^
    "$content = $content -replace '{{DB_NAME}}', '!DB_NAME!';" ^
    "$content = $content -replace '{{DB_USER}}', '!DB_USER!';" ^
    "$content = $content -replace '{{DB_PASSWORD}}', '!DB_PASSWORD!';" ^
    "$content = $content -replace '{{S3_BUCKET_NAME}}', '!S3_BUCKET_NAME!';" ^
    "$content = $content -replace '{{AWS_REGION}}', '!S3_AWS_REGION!';" ^
    "Set-Content '!MANIFESTS_DIR!\deployment.yaml' $content"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    copy "!MANIFESTS_DIR!\deployment.yaml.bak" "!MANIFESTS_DIR!\deployment.yaml" >nul
    exit /b 1
)

echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "!MANIFESTS_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply namespace.
    goto RESTORE_AND_EXIT
)

echo   [2/4] Applying deployment...
kubectl apply -f "!MANIFESTS_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply deployment.
    goto RESTORE_AND_EXIT
)

echo   [3/4] Applying service...
kubectl apply -f "!MANIFESTS_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply service.
    goto RESTORE_AND_EXIT
)

echo   [4/4] Applying ingress...
kubectl apply -f "!MANIFESTS_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply ingress.
    goto RESTORE_AND_EXIT
)

echo.
echo Waiting for deployment rollout...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed. Rolling back...
    kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    goto RESTORE_AND_EXIT
)

REM Restore original manifest
copy "!MANIFESTS_DIR!\deployment.yaml.bak" "!MANIFESTS_DIR!\deployment.yaml" >nul
del "!MANIFESTS_DIR!\deployment.yaml.bak" >nul 2>&1

echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo  DEPLOYMENT SUCCESSFUL!
echo  Application: !APP_NAME!
echo  Namespace:   !NAMESPACE!
echo  Image:       !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

endlocal
exit /b 0

:RESTORE_AND_EXIT
copy "!MANIFESTS_DIR!\deployment.yaml.bak" "!MANIFESTS_DIR!\deployment.yaml" >nul
del "!MANIFESTS_DIR!\deployment.yaml.bak" >nul 2>&1
endlocal
exit /b 1
