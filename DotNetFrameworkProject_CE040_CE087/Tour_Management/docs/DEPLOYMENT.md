# Tour_Management - Deployment Guide

## Overview

This guide covers containerization and deployment of the **Tour_Management** ASP.NET Web Forms application to **AWS EKS** (Elastic Kubernetes Service).

| Property | Value |
|---|---|
| Application | Tour_Management |
| Framework | .NET Framework 4.7.2 |
| Application Type | ASP.NET Web Forms (IIS) |
| Container Type | **Windows Container** |
| Runtime Base Image | `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2` |
| Build Base Image | `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019` |
| Application Port | 80 (HTTP / IIS) |
| Health Endpoint | `/health.ashx` |
| Target Platform | AWS EKS (Windows node groups) |

---

## Prerequisites

### Local Development
- Docker Desktop (Windows) with **Windows Containers** mode enabled
- .NET Framework 4.7.2 SDK
- Visual Studio 2019+ or MSBuild 15+
- NuGet CLI

### AWS EKS Deployment
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version`)
- eksctl (optional, for cluster creation)
- AWS IAM permissions:
  - `ecr:*` (ECR push/pull)
  - `eks:DescribeCluster`
  - `eks:UpdateKubeconfig`

---

## Project Structure

```
Tour_Management/
├── Dockerfile                    # Multi-stage Windows container build
├── .dockerignore                 # Excludes build artifacts from image
├── docker-compose.yml            # Local development compose file
├── Web.config                    # ASP.NET configuration
├── health.ashx                   # Health check endpoint
├── kubernetes/
│   ├── namespace.yaml            # Kubernetes namespace
│   ├── deployment.yaml           # Deployment manifest
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

## ⚠️ Windows Container Requirement

This application uses **Windows Containers** because it targets .NET Framework 4.7.2 with IIS hosting. This has important implications:

- Docker must be in **Windows Containers** mode (right-click Docker tray icon → "Switch to Windows containers")
- EKS cluster must have **Windows node groups** configured
- Windows container images are significantly larger than Linux images (~5-10 GB)
- Build times are longer due to Windows base image size

---

## Local Development Setup

### 1. Switch Docker to Windows Containers

Right-click the Docker Desktop tray icon and select **"Switch to Windows containers..."**

### 2. Configure Environment Variables

Create a `.env` file in the project root:

```env
DB_HOST=your-sql-server-host
DB_PORT=1433
DB_NAME=tourdb
DB_USER=sa
DB_PASSWORD=YourPassword123!
AWS_REGION=us-east-1
S3_BUCKET_NAME=your-tour-images-bucket
```

### 3. Build and Run with Docker Compose

```bash
# From the repository root
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml up --build
```

### 4. Access the Application

- Application: http://localhost:80
- Health Check: http://localhost:80/health.ashx

### 5. Stop the Application

```bash
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml down
```

---

## Build and Push Docker Image

### Linux/macOS

```bash
# Make script executable
chmod +x DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/build-push.sh

# Run from repository root
./DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/build-push.sh
```

### Windows

```cmd
# Run from repository root
DotNetFrameworkProject_CE040_CE087\Tour_Management\scripts\build-push.bat
```

The script will prompt you to:
1. Enter an image tag (default: `latest`)
2. Select registry type (AWS ECR or Docker Hub)
3. Provide registry credentials

**Note:** The build context is always the repository root (`.`). The Dockerfile path is specified explicitly.

---

## AWS EKS Prerequisites

### 1. Create ECR Repository (if not using build-push script)

```bash
aws ecr create-repository \
  --repository-name tour-management \
  --region us-east-1
```

### 2. Configure EKS Windows Node Group

Your EKS cluster must have a Windows node group. If not already configured:

```bash
eksctl create nodegroup \
  --cluster your-cluster-name \
  --name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --node-ami-family WindowsServer2019FullContainer \
  --region us-east-1
```

### 3. Install AWS Load Balancer Controller

The ingress manifest uses the AWS Load Balancer Controller. Install it if not present:

```bash
# Add the EKS chart repo
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Install the controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=your-cluster-name \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### 4. Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name your-cluster-name
kubectl cluster-info
```

---

## Deploy to AWS EKS

### Linux/macOS

```bash
chmod +x DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/deploy-image.sh
./DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/deploy-image.sh
```

### Windows

```cmd
DotNetFrameworkProject_CE040_CE087\Tour_Management\scripts\deploy-image.bat
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Full Docker image URI (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest`)
- Database connection details (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD)
- S3 bucket name for file uploads

### Manual Deployment

```bash
# 1. Set your image URI
export IMAGE_URI="123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest"

# 2. Update deployment manifest
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" kubernetes/deployment.yaml
# Update other placeholders similarly...

# 3. Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# 4. Wait for rollout
kubectl rollout status deployment/tour-management -n tour-management

# 5. Verify
kubectl get pods,svc,ingress -n tour-management
```

---

## Kubernetes Manifest Descriptions

### namespace.yaml
Creates the `tour-management` Kubernetes namespace to isolate application resources.

