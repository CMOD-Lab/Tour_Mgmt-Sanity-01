# Tour_Management — Deployment Guide

## Overview

This guide covers containerizing and deploying the **Tour_Management** ASP.NET Web Forms application (.NET Framework 4.7.2) to **AWS EKS** using Windows containers.

---

## Technology Stack

| Component | Details |
|-----------|---------|
| Framework | .NET Framework 4.7.2 |
| Application Type | ASP.NET Web Forms (IIS-hosted) |
| Container Type | **Windows Container** |
| Builder Image | `mcr.microsoft.com/dotnet/framework/sdk:4.7.2` |
| Runtime Image | `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2` |
| Application Port | 80 (HTTP via IIS) |
| Health Endpoint | `/health.ashx` |
| Database | SQL Server (via `CONNECTION_STRING` env var) |
| Cloud Storage | AWS S3 (via `S3_BUCKET_NAME` env var) |

---

## Prerequisites

### Local Development
- Docker Desktop with **Windows containers** enabled
- .NET Framework 4.7.2 SDK (for local builds)
- Visual Studio 2019/2022 (optional)

### AWS EKS Deployment
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version --client`)
- eksctl (optional, for cluster creation)
- An EKS cluster with a **Windows node group**
- IAM permissions: `ecr:*`, `eks:*`

---

## Project Structure

```
Tour_Management/
├── Dockerfile                  # Multi-stage Windows container build
├── docker-compose.yml          # Local development compose file
├── .dockerignore               # Docker build exclusions
├── kubernetes/
│   ├── namespace.yaml          # Kubernetes namespace
│   ├── deployment.yaml         # Deployment with health probes
│   ├── service.yaml            # ClusterIP service
│   └── ingress.yaml            # AWS ALB ingress
├── scripts/
│   ├── build-push.sh           # Linux/macOS build & push script
│   ├── build-push.bat          # Windows build & push script
│   ├── deploy-image.sh         # Linux/macOS EKS deploy script
│   └── deploy-image.bat        # Windows EKS deploy script
└── docs/
    └── DEPLOYMENT.md           # This file
```

---

## Local Development with Docker Compose

### 1. Configure Environment Variables

Create a `.env` file in the `Tour_Management/` directory:

```env
CONNECTION_STRING=Data Source=<your-sql-server>;Initial Catalog=tourdb;User ID=sa;Password=YourStrong@Passw0rd
S3_BUCKET_NAME=your-s3-bucket-name
AWS_ACCESS_KEY_ID=your-access-key
AWS_SECRET_ACCESS_KEY=your-secret-key
AWS_REGION=us-east-1
```

> **Note:** Never commit `.env` files to source control.

### 2. Build and Start the Application

```bash
# From the repository root
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml up --build
```

### 3. Access the Application

- Application: [http://localhost:80](http://localhost:80)
- Health Check: [http://localhost:80/health.ashx](http://localhost:80/health.ashx)

### 4. Stop the Application

```bash
docker-compose -f DotNetFrameworkProject_CE040_CE087/Tour_Management/docker-compose.yml down
```

---

## Building and Pushing the Docker Image

> **Important:** Windows containers must be built on a Windows host or a Windows-based CI/CD agent.

### Linux/macOS

```bash
chmod +x DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/build-push.sh
./DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/build-push.sh
```

### Windows

```cmd
DotNetFrameworkProject_CE040_CE087\Tour_Management\scripts\build-push.bat
```

The script will prompt you to:
1. Enter an image tag (default: `latest`)
2. Select a registry (AWS ECR or Docker Hub)
3. Provide registry credentials

### Manual Build

```bash
# From repository root
docker build \
  -f DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile \
  -t <registry>/<repo>:<tag> \
  .
```

---

## AWS EKS Deployment

### Step 1: Create an EKS Cluster with Windows Node Group

```bash
# Create cluster (if not already existing)
eksctl create cluster \
  --name my-cluster \
  --region us-east-1 \
  --nodegroup-name linux-nodes \
  --node-type t3.medium \
  --nodes 2

# Add Windows node group
eksctl create nodegroup \
  --cluster my-cluster \
  --region us-east-1 \
  --name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --node-ami-family WindowsServer2019FullContainer
```

> **Critical:** Windows containers require Windows nodes. The `deployment.yaml` includes `nodeSelector: kubernetes.io/os: windows`.

### Step 2: Install AWS Load Balancer Controller

```bash
# Add IAM policy for ALB controller
curl -o iam_policy.json https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/v2.6.0/docs/install/iam_policy.json

aws iam create-policy \
  --policy-name AWSLoadBalancerControllerIAMPolicy \
  --policy-document file://iam_policy.json

# Install via Helm
helm repo add eks https://aws.github.io/eks-charts
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=my-cluster \
  --set serviceAccount.create=true
```

### Step 3: Build and Push the Image

```bash
./DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/build-push.sh
# Note the full image URI output (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest)
```

### Step 4: Deploy to EKS

```bash
chmod +x DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/deploy-image.sh
./DotNetFrameworkProject_CE040_CE087/Tour_Management/scripts/deploy-image.sh
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Docker Image URI
- SQL Server connection string
- S3 bucket name and AWS credentials

### Step 5: Verify Deployment

```bash
# Check pods
kubectl get pods -n tour-management

# Check service
kubectl get svc -n tour-management

# Check ingress (wait for ALB to provision)
kubectl get ingress -n tour-management

# View pod logs
kubectl logs -l app=tour-management -n tour-management --tail=50

# Describe deployment
kubectl describe deployment tour-management -n tour-management
```

