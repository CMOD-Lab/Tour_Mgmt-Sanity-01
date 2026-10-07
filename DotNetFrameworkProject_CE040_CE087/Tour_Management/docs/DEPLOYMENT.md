# Tour_Management — Deployment Guide

## Overview

This guide covers building, containerizing, and deploying the **Tour_Management** ASP.NET Web Forms application to **AWS EKS** (Elastic Kubernetes Service) using Windows node groups.

| Property | Value |
|---|---|
| Application | Tour_Management |
| Framework | .NET Framework 4.7.2 |
| Application Type | ASP.NET Web Forms (IIS-hosted) |
| Container OS | Windows Server Core (LTSC 2019) |
| Runtime Image | mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 |
| Builder Image | mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 |
| Port | 80 (HTTP) |
| Health Endpoint | /Health.aspx |
| Target Platform | AWS EKS |

---

## Prerequisites

### Local Development
- Docker Desktop (Windows containers enabled)
- Windows 10/11 or Windows Server 2019+ (for Windows container builds)
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version --client`)
- Git

### AWS EKS Deployment
- AWS account with appropriate IAM permissions
- EKS cluster with **Windows node groups** (Windows Server 2019 LTSC)
- AWS Load Balancer Controller installed on the cluster
- Amazon ECR repository (auto-created by build-push.sh)
- kubectl configured for the target cluster

### Required IAM Permissions
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecr:GetAuthorizationToken",
        "ecr:BatchCheckLayerAvailability",
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage",
        "ecr:PutImage",
        "ecr:InitiateLayerUpload",
        "ecr:UploadLayerPart",
        "ecr:CompleteLayerUpload",
        "ecr:CreateRepository",
        "ecr:DescribeRepositories",
        "eks:DescribeCluster",
        "eks:UpdateClusterConfig"
      ],
      "Resource": "*"
    }
  ]
}
```

---

## Project Structure

```
Tour_Management/
├── Dockerfile                    # Multi-stage Windows container build
├── .dockerignore                 # Excludes bin/, obj/, App_Data/, etc.
├── docker-compose.yml            # Local development with Docker Compose
├── Web.config                    # ASP.NET configuration
├── packages.config               # NuGet packages (AWSSDK.Core, AWSSDK.S3)
├── Health.aspx / Health.aspx.cs  # Health check endpoint (/Health.aspx)
├── scripts/
│   ├── build-push.sh             # Linux/macOS: build & push to ECR or Docker Hub
│   ├── build-push.bat            # Windows: build & push to ECR or Docker Hub
│   ├── deploy-image.sh           # Linux/macOS: deploy to AWS EKS
│   └── deploy-image.bat          # Windows: deploy to AWS EKS
├── kubernetes/
│   ├── namespace.yaml            # Kubernetes namespace
│   ├── deployment.yaml           # Deployment with Windows node selector
│   ├── service.yaml              # ClusterIP service
│   └── ingress.yaml              # ALB Ingress (AWS Load Balancer Controller)
└── docs/
    └── DEPLOYMENT.md             # This file
```

---

## Step 1: Local Development with Docker Compose

> **Note**: Windows containers require Docker Desktop in Windows container mode.

### Switch Docker to Windows Containers
Right-click the Docker Desktop tray icon → **Switch to Windows containers...**

### Build and Run Locally
```bash
# From the repository root (comp2/)
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml up --build
```

### Access the Application
- Application: http://localhost:80
- Health check: http://localhost:80/Health.aspx

### Environment Variables for Local Development
Create a `.env` file in the same directory as `docker-compose.yml`:
```env
DB_HOST=your-sql-server-host
DB_PORT=1433
DB_NAME=tourdb
DB_USER=sa
DB_PASSWORD=YourPassword123!
AWS_REGION=us-east-1
S3_BUCKET_NAME=your-tour-images-bucket
```

### Stop the Application
```bash
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml down
```

---

## Step 2: Build and Push Docker Image

> **Important**: Windows container images must be built on a Windows host or a Windows-based CI/CD agent.

### Linux/macOS (cross-build requires Windows Docker host)
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

### Windows
```cmd
scripts\build-push.bat
```

### Script Prompts
| Prompt | Description |
|---|---|
| Image tag | Docker image tag (default: `latest`) |
| Registry choice | `1` for AWS ECR, `2` for Docker Hub |
| AWS region | AWS region for ECR (e.g., `us-east-1`) |
| AWS account ID | Your 12-digit AWS account ID |

