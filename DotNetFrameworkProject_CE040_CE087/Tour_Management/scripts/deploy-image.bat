@echo off
setlocal enabledelayedexpansion

REM =============================================================================
REM deploy-image.bat — Deploy Tour_Management to AWS EKS (Windows)
REM Prerequisites: aws-cli, kubectl
REM Usage: scripts\deploy-image.bat  (run from repository root)
REM =============================================================================

set "NAMESPACE=tour-management"
set "APP_NAME=tour-management"
set "K8S_DIR=DotNetFrameworkProject_CE040_CE087\Tour_Management\kubernetes"

echo ==============================================
echo   Tour_Management - Deploy to AWS EKS
echo ==============================================
echo.

REM ── Collect deployment inputs ─────────────────────────────────────────────
set /p "AWS_REGION=Enter AWS Region [us-east-1]: "
if "!AWS_REGION!"=="" set "AWS_REGION=us-east-1"

set /p "CLUSTER_NAME=Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker Image URI: "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker Image URI is required.
    exit /b 1
)

echo.
echo --- Application Environment Variables ---
echo Press Enter to skip any optional variable.
echo.

set /p "CONNECTION_STRING=Enter SQL Server CONNECTION_STRING (or press Enter to skip): "
set /p "S3_BUCKET_NAME=Enter S3_BUCKET_NAME (or press Enter to skip): "
set /p "APP_AWS_REGION=Enter AWS_REGION for S3 [!AWS_REGION!]: "
if "!APP_AWS_REGION!"=="" set "APP_AWS_REGION=!AWS_REGION!"
set /p "APP_AWS_ACCESS_KEY_ID=Enter AWS_ACCESS_KEY_ID (or press Enter to skip): "
set /p "APP_AWS_SECRET_ACCESS_KEY=Enter AWS_SECRET_ACCESS_KEY (or press Enter to skip): "

REM ── Configure kubectl for EKS ─────────────────────────────────────────────
echo.
echo Configuring kubectl for EKS cluster '!CLUSTER_NAME!' in '!AWS_REGION!'...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl.
    exit /b 1
)
echo kubectl configured.

echo.
echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

REM ── Update Kubernetes manifests ───────────────────────────────────────────
echo.
echo Updating Kubernetes manifests...

copy "!K8S_DIR!\deployment.yaml" "!K8S_DIR!\deployment.yaml.bak" >nul

REM Use PowerShell to replace placeholders (handles slashes in image URIs)
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{IMAGE_URI}}','!IMAGE_URI!' | Set-Content '!K8S_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{CONNECTION_STRING}}','!CONNECTION_STRING!' | Set-Content '!K8S_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{S3_BUCKET_NAME}}','!S3_BUCKET_NAME!' | Set-Content '!K8S_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{AWS_REGION}}','!APP_AWS_REGION!' | Set-Content '!K8S_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{AWS_ACCESS_KEY_ID}}','!APP_AWS_ACCESS_KEY_ID!' | Set-Content '!K8S_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{AWS_SECRET_ACCESS_KEY}}','!APP_AWS_SECRET_ACCESS_KEY!' | Set-Content '!K8S_DIR!\deployment.yaml'"

echo Manifests updated.

REM ── Apply Kubernetes manifests ────────────────────────────────────────────
echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "!K8S_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 (echo ERROR: Failed to apply namespace. & exit /b 1)

echo   [2/4] Applying deployment...
kubectl apply -f "!K8S_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 (echo ERROR: Failed to apply deployment. & exit /b 1)

echo   [3/4] Applying service...
kubectl apply -f "!K8S_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 (echo ERROR: Failed to apply service. & exit /b 1)

echo   [4/4] Applying ingress...
kubectl apply -f "!K8S_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 (echo ERROR: Failed to apply ingress. & exit /b 1)

REM ── Restore original deployment.yaml ─────────────────────────────────────
move /y "!K8S_DIR!\deployment.yaml.bak" "!K8S_DIR!\deployment.yaml" >nul

REM ── Wait for rollout ──────────────────────────────────────────────────────
echo.
echo Waiting for deployment rollout...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed.
    echo Rollback: kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    exit /b 1
)

REM ── Verify resources ──────────────────────────────────────────────────────
echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo   Deployment successful!
echo   Run the following to get the app URL:
echo   kubectl get ingress -n !NAMESPACE!
echo   Health Check: http://<INGRESS_HOST>/health.ashx
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

endlocal
exit /b 0
