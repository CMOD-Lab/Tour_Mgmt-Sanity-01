#!/bin/bash
set -e
set -o pipefail

# =============================================================
# deploy-image.sh — Deploy Tour_Management to AWS EKS
# =============================================================

APP_NAME="tour-management"
NAMESPACE="tour-management"
MANIFEST_DIR="$(cd "$(dirname "$0")/.." && pwd)/kubernetes"

echo "=============================================="
echo " Tour_Management — Deploy to AWS EKS"
echo "=============================================="
echo ""

# ---- Collect deployment inputs ----
read -rp "Enter AWS region [us-east-1]: " AWS_REGION_INPUT
AWS_REGION="${AWS_REGION_INPUT:-us-east-1}"

read -rp "Enter EKS cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS cluster name is required." >&2
  exit 1
fi

read -rp "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required." >&2
  exit 1
fi

echo ""
echo "--- Optional: Application Environment Variables ---"
echo "Press Enter to skip any variable."
echo ""

read -rp "Enter DB_HOST (SQL Server hostname or IP): " DB_HOST
read -rp "Enter DB_PORT [1433]: " DB_PORT_INPUT
DB_PORT="${DB_PORT_INPUT:-1433}"
read -rp "Enter DB_NAME (database name): " DB_NAME
read -rp "Enter AWS_REGION for app [${AWS_REGION}]: " APP_AWS_REGION_INPUT
APP_AWS_REGION="${APP_AWS_REGION_INPUT:-$AWS_REGION}"
read -rp "Enter S3_BUCKET_NAME: " S3_BUCKET_NAME

echo ""
echo "--- Configuring kubectl for EKS cluster '${CLUSTER_NAME}' ---"
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Failed to configure kubectl for EKS cluster." >&2
  exit 1
fi

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster." >&2; exit 1; }

echo ""
echo "--- Updating Kubernetes manifests with deployment values ---"

# Work on copies to avoid modifying originals
DEPLOY_TMP_DIR=$(mktemp -d)
cp "$MANIFEST_DIR"/*.yaml "$DEPLOY_TMP_DIR/"

# Replace placeholders using pipe delimiter
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g"           "$DEPLOY_TMP_DIR/deployment.yaml"
sed -i "s|{{DB_HOST}}|${DB_HOST}|g"               "$DEPLOY_TMP_DIR/deployment.yaml"
sed -i "s|{{DB_PORT}}|${DB_PORT}|g"               "$DEPLOY_TMP_DIR/deployment.yaml"
sed -i "s|{{DB_NAME}}|${DB_NAME}|g"               "$DEPLOY_TMP_DIR/deployment.yaml"
sed -i "s|{{AWS_REGION}}|${APP_AWS_REGION}|g"     "$DEPLOY_TMP_DIR/deployment.yaml"
sed -i "s|{{S3_BUCKET_NAME}}|${S3_BUCKET_NAME}|g" "$DEPLOY_TMP_DIR/deployment.yaml"

echo ""
echo "--- Applying Kubernetes manifests ---"

echo "1/4 Applying namespace..."
kubectl apply -f "$DEPLOY_TMP_DIR/namespace.yaml"

echo "2/4 Applying deployment..."
kubectl apply -f "$DEPLOY_TMP_DIR/deployment.yaml"

echo "3/4 Applying service..."
kubectl apply -f "$DEPLOY_TMP_DIR/service.yaml"

echo "4/4 Applying ingress..."
kubectl apply -f "$DEPLOY_TMP_DIR/ingress.yaml"

echo ""
echo "--- Waiting for deployment rollout ---"
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s
if [ $? -ne 0 ]; then
  echo "ERROR: Deployment rollout failed. Rolling back..." >&2
  kubectl rollout undo deployment/"$APP_NAME" -n "$NAMESPACE"
  exit 1
fi

echo ""
echo "--- Verifying deployed resources ---"
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "--- Application Access URL ---"
INGRESS_HOST=$(kubectl get ingress tour-management-ingress -n "$NAMESPACE" \
  -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo "Application URL: http://${INGRESS_HOST}"
else
  echo "Ingress hostname is still provisioning. Run the following to check:"
  echo "  kubectl get ingress tour-management-ingress -n ${NAMESPACE}"
fi

# Cleanup temp directory
rm -rf "$DEPLOY_TMP_DIR"

echo ""
echo "=============================================="
echo " SUCCESS: Tour_Management deployed to EKS!"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}"
