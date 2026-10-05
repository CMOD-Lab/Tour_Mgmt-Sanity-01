#!/bin/bash
# =============================================================================
# deploy-image.sh — Deploy Tour_Management to AWS EKS
# Prerequisites: aws-cli, kubectl
# Usage: ./scripts/deploy-image.sh  (run from repository root)
# =============================================================================
set -e
set -o pipefail

NAMESPACE="tour-management"
APP_NAME="tour-management"
K8S_DIR="DotNetFrameworkProject_CE040_CE087/Tour_Management/kubernetes"

echo "=============================================="
echo "  Tour_Management — Deploy to AWS EKS"
echo "=============================================="
echo ""

# ── Collect deployment inputs ─────────────────────────────────────────────────
read -rp "Enter AWS Region [us-east-1]: " AWS_REGION
AWS_REGION="${AWS_REGION:-us-east-1}"

read -rp "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required." >&2
  exit 1
fi

read -rp "Enter full Docker Image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker Image URI is required." >&2
  exit 1
fi

echo ""
echo "--- Application Environment Variables ---"
echo "Press Enter to skip any optional variable."
echo ""

read -rp "Enter SQL Server CONNECTION_STRING (or press Enter to skip): " CONNECTION_STRING
read -rp "Enter S3_BUCKET_NAME (or press Enter to skip): " S3_BUCKET_NAME
read -rp "Enter AWS_REGION for S3 [${AWS_REGION}]: " APP_AWS_REGION
APP_AWS_REGION="${APP_AWS_REGION:-$AWS_REGION}"
read -rp "Enter AWS_ACCESS_KEY_ID (or press Enter to skip — prefer IRSA): " APP_AWS_ACCESS_KEY_ID
read -rsp "Enter AWS_SECRET_ACCESS_KEY (or press Enter to skip — prefer IRSA): " APP_AWS_SECRET_ACCESS_KEY
echo ""

# ── Configure kubectl for EKS ─────────────────────────────────────────────────
echo ""
echo "Configuring kubectl for EKS cluster '$CLUSTER_NAME' in '$AWS_REGION'..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
echo "kubectl configured."

echo ""
echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster." >&2; exit 1; }

# ── Update Kubernetes manifests with actual values ────────────────────────────
echo ""
echo "Updating Kubernetes manifests..."

# Work on copies to avoid modifying originals
cp "${K8S_DIR}/deployment.yaml" "${K8S_DIR}/deployment.yaml.bak"

# Replace image placeholder (pipe delimiter to handle slashes in image URIs)
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" "${K8S_DIR}/deployment.yaml"

# Replace environment variable placeholders
if [ -n "$CONNECTION_STRING" ]; then
  sed -i "s|{{CONNECTION_STRING}}|${CONNECTION_STRING}|g" "${K8S_DIR}/deployment.yaml"
else
  sed -i "s|{{CONNECTION_STRING}}||g" "${K8S_DIR}/deployment.yaml"
fi

if [ -n "$S3_BUCKET_NAME" ]; then
  sed -i "s|{{S3_BUCKET_NAME}}|${S3_BUCKET_NAME}|g" "${K8S_DIR}/deployment.yaml"
else
  sed -i "s|{{S3_BUCKET_NAME}}||g" "${K8S_DIR}/deployment.yaml"
fi

sed -i "s|{{AWS_REGION}}|${APP_AWS_REGION}|g" "${K8S_DIR}/deployment.yaml"

if [ -n "$APP_AWS_ACCESS_KEY_ID" ]; then
  sed -i "s|{{AWS_ACCESS_KEY_ID}}|${APP_AWS_ACCESS_KEY_ID}|g" "${K8S_DIR}/deployment.yaml"
else
  sed -i "s|{{AWS_ACCESS_KEY_ID}}||g" "${K8S_DIR}/deployment.yaml"
fi

if [ -n "$APP_AWS_SECRET_ACCESS_KEY" ]; then
  sed -i "s|{{AWS_SECRET_ACCESS_KEY}}|${APP_AWS_SECRET_ACCESS_KEY}|g" "${K8S_DIR}/deployment.yaml"
else
  sed -i "s|{{AWS_SECRET_ACCESS_KEY}}||g" "${K8S_DIR}/deployment.yaml"
fi

echo "Manifests updated."

# ── Apply Kubernetes manifests ────────────────────────────────────────────────
echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f "${K8S_DIR}/namespace.yaml"

echo "  [2/4] Applying deployment..."
kubectl apply -f "${K8S_DIR}/deployment.yaml"

echo "  [3/4] Applying service..."
kubectl apply -f "${K8S_DIR}/service.yaml"

echo "  [4/4] Applying ingress..."
kubectl apply -f "${K8S_DIR}/ingress.yaml"

# ── Restore original deployment.yaml ─────────────────────────────────────────
mv "${K8S_DIR}/deployment.yaml.bak" "${K8S_DIR}/deployment.yaml"

# ── Wait for rollout ──────────────────────────────────────────────────────────
echo ""
echo "Waiting for deployment rollout..."
kubectl rollout status deployment/${APP_NAME} -n ${NAMESPACE} --timeout=300s

# ── Verify resources ──────────────────────────────────────────────────────────
echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n ${NAMESPACE}

# ── Display application URL ───────────────────────────────────────────────────
echo ""
echo "Fetching application URL from ingress..."
INGRESS_HOST=$(kubectl get ingress ${APP_NAME}-ingress -n ${NAMESPACE} \
  -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "")

if [ -n "$INGRESS_HOST" ]; then
  echo ""
  echo "=============================================="
  echo "  Deployment successful!"
  echo "  Application URL: http://${INGRESS_HOST}"
  echo "  Health Check:    http://${INGRESS_HOST}/health.ashx"
  echo "=============================================="
else
  echo ""
  echo "=============================================="
  echo "  Deployment successful!"
  echo "  Ingress host not yet assigned."
  echo "  Run: kubectl get ingress -n ${NAMESPACE}"
  echo "=============================================="
fi

echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}"
