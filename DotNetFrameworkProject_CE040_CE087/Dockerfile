# ============================================================
# Stage 1: Build
# Uses the .NET Framework SDK image to compile the ASP.NET
# Web Forms application (Tour_Management, targeting v4.7.2).
# ============================================================
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR /src

# Copy solution and project files first to restore NuGet packages
COPY Tour_Management.sln .
COPY Tour_Management/Tour_Management.csproj Tour_Management/
COPY Tour_Management/packages.config Tour_Management/

# Restore NuGet packages
RUN nuget restore Tour_Management.sln

# Copy the rest of the source code
COPY . .

# Build and publish the web application to /app
RUN msbuild Tour_Management.sln \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\app \
    /p:DeleteExistingFiles=True \
    /p:platform=AnyCPU \
    /p:VisualStudioVersion=15.0

# ============================================================
# Stage 2: Runtime
# Uses the ASP.NET 4.8 Windows Server Core image with IIS
# pre-configured for hosting ASP.NET Web Forms applications.
# ============================================================
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS runtime

# Set the working directory to the IIS default web root
WORKDIR /inetpub/wwwroot

# Remove the default IIS welcome page
RUN powershell -Command Remove-Item -Recurse -Force C:\inetpub\wwwroot\*

# Copy the published application from the build stage
COPY --from=build /app .

# -------------------------------------------------------
# IIS Site Configuration
# -------------------------------------------------------
# Configure the Default Web Site application pool to use
# .NET CLR v4.0 and set the application pool identity.
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' managedRuntimeVersion v4.0; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' enable32BitAppOnWin64 False; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value C:\inetpub\wwwroot

# -------------------------------------------------------
# Environment Variables
# Applications should read connection strings and settings
# from environment variables injected at runtime (EKS).
# -------------------------------------------------------
# Database connection (override at runtime via EKS ConfigMap/Secret)
ENV DB_HOST=""
ENV DB_PORT="1433"
ENV DB_NAME=""
ENV DB_USER=""
ENV DB_PASSWORD=""

# AWS S3 settings
ENV S3_BUCKET_NAME=""
ENV AWS_REGION=""

# Application settings
ENV APP_ENV="production"

# Expose HTTP port (IIS listens on 80 by default)
EXPOSE 80

# Health check – uses the existing health.ashx handler
# Kubernetes liveness/readiness probes can target GET /health.ashx
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD powershell -Command \
        try { \
            $response = Invoke-WebRequest -Uri http://localhost/health.ashx -UseBasicParsing -TimeoutSec 5; \
            if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } \
        } catch { exit 1 }
