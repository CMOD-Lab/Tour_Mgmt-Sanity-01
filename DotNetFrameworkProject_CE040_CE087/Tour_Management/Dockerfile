# ============================================================
# Dockerfile for Tour_Management ASP.NET Web Forms Application
# Builder Image : mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019
# Runtime Image : mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
# Target        : Windows Container on AWS EKS (Windows node groups)
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
WORKDIR /src

# Copy project and package manifest files first for NuGet layer caching
COPY ["DotNetFrameworkProject_CE040_CE087/Tour_Management/Tour_Management.csproj", "Tour_Management/"]
COPY ["DotNetFrameworkProject_CE040_CE087/Tour_Management/packages.config", "Tour_Management/"]

# Restore NuGet packages
WORKDIR /src/Tour_Management
RUN nuget restore Tour_Management.csproj -PackagesDirectory ..\packages

# Copy the rest of the application source
WORKDIR /src
COPY ["DotNetFrameworkProject_CE040_CE087/Tour_Management/", "Tour_Management/"]

# Build and publish the web application in Release mode (no PDB output)
WORKDIR /src/Tour_Management
RUN msbuild Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:platform=AnyCPU \
    /p:VisualStudioVersion=15.0 \
    /p:DebugSymbols=false \
    /p:DebugType=None

# Remove PDB files, source code, and temp artifacts from publish output
RUN powershell -Command \
    Get-ChildItem -Path C:\publish -Recurse -Include *.pdb,*.mdb,*.cs,*.vb,*.tmp,*.temp,*.cache,*.log | Remove-Item -Force -ErrorAction SilentlyContinue; \
    if (Test-Path C:\publish\obj) { Remove-Item -Recurse -Force C:\publish\obj }; \
    if (Test-Path C:\publish\App_Data) { Remove-Item -Recurse -Force C:\publish\App_Data }

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime

# Set working directory to IIS default web root
WORKDIR /inetpub/wwwroot

# Remove default IIS welcome page
RUN powershell -Command Remove-Item -Recurse -Force C:\inetpub\wwwroot\*

# Copy ONLY the published application artifacts from build stage (no source/PDB files)
COPY --from=build C:\publish .

# Final cleanup — remove any residual PDB, source, or temp files
RUN powershell -Command \
    Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include *.pdb,*.mdb,*.cs,*.vb,*.tmp,*.temp,*.cache,*.log | Remove-Item -Force -ErrorAction SilentlyContinue; \
    if (Test-Path C:\inetpub\wwwroot\obj) { Remove-Item -Recurse -Force C:\inetpub\wwwroot\obj }

# Configure IIS application pool for .NET 4.0 CLR
RUN powershell -NoProfile -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool managedRuntimeVersion v4.0; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool enable32BitAppOnWin64 False; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool managedPipelineMode Integrated; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' physicalPath C:\inetpub\wwwroot

# Expose HTTP port
EXPOSE 80

# Environment variables — override at runtime via EKS ConfigMap / Secrets
ENV ASPNET_ENVIRONMENT=Production \
    DB_HOST="" \
    DB_PORT="1433" \
    DB_NAME="" \
    DB_USER="" \
    DB_PASSWORD="" \
    AWS_REGION="us-east-1" \
    S3_BUCKET_NAME=""

# Start IIS in the foreground (keeps the container alive)
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
