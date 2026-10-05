#!/bin/bash
# =============================================================
# deploy-image.sh - Deploy Tour_Management to AWS EKS
# ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Containers
# =============================================================
set -e
set -o pipefail

APP_NAME="tour-management"
NAMESPACE="tour-management"
K8S_DIR="kubernetes"

echo "=============================================="
echo " Tour_Management - AWS EKS Deployment Script"
echo "=============================================="
echo ""

# ---- Prompt for AWS / EKS configuration ----
read -p "Enter AWS Region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
    echo "[ERROR] AWS Region is required."
    exit 1
fi

read -p "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
    echo "[ERROR] EKS Cluster Name is required."
    exit 1
fi

read -p "Enter full Docker Image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
    echo "[ERROR] Docker Image URI is required."
    exit 1
fi

echo ""
echo "[INFO] Configuring application environment variables..."
echo "       Press Enter to skip any variable (placeholder will remain)."
echo ""

read -p "Enter DB_CONNECTION_STRING (full SQL Server connection string, or Enter to skip): " DB_CONNECTION_STRING
read -p "Enter DB_HOST (SQL Server hostname/IP, or Enter to skip): " DB_HOST
read -p "Enter DB_PORT [default: 1433]: " DB_PORT
DB_PORT=${DB_PORT:-1433}
read -p "Enter DB_NAME (database name, or Enter to skip): " DB_NAME
read -p "Enter DB_USER (database username, or Enter to skip): " DB_USER
read -s -p "Enter DB_PASSWORD (database password, or Enter to skip): " DB_PASSWORD
echo ""
read -p "Enter AWS_REGION for S3 [default: $AWS_REGION]: " APP_AWS_REGION
APP_AWS_REGION=${APP_AWS_REGION:-$AWS_REGION}
read -p "Enter AWS_S3_BUCKET (S3 bucket name, or Enter to skip): " AWS_S3_BUCKET

echo ""
echo "[INFO] Configuring kubectl for EKS cluster: $CLUSTER_NAME in $AWS_REGION..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
if [ $? -ne 0 ]; then
    echo "[ERROR] Failed to configure kubectl for EKS cluster."
    exit 1
fi

echo "[INFO] Verifying cluster connectivity..."
kubectl cluster-info || { echo "[ERROR] Cannot connect to EKS cluster."; exit 1; }

echo ""
echo "[INFO] Updating Kubernetes manifests with deployment values..."

# Create working copies of manifests
cp "$K8S_DIR/deployment.yaml" "$K8S_DIR/deployment.yaml.bak"

# Replace image URI placeholder
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' "$K8S_DIR/deployment.yaml"

# Replace environment variable placeholders
sed -i 's|{{DB_CONNECTION_STRING}}|'"$DB_CONNECTION_STRING"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{DB_HOST}}|'"$DB_HOST"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{DB_PORT}}|'"$DB_PORT"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{DB_NAME}}|'"$DB_NAME"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{DB_USER}}|'"$DB_USER"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{DB_PASSWORD}}|'"$DB_PASSWORD"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{AWS_REGION}}|'"$APP_AWS_REGION"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{AWS_S3_BUCKET}}|'"$AWS_S3_BUCKET"'|g' "$K8S_DIR/deployment.yaml"

echo "[INFO] Manifests updated successfully."

echo ""
echo "[INFO] Applying Kubernetes manifests..."

echo "[INFO] 1/4 Applying namespace..."
kubectl apply -f "$K8S_DIR/namespace.yaml"

echo "[INFO] 2/4 Applying deployment..."
kubectl apply -f "$K8S_DIR/deployment.yaml"

echo "[INFO] 3/4 Applying service..."
kubectl apply -f "$K8S_DIR/service.yaml"

echo "[INFO] 4/4 Applying ingress..."
kubectl apply -f "$K8S_DIR/ingress.yaml"

echo ""
echo "[INFO] Waiting for deployment rollout to complete..."
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s
if [ $? -ne 0 ]; then
    echo "[ERROR] Deployment rollout failed. Rolling back..."
    kubectl rollout undo deployment/"$APP_NAME" -n "$NAMESPACE"
    echo "[INFO] Rollback initiated. Check pod logs:"
    echo "       kubectl logs -l app=$APP_NAME -n $NAMESPACE"
    # Restore original manifest
    mv "$K8S_DIR/deployment.yaml.bak" "$K8S_DIR/deployment.yaml"
    exit 1
fi

# Restore original manifest (with placeholders) for future deployments
mv "$K8S_DIR/deployment.yaml.bak" "$K8S_DIR/deployment.yaml"

echo ""
echo "[INFO] Verifying deployed resources..."
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "[INFO] Fetching application URL from ingress..."
INGRESS_HOST=$(kubectl get ingress "$APP_NAME-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")

echo ""
echo "=============================================="
echo "[SUCCESS] Tour_Management deployed to EKS!"
echo "=============================================="
echo ""
echo "  Namespace : $NAMESPACE"
echo "  Image     : $IMAGE_URI"
echo "  Cluster   : $CLUSTER_NAME ($AWS_REGION)"
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
    echo "  App URL   : http://$INGRESS_HOST"
    echo "  Health    : http://$INGRESS_HOST/health"
else
    echo "  App URL   : (ALB provisioning - check ingress status)"
    echo "              kubectl get ingress -n $NAMESPACE"
fi
echo ""
echo "Useful commands:"
echo "  kubectl get pods -n $NAMESPACE"
echo "  kubectl logs -l app=$APP_NAME -n $NAMESPACE"
echo "  kubectl describe deployment $APP_NAME -n $NAMESPACE"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE  # rollback"
