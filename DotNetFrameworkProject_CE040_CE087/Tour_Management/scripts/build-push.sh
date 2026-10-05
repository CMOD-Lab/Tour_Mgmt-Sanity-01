#!/bin/bash
# =============================================================
# build-push.sh - Build and Push Docker Image for Tour_Management
# ASP.NET Web Forms (.NET Framework 4.7.2) on Windows Containers
# Target: AWS EKS (Windows node groups)
# =============================================================
set -e

PROJECT_NAME="tour-management"
DOCKERFILE_PATH="Tour_Management/Dockerfile"

echo "=============================================="
echo " Tour_Management - Docker Build & Push Script"
echo "=============================================="
echo ""

# ---- Tag Sanitization ----
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Select container registry:"
echo "  1. AWS ECR"
echo "  2. Docker Hub"
echo ""
read -p "Enter choice [1 or 2]: " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" = "1" ]; then
    # ---- AWS ECR ----
    echo ""
    read -p "Enter AWS Region (e.g. us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR Repository name [default: $IMAGE_NAME]: " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    ECR_REPO=$(echo "$ECR_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9/_.-' '-' | sed 's/^-*//;s/-*$//')
    read -p "Enter image tag [default: latest]: " IMAGE_TAG
    IMAGE_TAG=${IMAGE_TAG:-latest}
    IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
    IMAGE_TAG=${IMAGE_TAG:-latest}

    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

    echo ""
    echo "[INFO] Logging in to AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"

    echo "[INFO] Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || \
        aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
    echo "[INFO] ECR repository ready: $ECR_REPO"

elif [ "$REGISTRY_CHOICE" = "2" ]; then
    # ---- Docker Hub ----
    echo ""
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -s -p "Enter Docker Hub password/token: " DOCKER_PASSWORD
    echo ""
    read -p "Enter Docker Hub repository name [default: $IMAGE_NAME]: " DOCKER_REPO
    DOCKER_REPO=${DOCKER_REPO:-$IMAGE_NAME}
    DOCKER_REPO=$(echo "$DOCKER_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9/_.-' '-' | sed 's/^-*//;s/-*$//')
    read -p "Enter image tag [default: latest]: " IMAGE_TAG
    IMAGE_TAG=${IMAGE_TAG:-latest}
    IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
    IMAGE_TAG=${IMAGE_TAG:-latest}

    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${DOCKER_REPO}:${IMAGE_TAG}"

    echo ""
    echo "[INFO] Logging in to Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin

else
    echo "[ERROR] Invalid choice. Please enter 1 or 2."
    exit 1
fi

echo ""
echo "[INFO] Building Docker image: $FULL_IMAGE_NAME"
echo "[INFO] Dockerfile: $DOCKERFILE_PATH"
echo "[INFO] Build context: . (repository root)"
echo ""

# Build the image (Windows container - requires Windows Docker host or buildx)
docker build -f "$DOCKERFILE_PATH" -t "$FULL_IMAGE_NAME" .
if [ $? -ne 0 ]; then
    echo "[ERROR] Docker build failed."
    exit 1
fi
echo "[INFO] Docker build succeeded."

echo ""
echo "[INFO] Pushing image: $FULL_IMAGE_NAME"
docker push "$FULL_IMAGE_NAME"
if [ $? -ne 0 ]; then
    echo "[ERROR] Docker push failed."
    exit 1
fi

echo ""
echo "=============================================="
echo "[SUCCESS] Image pushed: $FULL_IMAGE_NAME"
echo "=============================================="
echo ""
echo "Use this image URI in deploy-image.sh:"
echo "  $FULL_IMAGE_NAME"
