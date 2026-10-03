# Tour_Management - AWS EKS Deployment Guide

## Overview

This guide covers the complete deployment of the **Tour_Management** ASP.NET Web Forms application (.NET Framework 4.7.2) to AWS EKS using Windows container node groups.

- **Application**: Tour_Management
- **Framework**: ASP.NET Web Forms on .NET Framework 4.7.2
- **Container Type**: Windows Container (IIS-hosted)
- **Runtime Base Image**: `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2`
- **Health Endpoint**: `/Health.ashx`
- **Application Port**: 80 (HTTP via IIS)
- **Target Platform**: AWS EKS

---

## Prerequisites

### Local Development Tools
- Docker Desktop (with Windows container support enabled)
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version --client`)
- Git

### AWS Requirements
- AWS Account with appropriate IAM permissions
- EKS cluster with **Windows node groups** configured
- ECR repository access
- IAM role with the following policies:
  - `AmazonEKSClusterPolicy`
  - `AmazonEKSWorkerNodePolicy`
  - `AmazonEC2ContainerRegistryReadOnly`
  - `AmazonS3FullAccess` (for tour image uploads)

### Windows Container Requirements
> **IMPORTANT**: This application uses Windows containers. Your EKS cluster **must** have Windows node groups configured. Linux-only clusters will not work.

---

## Project Structure

```
Tour_Management/
├── Dockerfile                    # Multi-stage Windows container build
├── .dockerignore                 # Excludes build artifacts from image
├── docker-compose.yml            # Local development compose file
├── Web.config                    # ASP.NET configuration
├── Health.ashx / Health.ashx.cs  # Health check endpoint
├── kubernetes/
│   ├── namespace.yaml            # Kubernetes namespace
│   ├── deployment.yaml           # Deployment manifest (2 replicas)
│   ├── service.yaml              # ClusterIP service
│   └── ingress.yaml              # AWS ALB ingress
├── scripts/
│   ├── build-push.sh             # Linux/macOS build & push script
│   ├── build-push.bat            # Windows build & push script
│   ├── deploy-image.sh           # Linux/macOS EKS deploy script
│   └── deploy-image.bat          # Windows EKS deploy script
└── docs/
    └── DEPLOYMENT.md             # This file
```

---

## Step 1: Local Development with Docker Compose

### Switch Docker to Windows Containers
Right-click the Docker Desktop tray icon → **Switch to Windows containers**.

### Configure Environment Variables
Create a `.env` file in the project root:
```env
DB_CONNECTION_STRING=Server=your-sql-server;Database=tourdb;User Id=sa;Password=yourpassword;
DB_HOST=your-sql-server
DB_PORT=1433
DB_NAME=tourdb
DB_USER=sa
DB_PASSWORD=yourpassword
AWS_REGION=us-east-1
S3_BUCKET_NAME=your-tour-images-bucket
```

### Build and Run Locally
```bash
# From the repository root
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml up --build
```

### Verify Local Health Check
```
http://localhost/Health.ashx
```
Expected response: `{"status":"healthy","application":"Tour_Management","timestamp":"..."}`

---

## Step 2: Build and Push Docker Image

### Linux/macOS
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

### Windows
```cmd
scripts\build-push.bat
```

The script will prompt you to:
1. Select registry type (AWS ECR or Docker Hub)
2. Enter registry credentials/details
3. Enter image tag (defaults to `latest`)

### Manual Build (AWS ECR)
```bash
# Authenticate to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

# Create ECR repository (if not exists)
aws ecr create-repository --repository-name tour-management --region us-east-1

# Build image (from repository root)
docker build -f DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile \
  -t 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest .

# Push image
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest
```

---

## Step 3: AWS EKS Cluster Setup

### Install Required Tools
```bash
# Install eksctl
curl --silent --location "https://github.com/weaveworks/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin

# Install kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl
```

### Create EKS Cluster with Windows Node Group
```bash
eksctl create cluster \
  --name tour-management-cluster \
  --region us-east-1 \
  --nodegroup-name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --node-ami-family WindowsServer2019FullContainer \
  --managed
```

### Configure kubectl
```bash
aws eks update-kubeconfig --region us-east-1 --name tour-management-cluster
kubectl cluster-info
```

### Install AWS Load Balancer Controller
```bash
# Add EKS Helm chart repository
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Install AWS Load Balancer Controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=tour-management-cluster \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

---

## Step 4: Deploy to AWS EKS

### Using Deployment Scripts

**Linux/macOS:**
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

**Windows:**
```cmd
scripts\deploy-image.bat
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Docker image URI (full path with tag)
- Database connection details
- S3 bucket configuration

### Manual Deployment
```bash
# Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# Wait for rollout
kubectl rollout status deployment/tour-management -n tour-management --timeout=300s

