@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: deploy-image.bat - Deploy Tour_Management to AWS EKS
:: ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Containers
:: =============================================================

set "APP_NAME=tour-management"
set "NAMESPACE=tour-management"
set "K8S_DIR=kubernetes"

echo ==============================================
echo  Tour_Management - AWS EKS Deployment Script
echo ==============================================
echo.

:: ---- Prompt for AWS / EKS configuration ----
set /p "AWS_REGION=Enter AWS Region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo [ERROR] AWS Region is required.
    exit /b 1
)

set /p "CLUSTER_NAME=Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo [ERROR] EKS Cluster Name is required.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker Image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): "
if "!IMAGE_URI!"=="" (
    echo [ERROR] Docker Image URI is required.
    exit /b 1
)

echo.
echo [INFO] Configuring application environment variables...
echo        Press Enter to skip any variable.
echo.

set /p "DB_CONNECTION_STRING=Enter DB_CONNECTION_STRING (or Enter to skip): "
set /p "DB_HOST=Enter DB_HOST (SQL Server hostname/IP, or Enter to skip): "
set /p "DB_PORT=Enter DB_PORT [default: 1433]: "
if "!DB_PORT!"=="" set "DB_PORT=1433"
set /p "DB_NAME=Enter DB_NAME (database name, or Enter to skip): "
set /p "DB_USER=Enter DB_USER (database username, or Enter to skip): "
set /p "DB_PASSWORD=Enter DB_PASSWORD (database password, or Enter to skip): "
set /p "APP_AWS_REGION=Enter AWS_REGION for S3 [default: !AWS_REGION!]: "
if "!APP_AWS_REGION!"=="" set "APP_AWS_REGION=!AWS_REGION!"
set /p "AWS_S3_BUCKET=Enter AWS_S3_BUCKET (S3 bucket name, or Enter to skip): "

echo.
echo [INFO] Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Failed to configure kubectl for EKS cluster.
    exit /b 1
)

echo [INFO] Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Cannot connect to EKS cluster.
    exit /b 1
)

echo.
echo [INFO] Updating Kubernetes manifests with deployment values...

:: Create backup of deployment manifest
copy "!K8S_DIR!\deployment.yaml" "!K8S_DIR!\deployment.yaml.bak" >nul

:: Replace placeholders using PowerShell
powershell -Command ^
    "$content = Get-Content '!K8S_DIR!\deployment.yaml' -Raw;" ^
    "$content = $content -replace '\{\{IMAGE_URI\}\}', '!IMAGE_URI!';" ^
    "$content = $content -replace '\{\{DB_CONNECTION_STRING\}\}', '!DB_CONNECTION_STRING!';" ^
    "$content = $content -replace '\{\{DB_HOST\}\}', '!DB_HOST!';" ^
    "$content = $content -replace '\{\{DB_PORT\}\}', '!DB_PORT!';" ^
    "$content = $content -replace '\{\{DB_NAME\}\}', '!DB_NAME!';" ^
    "$content = $content -replace '\{\{DB_USER\}\}', '!DB_USER!';" ^
    "$content = $content -replace '\{\{DB_PASSWORD\}\}', '!DB_PASSWORD!';" ^
    "$content = $content -replace '\{\{AWS_REGION\}\}', '!APP_AWS_REGION!';" ^
    "$content = $content -replace '\{\{AWS_S3_BUCKET\}\}', '!AWS_S3_BUCKET!';" ^
    "Set-Content '!K8S_DIR!\deployment.yaml' -Value $content"

if !ERRORLEVEL! neq 0 (
    echo [ERROR] Failed to update deployment manifest.
    copy "!K8S_DIR!\deployment.yaml.bak" "!K8S_DIR!\deployment.yaml" >nul
    exit /b 1
)
echo [INFO] Manifests updated successfully.

echo.
echo [INFO] Applying Kubernetes manifests...

echo [INFO] 1/4 Applying namespace...
kubectl apply -f "!K8S_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo [ERROR] Failed to apply namespace. & goto RESTORE_AND_FAIL )

echo [INFO] 2/4 Applying deployment...
kubectl apply -f "!K8S_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo [ERROR] Failed to apply deployment. & goto RESTORE_AND_FAIL )

echo [INFO] 3/4 Applying service...
kubectl apply -f "!K8S_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo [ERROR] Failed to apply service. & goto RESTORE_AND_FAIL )

echo [INFO] 4/4 Applying ingress...
kubectl apply -f "!K8S_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo [ERROR] Failed to apply ingress. & goto RESTORE_AND_FAIL )

echo.
echo [INFO] Waiting for deployment rollout to complete...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo [ERROR] Deployment rollout failed. Rolling back...
    kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    echo [INFO] Rollback initiated. Check pod logs:
    echo        kubectl logs -l app=!APP_NAME! -n !NAMESPACE!
    goto RESTORE_AND_FAIL
)

:: Restore original manifest with placeholders
copy "!K8S_DIR!\deployment.yaml.bak" "!K8S_DIR!\deployment.yaml" >nul
del "!K8S_DIR!\deployment.yaml.bak" >nul 2>&1

echo.
echo [INFO] Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo [SUCCESS] Tour_Management deployed to EKS!
echo ==============================================
echo.
echo   Namespace : !NAMESPACE!
echo   Image     : !IMAGE_URI!
echo   Cluster   : !CLUSTER_NAME! (!AWS_REGION!)
echo.
echo Useful commands:
echo   kubectl get pods -n !NAMESPACE!
echo   kubectl logs -l app=!APP_NAME! -n !NAMESPACE!
echo   kubectl describe deployment !APP_NAME! -n !NAMESPACE!
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo.
endlocal
exit /b 0

:RESTORE_AND_FAIL
copy "!K8S_DIR!\deployment.yaml.bak" "!K8S_DIR!\deployment.yaml" >nul
del "!K8S_DIR!\deployment.yaml.bak" >nul 2>&1
endlocal
exit /b 1
