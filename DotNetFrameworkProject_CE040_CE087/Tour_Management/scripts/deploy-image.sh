#!/bin/bash
# ============================================================
# deploy-image.sh - Deploy Tour_Management to AWS EKS
# Application: Tour_Management (.NET Framework 4.7.2 / Windows Containers)
# Target: AWS EKS with Windows node groups
# ============================================================
set -e
set -o pipefail

NAMESPACE="tour-management"
APP_NAME="tour-management"
K8S_DIR="DotNetFrameworkProject_CE040_CE087/Tour_Management/kubernetes"

echo "============================================================"
echo " Tour_Management - Deploy to AWS EKS"
echo "============================================================"
echo ""

# Prompt for AWS region
read -rp "Enter AWS Region [us-east-1]: " AWS_REGION
AWS_REGION="${AWS_REGION:-us-east-1}"

# Prompt for EKS cluster name
read -rp "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required."
  exit 1
fi

# Prompt for Docker image URI
read -rp "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required."
  exit 1
fi

# Prompt for application environment variables
echo ""
echo "--- Application Environment Variables ---"
echo "(Press Enter to skip any variable)"

read -rp "Enter DB_HOST (SQL Server host): " DB_HOST
read -rp "Enter DB_PORT [1433]: " DB_PORT
DB_PORT="${DB_PORT:-1433}"
read -rp "Enter DB_NAME (database name): " DB_NAME
read -rp "Enter DB_USER (database user): " DB_USER
read -rsp "Enter DB_PASSWORD (database password): " DB_PASSWORD
echo ""
read -rp "Enter S3_BUCKET_NAME (AWS S3 bucket for file uploads): " S3_BUCKET_NAME

echo ""
echo "============================================================"
echo " Configuring kubectl for EKS cluster: $CLUSTER_NAME"
echo "============================================================"
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster."; exit 1; }

echo ""
echo "============================================================"
echo " Updating Kubernetes manifests with deployment values..."
echo "============================================================"

# Create temporary working copies of manifests
TEMP_DIR=$(mktemp -d)
cp -r "$K8S_DIR"/* "$TEMP_DIR"/

# Replace all placeholders using pipe delimiter
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g"         "$TEMP_DIR/deployment.yaml"
sed -i "s|{{DB_HOST}}|${DB_HOST}|g"             "$TEMP_DIR/deployment.yaml"
sed -i "s|{{DB_PORT}}|${DB_PORT}|g"             "$TEMP_DIR/deployment.yaml"
sed -i "s|{{DB_NAME}}|${DB_NAME}|g"             "$TEMP_DIR/deployment.yaml"
sed -i "s|{{DB_USER}}|${DB_USER}|g"             "$TEMP_DIR/deployment.yaml"
sed -i "s|{{DB_PASSWORD}}|${DB_PASSWORD}|g"     "$TEMP_DIR/deployment.yaml"
sed -i "s|{{AWS_REGION}}|${AWS_REGION}|g"       "$TEMP_DIR/deployment.yaml"
sed -i "s|{{S3_BUCKET_NAME}}|${S3_BUCKET_NAME}|g" "$TEMP_DIR/deployment.yaml"

echo ""
echo "============================================================"
echo " Applying Kubernetes manifests..."
echo "============================================================"

echo "1/4 Applying namespace..."
kubectl apply -f "$TEMP_DIR/namespace.yaml"

echo "2/4 Applying deployment..."
kubectl apply -f "$TEMP_DIR/deployment.yaml"

echo "3/4 Applying service..."
kubectl apply -f "$TEMP_DIR/service.yaml"

echo "4/4 Applying ingress..."
kubectl apply -f "$TEMP_DIR/ingress.yaml"

# Clean up temp files
rm -rf "$TEMP_DIR"

echo ""
echo "============================================================"
echo " Waiting for deployment rollout..."
echo "============================================================"
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s

echo ""
echo "============================================================"
echo " Verifying deployed resources..."
echo "============================================================"
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "============================================================"
echo " Deployment complete!"
echo " Application URL: http://tour-management.example.com"
echo " Health Check:    http://tour-management.example.com/health.ashx"
echo ""
echo " To rollback if needed:"
echo "   kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
echo "============================================================"