# Verify resources
kubectl get pods,svc,ingress -n tour-management
```

---

## Step 5: Configuration Management

### Environment Variables Reference

| Variable | Description | Required |
|----------|-------------|----------|
| `DB_CONNECTION_STRING` | Full SQL Server connection string | Yes |
| `DB_HOST` | SQL Server hostname | Yes |
| `DB_PORT` | SQL Server port (default: 1433) | No |
| `DB_NAME` | Database name | Yes |
| `DB_USER` | Database username | Yes |
| `DB_PASSWORD` | Database password | Yes |
| `AWS_REGION` | AWS region for S3 | Yes |
| `S3_BUCKET_NAME` | S3 bucket for tour image uploads | Yes |
| `ASPNET_ENVIRONMENT` | ASP.NET environment (Production) | No |

### Using Kubernetes Secrets (Recommended)
```bash
kubectl create secret generic tour-management-secrets \
  --from-literal=DB_CONNECTION_STRING="Server=your-server;Database=tourdb;..." \
  --from-literal=DB_PASSWORD="yourpassword" \
  -n tour-management
```

Update `deployment.yaml` to reference secrets:
```yaml
env:
  - name: DB_PASSWORD
    valueFrom:
      secretKeyRef:
        name: tour-management-secrets
        key: DB_PASSWORD
```

---

## Kubernetes Manifest Descriptions

### namespace.yaml
Creates the `tour-management` namespace to isolate all application resources.

### deployment.yaml
- **Replicas**: 2 (for high availability)
- **Node Selector**: `kubernetes.io/os: windows` (requires Windows node group)
- **Resources**: 250m CPU / 512Mi memory (requests), 500m CPU / 1Gi memory (limits)
- **Liveness Probe**: HTTP GET `/Health.ashx` every 30s (starts after 90s)
- **Readiness Probe**: HTTP GET `/Health.ashx` every 15s (starts after 60s)

### service.yaml
ClusterIP service exposing port 80, routing to application pods.

### ingress.yaml
AWS ALB Ingress with internet-facing scheme. Uses `/Health.ashx` for ALB health checks.

---

## Scaling and Management

### Horizontal Pod Autoscaling
```bash
kubectl autoscale deployment tour-management \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n tour-management
```

### Rolling Update
```bash
kubectl set image deployment/tour-management \
  tour-management=123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:v2.0 \
  -n tour-management
```

### Rollback
```bash
kubectl rollout undo deployment/tour-management -n tour-management
# Rollback to specific revision
kubectl rollout undo deployment/tour-management --to-revision=2 -n tour-management
```

---

## Troubleshooting

### Pod Not Starting
```bash
# Check pod status
kubectl get pods -n tour-management

# Describe pod for events
kubectl describe pod <pod-name> -n tour-management

# View container logs
kubectl logs <pod-name> -n tour-management
```

### Windows Container Issues
```bash
# Verify Windows nodes are available
kubectl get nodes -l kubernetes.io/os=windows

# Check node selector in deployment
kubectl describe deployment tour-management -n tour-management | grep -A5 "Node-Selectors"
```

### Health Check Failures
```bash
# Test health endpoint directly
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Invoke-WebRequest -Uri http://localhost/Health.ashx -UseBasicParsing"

# Check IIS application pool status
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-WebConfiguration system.applicationHost/applicationPools/add | Select-Object name, state"
```

### Database Connection Issues
```bash
# Verify environment variables are set
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "[System.Environment]::GetEnvironmentVariable('DB_CONNECTION_STRING')"

# Test SQL Server connectivity
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Test-NetConnection -ComputerName <DB_HOST> -Port 1433"
```

### Ingress/ALB Issues
```bash
# Check ingress status
kubectl describe ingress tour-management-ingress -n tour-management

# Verify AWS Load Balancer Controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller
```

---

## Security Considerations

1. **Secrets Management**: Use Kubernetes Secrets or AWS Secrets Manager for sensitive values (DB passwords, connection strings).
2. **IRSA (IAM Roles for Service Accounts)**: Configure IRSA for S3 access instead of embedding AWS credentials.
3. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.
4. **Image Scanning**: Enable ECR image scanning to detect vulnerabilities.
5. **Least Privilege**: Ensure the EKS node IAM role has only required permissions.
6. **TLS/HTTPS**: Configure HTTPS on the ALB ingress using ACM certificates:
   ```yaml
   annotations:
     alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789:certificate/xxx
     alb.ingress.kubernetes.io/listen-ports: '[{"HTTPS":443}]'
   ```

---

## .NET Framework Specific Notes

- This application runs on **Windows containers** using IIS as the web server.
- The `ServiceMonitor.exe` entrypoint monitors the IIS `w3svc` Windows service.
- Application pool is configured for .NET Framework 4.0 in Integrated Pipeline mode.
- The `Health.ashx` HTTP handler provides health check responses for Kubernetes probes.
- Connection strings are read from the `DB_CONNECTION_STRING` environment variable (not Web.config).
- AWS SDK for S3 uses IRSA (IAM Roles for Service Accounts) - no embedded credentials needed.
- Windows containers require significantly more startup time than Linux containers; health probe `initialDelaySeconds` is set accordingly (60-90s).
