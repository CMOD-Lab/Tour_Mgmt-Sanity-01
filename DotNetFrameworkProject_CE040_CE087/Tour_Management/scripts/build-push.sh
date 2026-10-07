#!/bin/bash
set -e

# =============================================================
# build-push.sh — Build and push Tour_Management Docker image
# Target: AWS EKS (Windows node groups)
# =============================================================

PROJECT_NAME="tour-management"
DOCKERFILE_PATH="DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile"

echo "=============================================="
echo " Tour_Management — Docker Build & Push"
echo "=============================================="
echo ""

# Sanitize project name: lowercase, replace non-alphanumeric with hyphens, trim hyphens
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

# Prompt for image tag
read -rp "Enter image tag [latest]: " IMAGE_TAG_INPUT
IMAGE_TAG=$(echo "$IMAGE_TAG_INPUT" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG" ]; then
  IMAGE_TAG="latest"
fi

echo ""
echo "Select container registry:"
echo "  1. AWS ECR"
echo "  2. Docker Hub"
read -rp "Enter choice [1 or 2]: " REGISTRY_CHOICE

echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
  # ---- AWS ECR ----
  read -rp "Enter AWS region [us-east-1]: " AWS_REGION_INPUT
  AWS_REGION="${AWS_REGION_INPUT:-us-east-1}"

  read -rp "Enter AWS account ID: " AWS_ACCOUNT_ID
  if [ -z "$AWS_ACCOUNT_ID" ]; then
    echo "ERROR: AWS account ID is required." >&2
    exit 1
  fi

  ECR_REPO="$IMAGE_NAME"
  REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
  FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

  echo "Logging in to AWS ECR..."
  aws ecr get-login-password --region "$AWS_REGION" | \
    docker login --username AWS --password-stdin "$REGISTRY_URL"
  if [ $? -ne 0 ]; then
    echo "ERROR: ECR login failed." >&2
    exit 1
  fi

  # Auto-create ECR repository if it does not exist
  echo "Checking ECR repository '${ECR_REPO}'..."
  aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || \
    aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # ---- Docker Hub ----
  read -rp "Enter Docker Hub username: " DOCKER_USERNAME
  if [ -z "$DOCKER_USERNAME" ]; then
    echo "ERROR: Docker Hub username is required." >&2
    exit 1
  fi

  read -rsp "Enter Docker Hub password/token: " DOCKER_PASSWORD
  echo ""
  if [ -z "$DOCKER_PASSWORD" ]; then
    echo "ERROR: Docker Hub password is required." >&2
    exit 1
  fi

  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo "Logging in to Docker Hub..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
  if [ $? -ne 0 ]; then
    echo "ERROR: Docker Hub login failed." >&2
    exit 1
  fi

else
  echo "ERROR: Invalid registry choice. Please enter 1 or 2." >&2
  exit 1
fi

echo ""
echo "Building Docker image: ${FULL_IMAGE_NAME}"
echo "Dockerfile: ${DOCKERFILE_PATH}"
echo "Build context: . (repository root)"
echo ""

# NOTE: Build context is the repository root; Dockerfile path is relative to it.
# Windows containers require --platform windows/amd64
docker build \
  --platform windows/amd64 \
  -f "$DOCKERFILE_PATH" \
  -t "$FULL_IMAGE_NAME" \
  .

if [ $? -ne 0 ]; then
  echo "ERROR: Docker build failed." >&2
  exit 1
fi

echo ""
echo "Pushing image: ${FULL_IMAGE_NAME}"
docker push "$FULL_IMAGE_NAME"

if [ $? -ne 0 ]; then
  echo "ERROR: Docker push failed." >&2
  exit 1
fi

echo ""
echo "=============================================="
echo " SUCCESS: Image pushed successfully!"
echo " Image: ${FULL_IMAGE_NAME}"
echo "=============================================="
