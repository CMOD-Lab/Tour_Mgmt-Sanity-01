# Tour_Management - Deployment Guide

## Overview

This guide covers containerization and deployment of the **Tour_Management** ASP.NET Web Forms application to **AWS EKS (Elastic Kubernetes Service)** using Windows containers.

| Property | Value |
|---|---|
| Application | Tour_Management |
| Technology | ASP.NET Web Forms |
| Framework | .NET Framework 4.7.2 |
| Container Type | Windows Container (IIS) |
| Runtime Image | mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 |
| Application Port | 80 (HTTP via IIS) |
| Health Endpoint | `/health` |
| Target Platform | AWS EKS (Windows node groups) |

---

## Prerequisites

### Local Development
- Docker Desktop for Windows with **Windows Containers** mode enabled
- .NET Framework 4.7.2 SDK
- Visual Studio 2019+ or MSBuild 15+
- NuGet CLI

### AWS EKS Deployment
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version --client`)
- eksctl (optional, for cluster creation)
- AWS IAM permissions:
  - `ecr:GetAuthorizationToken`, `ecr:BatchCheckLayerAvailability`, `ecr:PutImage`
  - `eks:DescribeCluster`, `eks:UpdateKubeconfig`
  - `ec2:DescribeInstances` (for node group management)

### Windows Node Group Requirement
> ⚠️ **CRITICAL**: This application uses Windows containers. Your EKS cluster **must** have a Windows node group configured.

---

## Project Structure

```
Tour_Management/
├── Dockerfile                    # Multi-stage Windows container build
├── .dockerignore                 # Docker build context exclusions
├── docker-compose.yml            # Local development compose file
├── kubernetes/
│   ├── namespace.yaml            # Kubernetes namespace
│   ├── deployment.yaml           # Application deployment (Windows nodes)
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

## Local Development with Docker Compose

### Step 1: Switch Docker to Windows Containers
Right-click the Docker Desktop tray icon → **Switch to Windows containers...**

### Step 2: Configure Environment Variables
Create a `.env` file in the project root:
```env
DB_CONNECTION_STRING=Data Source=your-sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword
DB_HOST=your-sql-server
DB_PORT=1433
DB_NAME=tourdb
DB_USER=sa
DB_PASSWORD=YourPassword
AWS_REGION=us-east-1
AWS_S3_BUCKET=your-s3-bucket
```

### Step 3: Build and Run
```bash
# From repository root
docker-compose up --build
```

### Step 4: Access the Application
- Application: http://localhost:80
- Health Check: http://localhost:80/health

### Step 5: Stop the Application
```bash
docker-compose down
```

---

## Building and Pushing the Docker Image

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
2. Enter registry credentials and details
3. Enter image tag (defaults to `latest`)

The script automatically:
- Sanitizes the image name (lowercase, hyphens)
- Creates the ECR repository if it doesn't exist
- Builds and pushes the image

### Manual Build (AWS ECR)
```bash
# Authenticate to ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin \
  123456789012.dkr.ecr.us-east-1.amazonaws.com

# Build (from repository root)
docker build -f Tour_Management/Dockerfile \
  -t 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest .

# Push
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest
```

---

## AWS EKS Setup

### Step 1: Create EKS Cluster with Windows Node Group

```bash
# Create cluster (if not exists)
eksctl create cluster \
  --name tour-management-cluster \
  --region us-east-1 \
  --nodegroup-name linux-nodes \
  --node-type t3.medium \
  --nodes 2

# Add Windows node group
eksctl create nodegroup \
  --cluster tour-management-cluster \
  --region us-east-1 \
  --name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --node-ami-family WindowsServer2019FullContainer
```

### Step 2: Enable Windows Support
```bash
kubectl apply -f https://amazon-eks.s3.us-west-2.amazonaws.com/manifests/us-west-2/vpc-resource-controller/latest/vpc-resource-controller-windows.yaml
```

### Step 3: Install AWS Load Balancer Controller
```bash
# Install cert-manager
kubectl apply --validate=false -f \
  https://github.com/jetstack/cert-manager/releases/download/v1.5.4/cert-manager.yaml

# Install AWS Load Balancer Controller
helm repo add eks https://aws.github.io/eks-charts
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=tour-management-cluster \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### Step 4: Configure kubectl
```bash
aws eks update-kubeconfig \
  --region us-east-1 \
  --name tour-management-cluster
```

---

## Deploying to AWS EKS

### Using the Deploy Script (Recommended)

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
- AWS Region and EKS cluster name
- Full Docker image URI (with tag)
- Database connection details (DB_HOST, DB_NAME, DB_USER, DB_PASSWORD)
- AWS S3 configuration (AWS_REGION, AWS_S3_BUCKET)

### Manual Deployment

```bash
# 1. Update deployment.yaml with your image URI
sed -i 's|{{IMAGE_URI}}|123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest|g' \
  kubernetes/deployment.yaml

# 2. Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# 3. Wait for rollout
kubectl rollout status deployment/tour-management -n tour-management

