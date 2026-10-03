# ============================================================
# Dockerfile for Tour_Management ASP.NET Web Forms Application
# Framework: .NET Framework 4.7.2
# Base Runtime Image: mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
# Target Platform: AWS EKS (Windows node groups)
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR /src

# Copy solution/project files first for layer caching
COPY ["Tour_Management/Tour_Management.csproj", "Tour_Management/"]
COPY ["Tour_Management/packages.config", "Tour_Management/"]

# Restore NuGet packages
RUN nuget restore Tour_Management/Tour_Management.csproj -PackagesDirectory packages

# Copy the rest of the application source
COPY . .

# Build and publish the Web Application in Release mode
RUN msbuild Tour_Management/Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:platform=AnyCPU \
    /p:VisualStudioVersion=16.0

# Remove PDB files, XML doc files, and temp artifacts from publish output
RUN powershell -Command \
    Write-Host 'Cleaning PDB and temp artifacts from publish output...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include '*.pdb' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\publish -Recurse -Include '*.xml' | Where-Object { $_.DirectoryName -like '*bin*' } | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\publish -Recurse -Include '*.tmp' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\publish -Recurse -Include '*.cs' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\publish -Recurse -Include '*.csproj' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Write-Host 'Cleanup complete.'

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime

# Set working directory to IIS default web root
WORKDIR /inetpub/wwwroot

# Remove default IIS welcome page
RUN powershell -Command \
    Remove-Item -Recurse -Force C:\inetpub\wwwroot\* -ErrorAction SilentlyContinue

# Copy ONLY the published application from build stage
COPY --from=build C:\publish .

# Final cleanup pass
RUN powershell -Command \
    Write-Host 'Final runtime image cleanup...'; \
    Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include '*.pdb' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include '*.cs' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include '*.tmp' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include '*.log' | Remove-Item -Force -ErrorAction SilentlyContinue; \
    Write-Host 'Runtime image cleanup complete.'

# Configure IIS application pool
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedPipelineMode -Value 'Integrated'; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# Environment Variables (override at runtime via EKS ConfigMap / Secrets)
ENV DB_CONNECTION_STRING="" \
    DB_HOST="" \
    DB_PORT="1433" \
    DB_NAME="" \
    DB_USER="" \
    DB_PASSWORD="" \
    AWS_REGION="" \
    S3_BUCKET_NAME="" \
    ASPNET_ENVIRONMENT="Production"

# Expose HTTP port
EXPOSE 80

# IIS is managed by the base image's ServiceMonitor entrypoint
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