### Manual Build (from repository root)
```bash
docker build \
  --platform windows/amd64 \
  -f DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile \
  -t <your-registry>/tour-management:latest \
  .
```

---

## Step 3: AWS EKS Prerequisites

### 3.1 Install Required Tools
```bash
# AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip && sudo ./aws/install

# kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
chmod +x kubectl && sudo mv kubectl /usr/local/bin/

# eksctl (optional, for cluster creation)
curl --silent --location "https://github.com/weaveworks/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin
```

### 3.2 Configure AWS CLI
```bash
aws configure
# Enter: AWS Access Key ID, Secret Access Key, Region, Output format
```

### 3.3 Create EKS Cluster with Windows Node Group (if not existing)
```bash
eksctl create cluster \
  --name tour-management-cluster \
  --region us-east-1 \
  --nodegroup-name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --managed \
  --node-ami-family WindowsServer2019FullContainer
```

### 3.4 Install AWS Load Balancer Controller
```bash
# Add EKS chart repo
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Install AWS Load Balancer Controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=tour-management-cluster \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### 3.5 Configure kubectl
```bash
aws eks update-kubeconfig --region us-east-1 --name tour-management-cluster
kubectl cluster-info
```

---

## Step 4: Deploy to AWS EKS

### Linux/macOS
```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

### Windows
```cmd
scripts\deploy-image.bat
```