# 4. Verify
kubectl get pods,svc,ingress -n tour-management
```

---

## Kubernetes Manifest Descriptions

### namespace.yaml
Creates the `tour-management` Kubernetes namespace to isolate all application resources.

### deployment.yaml
- **Replicas**: 2 (for high availability)
- **Node Selector**: `kubernetes.io/os: windows` (targets Windows node group)
- **Resources**: requests: 250m CPU / 512Mi RAM; limits: 500m CPU / 1Gi RAM
- **Probes**: TCP socket probes on port 80 (Windows containers don't support exec probes easily)
- **Rolling Update**: maxSurge: 1, maxUnavailable: 0 (zero-downtime deployments)

### service.yaml
ClusterIP service exposing port 80, routing traffic to application pods.

### ingress.yaml
AWS ALB Ingress with:
- Internet-facing scheme
- Health check path: `/health`
- Target type: IP (for EKS pod-level routing)

---

## Configuration Management

### Environment Variables Reference

| Variable | Description | Required |
|---|---|---|
| `DB_CONNECTION_STRING` | Full SQL Server connection string | Yes (or use individual DB_* vars) |
| `DB_HOST` | SQL Server hostname or IP | Yes |
| `DB_PORT` | SQL Server port (default: 1433) | No |
| `DB_NAME` | Database name | Yes |
| `DB_USER` | Database username | Yes |
| `DB_PASSWORD` | Database password | Yes |
| `AWS_REGION` | AWS region for S3 operations | Yes |
| `AWS_S3_BUCKET` | S3 bucket name for file storage | Yes |
| `ASPNET_ENV` | ASP.NET environment (Production) | No |

### Using Kubernetes Secrets (Recommended for Production)
```bash
kubectl create secret generic tour-management-secrets \
  --from-literal=DB_PASSWORD=your-password \
  --from-literal=DB_CONNECTION_STRING="Data Source=..." \
  -n tour-management
```

Then reference in deployment.yaml:
```yaml
env:
  - name: DB_PASSWORD
    valueFrom:
      secretKeyRef:
        name: tour-management-secrets
        key: DB_PASSWORD
```

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

### Rolling Update (New Image)
```bash
kubectl set image deployment/tour-management \
  tour-management=123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:v2.0 \
  -n tour-management

kubectl rollout status deployment/tour-management -n tour-management
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

# Check logs
kubectl logs <pod-name> -n tour-management
```

### Windows Container Issues
```bash
# Verify Windows nodes are available
kubectl get nodes -l kubernetes.io/os=windows

# Check node capacity
kubectl describe node <windows-node-name>
```

### IIS / Application Issues
```bash
# Access pod shell (Windows)
kubectl exec -it <pod-name> -n tour-management -- powershell

# Check IIS status inside pod
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-Service W3SVC"

# Test health endpoint from inside pod
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Invoke-WebRequest http://localhost/health -UseBasicParsing"
```

### Database Connection Issues
```bash
# Verify environment variables are set
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "[System.Environment]::GetEnvironmentVariable('DB_HOST')"

# Test SQL Server connectivity from pod
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Test-NetConnection -ComputerName $env:DB_HOST -Port 1433"
```

### ALB Ingress Not Provisioning
```bash
# Check AWS Load Balancer Controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller

# Check ingress events
kubectl describe ingress tour-management-ingress -n tour-management
```

---

## Security Considerations

1. **Secrets Management**: Use Kubernetes Secrets or AWS Secrets Manager for sensitive values (DB passwords, API keys). Never hardcode credentials.
2. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.
3. **IAM Roles for Service Accounts (IRSA)**: Use IRSA for AWS SDK operations (S3) instead of static credentials.
4. **ECR Image Scanning**: Enable ECR image scanning to detect vulnerabilities.
5. **Windows Security Updates**: Regularly rebuild the Docker image to include Windows security patches.
6. **SQL Server**: Use SQL Server on RDS with SSL/TLS enabled. Avoid LocalDB in production.
7. **HTTPS**: Configure SSL termination at the ALB level using ACM certificates.

### Enable HTTPS on ALB
Add to `ingress.yaml` annotations:
```yaml
alb.ingress.kubernetes.io/listen-ports: '[{"HTTP": 80}, {"HTTPS": 443}]'
alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789012:certificate/your-cert-id
alb.ingress.kubernetes.io/ssl-redirect: '443'
```

---

## .NET Framework Specific Notes

- **Windows Containers Only**: .NET Framework 4.7.2 requires Windows containers. Linux containers are not supported.
- **IIS Hosting**: The application is hosted by IIS inside the container. The `ServiceMonitor.exe` entrypoint monitors the `w3svc` Windows service.
- **Application Pool**: Configured for .NET Framework v4.0 (CLR 4.0) with ApplicationPoolIdentity.
- **Session State**: Default in-process session state. For multi-replica deployments, configure SQL Server or Redis session state.
- **Static Files**: IIS serves static files (images in `Tour_pics/`, `pics/`) directly.
- **Health Check**: The `/health` endpoint is implemented via `HealthCheckHandler.cs` (IHttpHandler) and registered in `Web.config`.

---

## Support

For issues with this deployment:
1. Check the Troubleshooting section above
2. Review pod logs: `kubectl logs -l app=tour-management -n tour-management`
3. Check EKS cluster events: `kubectl get events -n tour-management --sort-by='.lastTimestamp'`
