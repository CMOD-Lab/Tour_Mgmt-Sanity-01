#!/bin/bash
set -e
set -o pipefail

# =============================================================
# deploy-image.sh - Deploy Tour_Management to AWS EKS
# Target Platform: AWS EKS (Windows node groups)
# =============================================================

APP_NAME="tour-management"
NAMESPACE="tour-management"
MANIFESTS_DIR="$(dirname "$0")/../kubernetes"

echo "=============================================="
echo " Tour_Management - AWS EKS Deployment Script"
echo "=============================================="
echo ""

# Prompt for AWS region
read -p "Enter AWS Region [us-east-1]: " AWS_REGION_INPUT
AWS_REGION="${AWS_REGION_INPUT:-us-east-1}"

# Prompt for EKS cluster name
read -p "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required."
  exit 1
fi

# Prompt for Docker image URI
read -p "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required."
  exit 1
fi

echo ""
echo "--- Application Environment Variables ---"
echo "Press Enter to skip any variable (it will remain as placeholder)."
echo ""

read -p "Enter DB_CONNECTION_STRING (SQL Server connection string): " DB_CONNECTION_STRING
read -p "Enter DB_HOST (SQL Server host): " DB_HOST
read -p "Enter DB_PORT [1433]: " DB_PORT_INPUT
DB_PORT="${DB_PORT_INPUT:-1433}"
read -p "Enter DB_NAME (database name): " DB_NAME
read -p "Enter DB_USER (database user): " DB_USER
read -s -p "Enter DB_PASSWORD (database password): " DB_PASSWORD
echo ""
read -p "Enter AWS_REGION for S3 [${AWS_REGION}]: " S3_AWS_REGION_INPUT
S3_AWS_REGION="${S3_AWS_REGION_INPUT:-$AWS_REGION}"
read -p "Enter S3_BUCKET_NAME (for tour image uploads): " S3_BUCKET_NAME

echo ""
echo "Configuring kubectl for EKS cluster: ${CLUSTER_NAME} in ${AWS_REGION}..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Failed to configure kubectl for EKS cluster."
  exit 1
fi

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to Kubernetes cluster."; exit 1; }

echo ""
echo "Updating Kubernetes manifests with deployment values..."

# Create working copies of manifests
cp "${MANIFESTS_DIR}/deployment.yaml" "${MANIFESTS_DIR}/deployment.yaml.bak"

# Replace image URI placeholder
sed -i 's|{{IMAGE_URI}}|'"${IMAGE_URI}"'|g' "${MANIFESTS_DIR}/deployment.yaml"

# Replace environment variable placeholders
if [ -n "$DB_CONNECTION_STRING" ]; then
  sed -i 's|{{DB_CONNECTION_STRING}}|'"${DB_CONNECTION_STRING}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
if [ -n "$DB_HOST" ]; then
  sed -i 's|{{DB_HOST}}|'"${DB_HOST}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
sed -i 's|{{DB_PORT}}|'"${DB_PORT}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
if [ -n "$DB_NAME" ]; then
  sed -i 's|{{DB_NAME}}|'"${DB_NAME}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
if [ -n "$DB_USER" ]; then
  sed -i 's|{{DB_USER}}|'"${DB_USER}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
if [ -n "$DB_PASSWORD" ]; then
  sed -i 's|{{DB_PASSWORD}}|'"${DB_PASSWORD}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
if [ -n "$S3_BUCKET_NAME" ]; then
  sed -i 's|{{S3_BUCKET_NAME}}|'"${S3_BUCKET_NAME}"'|g' "${MANIFESTS_DIR}/deployment.yaml"
fi
sed -i 's|{{AWS_REGION}}|'"${S3_AWS_REGION}"'|g' "${MANIFESTS_DIR}/deployment.yaml"

echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f "${MANIFESTS_DIR}/namespace.yaml"

echo "  [2/4] Applying deployment..."
kubectl apply -f "${MANIFESTS_DIR}/deployment.yaml"

echo "  [3/4] Applying service..."
kubectl apply -f "${MANIFESTS_DIR}/service.yaml"

echo "  [4/4] Applying ingress..."
kubectl apply -f "${MANIFESTS_DIR}/ingress.yaml"

echo ""
echo "Waiting for deployment rollout..."
kubectl rollout status deployment/${APP_NAME} -n ${NAMESPACE} --timeout=300s
if [ $? -ne 0 ]; then
  echo "ERROR: Deployment rollout failed. Rolling back..."
  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}
  # Restore original manifest
  mv "${MANIFESTS_DIR}/deployment.yaml.bak" "${MANIFESTS_DIR}/deployment.yaml"
  exit 1
fi

# Restore original manifest (with placeholders)
mv "${MANIFESTS_DIR}/deployment.yaml.bak" "${MANIFESTS_DIR}/deployment.yaml"

echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n ${NAMESPACE}

echo ""
echo "Fetching application URL..."
INGRESS_HOST=$(kubectl get ingress ${APP_NAME}-ingress -n ${NAMESPACE} -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")

echo ""
echo "=============================================="
echo " DEPLOYMENT SUCCESSFUL!"
echo " Application: ${APP_NAME}"
echo " Namespace:   ${NAMESPACE}"
echo " Image:       ${IMAGE_URI}"
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo " URL:         http://${INGRESS_HOST}"
else
  echo " URL:         (Ingress hostname pending - check 'kubectl get ingress -n ${NAMESPACE}')"
fi
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}"