---

## Kubernetes Manifest Reference

### namespace.yaml
Creates the `tour-management` namespace to isolate all resources.

### deployment.yaml
- **Replicas:** 2 (for high availability)
- **Image:** `{{IMAGE_URI}}` — replaced by deploy script
- **Node Selector:** `kubernetes.io/os: windows` — requires Windows nodes
- **Liveness Probe:** HTTP GET `/health.ashx` on port 80 (initial delay: 60s)
- **Readiness Probe:** HTTP GET `/health.ashx` on port 80 (initial delay: 45s)
- **Resources:** requests: 250m CPU / 512Mi RAM; limits: 500m CPU / 1Gi RAM

### service.yaml
- **Type:** ClusterIP — internal cluster access only
- **Port:** 80 → 80

### ingress.yaml
- **Class:** AWS ALB (internet-facing)
- **Health Check Path:** `/health.ashx`
- **Host:** `tour-management.example.com` — update to your actual domain

---

## Configuration Management

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `CONNECTION_STRING` | SQL Server connection string | Yes |
| `S3_BUCKET_NAME` | AWS S3 bucket for file storage | No |
| `AWS_REGION` | AWS region for S3 | No |
| `AWS_ACCESS_KEY_ID` | AWS access key (prefer IRSA) | No |
| `AWS_SECRET_ACCESS_KEY` | AWS secret key (prefer IRSA) | No |
| `ASPNET_ENV` | ASP.NET environment name | No (default: Production) |

### Using Kubernetes Secrets (Recommended)

```bash
kubectl create secret generic tour-management-secrets \
  --from-literal=CONNECTION_STRING="Data Source=rds-endpoint;..." \
  --from-literal=AWS_ACCESS_KEY_ID="AKIA..." \
  --from-literal=AWS_SECRET_ACCESS_KEY="..." \
  -n tour-management
```

Then reference in `deployment.yaml`:
```yaml
env:
  - name: CONNECTION_STRING
    valueFrom:
      secretKeyRef:
        name: tour-management-secrets
        key: CONNECTION_STRING
```

### Using AWS IAM Roles for Service Accounts (IRSA) — Recommended for S3

```bash
# Associate OIDC provider
eksctl utils associate-iam-oidc-provider --cluster my-cluster --approve

# Create IAM role with S3 access
eksctl create iamserviceaccount \
  --name tour-management-sa \
  --namespace tour-management \
  --cluster my-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/AmazonS3FullAccess \
  --approve
```

---

## Scaling and Management

### Horizontal Pod Autoscaler

```bash
kubectl autoscale deployment tour-management \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n tour-management
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
kubectl rollout undo deployment/tour-management -n tour-management
```

---

## Troubleshooting

### Pod Not Starting

```bash
# Check pod events
kubectl describe pod -l app=tour-management -n tour-management

# Check logs
kubectl logs -l app=tour-management -n tour-management --previous
```

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| `ImagePullBackOff` | Wrong image URI or missing ECR permissions | Verify image URI; check IAM permissions |
| `CrashLoopBackOff` | Application startup failure | Check logs; verify `CONNECTION_STRING` |
| `Pending` pods | No Windows nodes available | Add Windows node group to EKS cluster |
| Health check failing | IIS not started or DB unreachable | Check `/health.ashx` response; verify DB connectivity |
| ALB not provisioned | ALB controller not installed | Install AWS Load Balancer Controller |

### Windows Container Specific

```bash
# Verify Windows nodes are available
kubectl get nodes -l kubernetes.io/os=windows

# Check node capacity
kubectl describe node <windows-node-name>
```

### Database Connectivity

The application uses SQL Server. Ensure:
1. The SQL Server is accessible from the EKS VPC
2. Security groups allow port 1433 from the EKS node security group
3. The `CONNECTION_STRING` uses the RDS endpoint (not LocalDB)

Example production connection string:
```
Data Source=my-rds-instance.xxxx.us-east-1.rds.amazonaws.com;Initial Catalog=tourdb;User ID=touruser;Password=SecurePassword123
```

---

## Security Considerations

1. **Never use LocalDB in production** — use Amazon RDS for SQL Server
2. **Use Kubernetes Secrets** for sensitive values (connection strings, API keys)
3. **Use IRSA** instead of static AWS credentials for S3 access
4. **Restrict ingress** — configure security groups to limit ALB access
5. **Enable HTTPS** — configure ACM certificate on the ALB ingress
6. **Scan images** — use Amazon ECR image scanning for vulnerability detection
7. **Network policies** — restrict pod-to-pod communication as needed

### Enable HTTPS on ALB

Add to `ingress.yaml` annotations:
```yaml
alb.ingress.kubernetes.io/listen-ports: '[{"HTTP": 80}, {"HTTPS": 443}]'
alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789:certificate/xxx
alb.ingress.kubernetes.io/ssl-redirect: '443'
```

---

## .NET Framework Specific Notes

- **Windows containers only**: .NET Framework 4.7.2 requires Windows Server containers
- **IIS hosting**: The application is hosted by IIS inside the container (managed by the base image)
- **Build environment**: The Docker build must run on a Windows host or Windows CI/CD agent
- **Image size**: Windows container images are significantly larger than Linux images (~8-10 GB)
- **Startup time**: Allow 60+ seconds for IIS to initialize (reflected in `initialDelaySeconds`)
- **Web.config transforms**: The `Web.Release.config` transform removes debug compilation in Release builds

---

*Generated for Tour_Management — .NET Framework 4.7.2 ASP.NET Web Forms — AWS EKS*
