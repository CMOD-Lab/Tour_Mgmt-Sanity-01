# ============================================================
# Dockerfile for Tour_Management ASP.NET Web Forms Application
# Technology: ASP.NET Web Forms on .NET Framework 4.7.2
# Base Runtime Image: mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
# Target Platform: AWS EKS (Windows node groups)
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
WORKDIR /src

# Copy solution and project files first for NuGet layer caching
COPY ["Tour_Management/Tour_Management.csproj", "Tour_Management/"]
COPY ["Tour_Management/packages.config", "Tour_Management/"]

# Restore NuGet packages
RUN nuget restore Tour_Management/Tour_Management.csproj -PackagesDirectory ../packages

# Copy the full source code
COPY . .

# Build and publish the Web Application in Release mode
RUN msbuild Tour_Management/Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:platform=AnyCPU \
    /p:VisualStudioVersion=15.0 \
    /p:DebugSymbols=false \
    /p:DebugType=none

# Remove PDB files, source code, and temp artifacts from publish output
RUN powershell -Command \
    Get-ChildItem -Path C:\publish -Recurse -Include *.pdb | Remove-Item -Force; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.cs | Remove-Item -Force; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.vb | Remove-Item -Force; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.csproj | Remove-Item -Force; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.sln | Remove-Item -Force; \
    if (Test-Path C:\publish\obj) { Remove-Item -Recurse -Force C:\publish\obj }; \
    if (Test-Path C:\publish\App_Data\*.mdf) { Remove-Item -Force C:\publish\App_Data\*.mdf }; \
    if (Test-Path C:\publish\App_Data\*.ldf) { Remove-Item -Force C:\publish\App_Data\*.ldf }

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 AS runtime

# Set working directory to IIS default web root
WORKDIR /inetpub/wwwroot

# Remove default IIS welcome page
RUN powershell -Command Remove-Item -Recurse -Force C:\inetpub\wwwroot\*

# Copy only the published application artifacts from build stage
COPY --from=build C:\publish .

# Configure IIS application pool for .NET Framework 4.7.2
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# Expose HTTP port
EXPOSE 80

# Environment variables for application configuration
# Override these at runtime via EKS pod spec / Kubernetes secrets
ENV ASPNET_ENV="Production" \
    DB_CONNECTION_STRING="" \
    DB_HOST="" \
    DB_PORT="1433" \
    DB_NAME="" \
    DB_USER="" \
    DB_PASSWORD="" \
    AWS_REGION="" \
    AWS_S3_BUCKET="" \
    APP_PORT="80"

# IIS is managed by the base image entrypoint (ServiceMonitor.exe)
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