### Script Prompts
| Prompt | Description |
|---|---|
| AWS region | AWS region of your EKS cluster |
| EKS cluster name | Name of your EKS cluster |
| Docker image URI | Full image URI (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest`) |
| DB_HOST | SQL Server hostname or IP |
| DB_PORT | SQL Server port (default: 1433) |
| DB_NAME | Database name |
| AWS_REGION | AWS region for S3 operations |
| S3_BUCKET_NAME | S3 bucket for tour images |

### Manual Deployment
```bash
# Configure kubectl
aws eks update-kubeconfig --region us-east-1 --name your-cluster-name

# Update image in deployment.yaml
sed -i 's|{{IMAGE_URI}}|123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest|g' kubernetes/deployment.yaml

# Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# Wait for rollout
kubectl rollout status deployment/tour-management -n tour-management
```

---

## Step 5: Verify Deployment

```bash
# Check all resources
kubectl get pods,svc,ingress -n tour-management

# Check pod logs
kubectl logs -l app=tour-management -n tour-management --tail=50

# Check pod details
kubectl describe pod -l app=tour-management -n tour-management

# Get ingress hostname
kubectl get ingress tour-management-ingress -n tour-management
```

### Expected Output
```
NAME                                    READY   STATUS    RESTARTS   AGE
pod/tour-management-xxxxxxxxx-xxxxx     1/1     Running   0          2m

NAME                              TYPE        CLUSTER-IP      PORT(S)   AGE
service/tour-management-service   ClusterIP   10.100.x.x      80/TCP    2m

NAME                                        CLASS   HOSTS                          ADDRESS
ingress.networking.k8s.io/tour-management   alb     tour-management.example.com    xxx.elb.amazonaws.com
```

---

## Kubernetes Secrets Management

### Create Database Secret
```bash
kubectl create secret generic tour-management-db-secret \
  --namespace tour-management \
  --from-literal=db-user='your-db-username' \
  --from-literal=db-password='your-db-password'
```

### Verify Secret
```bash
kubectl get secret tour-management-db-secret -n tour-management
```

---

## Scaling and Management

### Manual Scaling
```bash
kubectl scale deployment tour-management --replicas=3 -n tour-management
```

### Horizontal Pod Autoscaler (HPA)
```bash
kubectl autoscale deployment tour-management \
  --namespace tour-management \
  --cpu-percent=70 \
  --min=2 \
  --max=10
```

### Rolling Update
```bash
# Update image
kubectl set image deployment/tour-management \
  tour-management=<new-image-uri> \
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
# Check pod events
kubectl describe pod -l app=tour-management -n tour-management

# Check pod logs
kubectl logs -l app=tour-management -n tour-management

# Check node availability (Windows nodes)
kubectl get nodes -l kubernetes.io/os=windows
```

### Windows Container Issues
```bash
# Verify Windows node group is available
kubectl get nodes -l kubernetes.io/os=windows --show-labels

# Check if pod is scheduled on Windows node
kubectl get pod -l app=tour-management -n tour-management -o wide
```

### IIS / Application Issues
```bash
# Exec into running container (Windows)
kubectl exec -it <pod-name> -n tour-management -- powershell

# Check IIS status inside container
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-Service W3SVC"

# Check application event log
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-EventLog -LogName Application -Newest 20"
```

### Health Check Failures
```bash
# Test health endpoint directly
kubectl port-forward svc/tour-management-service 8080:80 -n tour-management
# Then visit: http://localhost:8080/Health.aspx

# Check liveness/readiness probe status
kubectl describe pod -l app=tour-management -n tour-management | grep -A 10 "Liveness\|Readiness"
```

### Database Connection Issues
```bash
# Verify secret exists
kubectl get secret tour-management-db-secret -n tour-management

# Check environment variables in pod
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-ChildItem Env:"
```

### Ingress / ALB Issues
```bash
# Check ingress status
kubectl describe ingress tour-management-ingress -n tour-management

# Check AWS Load Balancer Controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller --tail=50
```

---

## Configuration Management

### Web.config Environment-Specific Settings
The application uses `Web.config` for configuration. In containers, override connection strings and app settings via environment variables:

| Environment Variable | Description |
|---|---|
| `DB_HOST` | SQL Server hostname |
| `DB_PORT` | SQL Server port (default: 1433) |
| `DB_NAME` | Database name |
| `DB_USER` | Database username (from Kubernetes Secret) |
| `DB_PASSWORD` | Database password (from Kubernetes Secret) |
| `AWS_REGION` | AWS region for SDK operations |
| `S3_BUCKET_NAME` | S3 bucket for tour image uploads |

### ConfigMap for Non-Sensitive Configuration
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: tour-management-config
  namespace: tour-management
data:
  DB_HOST: "your-sql-server.rds.amazonaws.com"
  DB_PORT: "1433"
  DB_NAME: "tourdb"
  AWS_REGION: "us-east-1"
  S3_BUCKET_NAME: "tour-management-images"
```

---

## Security Considerations

1. **Secrets Management**: Never store database passwords or AWS credentials in environment variables directly. Use Kubernetes Secrets or AWS Secrets Manager.
2. **IRSA (IAM Roles for Service Accounts)**: Configure IRSA for the pod to access S3 without static credentials.
3. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.
4. **Image Scanning**: Enable ECR image scanning to detect vulnerabilities.
5. **Windows Security Updates**: Regularly rebuild the container image to include Windows security patches.
6. **Least Privilege**: Ensure the EKS node IAM role has only the minimum required permissions.

### IRSA Setup for S3 Access
```bash
# Create IAM policy for S3 access
aws iam create-policy \
  --policy-name TourManagementS3Policy \
  --policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Action": ["s3:GetObject", "s3:PutObject", "s3:DeleteObject", "s3:ListBucket"],
      "Resource": ["arn:aws:s3:::your-bucket/*", "arn:aws:s3:::your-bucket"]
    }]
  }'

# Associate IAM OIDC provider with cluster
eksctl utils associate-iam-oidc-provider \
  --cluster tour-management-cluster \
  --approve

# Create service account with IRSA
eksctl create iamserviceaccount \
  --name tour-management-sa \
  --namespace tour-management \
  --cluster tour-management-cluster \
  --attach-policy-arn arn:aws:iam::<account-id>:policy/TourManagementS3Policy \
  --approve
```

---

## .NET Framework Specific Notes

- **Windows Containers Required**: .NET Framework 4.7.2 applications require Windows containers. Linux containers are not supported.
- **IIS Hosting**: The application is hosted by IIS inside the container. The `ServiceMonitor.exe` entrypoint keeps the container alive by monitoring the `w3svc` Windows service.
- **Application Pool**: Configured for .NET 4.0 CLR with Integrated Pipeline mode.
- **Startup Time**: Windows containers have longer startup times (~60-90 seconds). The liveness and readiness probes are configured with appropriate `initialDelaySeconds` values.
- **Image Size**: Windows container images are significantly larger than Linux images (several GB). Plan ECR storage and pull times accordingly.
- **Node Selector**: The deployment includes `kubernetes.io/os: windows` node selector to ensure pods are scheduled on Windows nodes only.
- **NuGet Packages**: AWSSDK.Core and AWSSDK.S3 are used for S3 image uploads. Configure IRSA for production use instead of static credentials.