### deployment.yaml
- **Replicas**: 2 (for high availability)
- **Node Selector**: `kubernetes.io/os: windows` (required for Windows containers)
- **Image**: Placeholder `{{IMAGE_URI}}` replaced at deploy time
- **Resources**: 250m CPU / 512Mi memory (requests), 500m CPU / 1Gi memory (limits)
- **Liveness Probe**: HTTP GET `/health.ashx` — restarts container if unhealthy
- **Readiness Probe**: HTTP GET `/health.ashx` — removes from load balancer if not ready
- **Environment Variables**: Database and AWS configuration via env vars

### service.yaml
- **Type**: ClusterIP (internal cluster access)
- **Port**: 80 → 80 (HTTP)
- Routes traffic to pods with label `app: tour-management`

### ingress.yaml
- **Class**: AWS ALB (Application Load Balancer)
- **Scheme**: internet-facing
- **Health Check Path**: `/health.ashx`
- **Host**: `tour-management.example.com` (update to your actual domain)

---

## Configuration Management

### Database Connection

The application uses SQL Server. Configure via environment variables:

| Variable | Description | Example |
|---|---|---|
| `DB_HOST` | SQL Server hostname | `my-rds.us-east-1.rds.amazonaws.com` |
| `DB_PORT` | SQL Server port | `1433` |
| `DB_NAME` | Database name | `tourdb` |
| `DB_USER` | Database username | `tourapp` |
| `DB_PASSWORD` | Database password | (use Kubernetes Secret) |

### Using Kubernetes Secrets for Sensitive Data

```bash
kubectl create secret generic tour-management-db-secret \
  --from-literal=DB_PASSWORD='YourPassword123!' \
  --from-literal=DB_USER='tourapp' \
  -n tour-management
```

Then reference in deployment.yaml:
```yaml
- name: DB_PASSWORD
  valueFrom:
    secretKeyRef:
      name: tour-management-db-secret
      key: DB_PASSWORD
```

### AWS S3 Configuration

The application uses AWS S3 for file uploads (tour images). Configure:

| Variable | Description |
|---|---|
| `AWS_REGION` | AWS region for S3 bucket |
| `S3_BUCKET_NAME` | S3 bucket name for tour images |

Ensure the EKS node IAM role has `s3:PutObject`, `s3:GetObject`, `s3:DeleteObject` permissions on the bucket.

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

### Rolling Updates

```bash
# Update image
kubectl set image deployment/tour-management \
  tour-management=123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:v2.0 \
  -n tour-management

# Monitor rollout
kubectl rollout status deployment/tour-management -n tour-management
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/tour-management -n tour-management

# Rollback to specific revision
kubectl rollout history deployment/tour-management -n tour-management
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

# Check logs
kubectl logs <pod-name> -n tour-management
```

### Windows Container Issues

```bash
# Verify node selector is correct
kubectl get nodes -l kubernetes.io/os=windows

# Check if Windows nodes are available
kubectl get nodes -o wide
```

### Health Check Failures

```bash
# Test health endpoint directly
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Invoke-WebRequest -Uri http://localhost/health.ashx -UseBasicParsing"

# Check IIS application pool status
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-WebConfiguration system.applicationHost/applicationPools/add | Select-Object name, state"
```

### Database Connection Issues

```bash
# Verify environment variables are set
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-ChildItem Env: | Where-Object { $_.Name -like 'DB_*' }"

# Test SQL Server connectivity
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Test-NetConnection -ComputerName $env:DB_HOST -Port $env:DB_PORT"
```

### Ingress / ALB Issues

```bash
# Check ingress status
kubectl describe ingress tour-management-ingress -n tour-management

# Check ALB controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller
```

---

## Security Considerations

1. **Secrets Management**: Use Kubernetes Secrets or AWS Secrets Manager for DB_PASSWORD and other sensitive values — never hardcode in manifests.
2. **IAM Roles for Service Accounts (IRSA)**: Use IRSA to grant the application pod access to S3 without embedding AWS credentials.
3. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicy.
4. **Image Scanning**: Enable ECR image scanning to detect vulnerabilities in the Windows base image.
5. **Windows Updates**: Regularly rebuild the container image to pick up Windows security patches from the base image.
6. **HTTPS**: Configure HTTPS on the ALB using ACM certificates. Update ingress annotations:
   ```yaml
   alb.ingress.kubernetes.io/listen-ports: '[{"HTTP": 80}, {"HTTPS": 443}]'
   alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789:certificate/xxx
   alb.ingress.kubernetes.io/ssl-redirect: '443'
   ```

---

## .NET Framework Specific Notes

- **Windows Containers Only**: .NET Framework 4.7.2 requires Windows containers. Linux containers are not supported.
- **IIS Hosting**: The application is hosted by IIS inside the container. The DefaultAppPool is configured for .NET 4.0 in Integrated Pipeline mode.
- **App_Data**: Local database files (`.mdf`) are not supported in containers. Use an external SQL Server (e.g., Amazon RDS for SQL Server).
- **Session State**: If using InProc session state, sessions will not persist across pod restarts. Consider SQL Server session state or Redis for distributed sessions.
- **File Uploads**: Local file system writes are replaced with AWS S3 uploads. Ensure `S3_BUCKET_NAME` and `AWS_REGION` are configured.
- **Startup Time**: Windows containers take longer to start (60-90 seconds). The readiness probe has a 60-second initial delay to accommodate this.
- **Image Size**: Windows container images are large (~5-10 GB). Use ECR for storage to avoid Docker Hub rate limits.
