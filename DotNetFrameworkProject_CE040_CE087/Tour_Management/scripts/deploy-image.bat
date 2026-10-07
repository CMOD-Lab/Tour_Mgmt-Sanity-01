@echo off
setlocal enabledelayedexpansion

REM =============================================================
REM deploy-image.bat — Deploy Tour_Management to AWS EKS
REM =============================================================

set "APP_NAME=tour-management"
set "NAMESPACE=tour-management"

echo ==============================================
echo  Tour_Management - Deploy to AWS EKS
echo ==============================================
echo.

REM ---- Collect deployment inputs ----
set /p "AWS_REGION_INPUT=Enter AWS region [us-east-1]: "
if "!AWS_REGION_INPUT!"=="" (
    set "AWS_REGION=us-east-1"
) else (
    set "AWS_REGION=!AWS_REGION_INPUT!"
)

set /p "CLUSTER_NAME=Enter EKS cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS cluster name is required.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

echo.
echo --- Optional: Application Environment Variables ---
echo Press Enter to skip any variable.
echo.

set /p "DB_HOST=Enter DB_HOST (SQL Server hostname or IP): "
set /p "DB_PORT_INPUT=Enter DB_PORT [1433]: "
if "!DB_PORT_INPUT!"=="" (
    set "DB_PORT=1433"
) else (
    set "DB_PORT=!DB_PORT_INPUT!"
)
set /p "DB_NAME=Enter DB_NAME (database name): "
set /p "APP_AWS_REGION_INPUT=Enter AWS_REGION for app [!AWS_REGION!]: "
if "!APP_AWS_REGION_INPUT!"=="" (
    set "APP_AWS_REGION=!AWS_REGION!"
) else (
    set "APP_AWS_REGION=!APP_AWS_REGION_INPUT!"
)
set /p "S3_BUCKET_NAME=Enter S3_BUCKET_NAME: "

echo.
echo --- Configuring kubectl for EKS cluster '!CLUSTER_NAME!' ---
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
echo --- Updating Kubernetes manifests with deployment values ---

REM Create temp directory for modified manifests
set "DEPLOY_TMP_DIR=%TEMP%\tour-management-deploy-%RANDOM%"
mkdir "!DEPLOY_TMP_DIR!"

REM Copy manifests to temp directory
copy "kubernetes\*.yaml" "!DEPLOY_TMP_DIR!\" >nul

REM Replace placeholders using PowerShell
powershell -NoProfile -Command ^
    "$content = Get-Content '!DEPLOY_TMP_DIR!\deployment.yaml' -Raw;" ^
    "$content = $content -replace '\{\{IMAGE_URI\}\}', '!IMAGE_URI!';" ^
    "$content = $content -replace '\{\{DB_HOST\}\}', '!DB_HOST!';" ^
    "$content = $content -replace '\{\{DB_PORT\}\}', '!DB_PORT!';" ^
    "$content = $content -replace '\{\{DB_NAME\}\}', '!DB_NAME!';" ^
    "$content = $content -replace '\{\{AWS_REGION\}\}', '!APP_AWS_REGION!';" ^
    "$content = $content -replace '\{\{S3_BUCKET_NAME\}\}', '!S3_BUCKET_NAME!';" ^
    "Set-Content '!DEPLOY_TMP_DIR!\deployment.yaml' -Value $content"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    exit /b 1
)

echo.
echo --- Applying Kubernetes manifests ---

echo 1/4 Applying namespace...
kubectl apply -f "!DEPLOY_TMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply namespace.
    exit /b 1
)

echo 2/4 Applying deployment...
kubectl apply -f "!DEPLOY_TMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply deployment.
    exit /b 1
)

echo 3/4 Applying service...
kubectl apply -f "!DEPLOY_TMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply service.
    exit /b 1
)

echo 4/4 Applying ingress...
kubectl apply -f "!DEPLOY_TMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply ingress.
    exit /b 1
)

echo.
echo --- Waiting for deployment rollout ---
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed. Rolling back...
    kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    exit /b 1
)

echo.
echo --- Verifying deployed resources ---
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo --- Application Access URL ---
for /f "delims=" %%h in ('kubectl get ingress tour-management-ingress -n !NAMESPACE! -o jsonpath^="{.status.loadBalancer.ingress[0].hostname}" 2^>nul') do set "INGRESS_HOST=%%h"
if "!INGRESS_HOST!"=="" (
    echo Ingress hostname is still provisioning. Run the following to check:
    echo   kubectl get ingress tour-management-ingress -n !NAMESPACE!
) else (
    echo Application URL: http://!INGRESS_HOST!
)

REM Cleanup temp directory
rmdir /s /q "!DEPLOY_TMP_DIR!" >nul 2>&1

echo.
echo ==============================================
echo  SUCCESS: Tour_Management deployed to EKS!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

endlocal
exit /b 0
