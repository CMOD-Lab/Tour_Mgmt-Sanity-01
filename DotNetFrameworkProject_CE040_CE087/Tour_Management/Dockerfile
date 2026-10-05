# ============================================================
# Dockerfile for Tour_Management ASP.NET Web Forms Application
# Framework: .NET Framework 4.7.2 (Windows Containers)
# Build Stage: mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019
# Runtime Stage: mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
# Target: AWS EKS Windows node groups
# Health Endpoint: /health.ashx
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR C:\\build

# Copy project and package manifest files first for NuGet layer caching
COPY Tour_Management.csproj .
COPY packages.config .

# Restore NuGet packages (layer cached until project/packages files change)
RUN nuget restore Tour_Management.csproj -PackagesDirectory ..\packages

# Copy the rest of the application source
COPY . .

# Build and publish the web application in Release mode
# /p:DebugSymbols=false and /p:DebugType=none ensure no PDB files are emitted
RUN msbuild Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\\publish \
    /p:DeleteExistingFiles=True \
    /p:platform=AnyCPU \
    /p:VisualStudioVersion=15.0 \
    /p:DebugSymbols=false \
    /p:DebugType=none

# ---- Cleanup Step: Remove PDB files, temp files, and source artifacts ----
RUN powershell -Command \
    Write-Host 'Removing PDB files from publish output...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.pdb | Remove-Item -Force; \
    Write-Host 'Removing XML documentation files...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.xml | Where-Object { $_.Name -notlike 'Web.config' } | Remove-Item -Force; \
    Write-Host 'Removing temp and cache files...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.tmp,*.cache,*.log,*.bak | Remove-Item -Force; \
    Write-Host 'Removing roslyn compiler binaries (not needed at runtime)...'; \
    if (Test-Path C:\publish\bin\roslyn) { Remove-Item -Recurse -Force C:\publish\bin\roslyn }; \
    Write-Host 'Cleanup complete.'

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime

# Set working directory to IIS default web root
WORKDIR C:\\inetpub\\wwwroot

# Remove default IIS welcome page
RUN powershell -Command Remove-Item -Recurse -Force C:\inetpub\wwwroot\*

# Copy ONLY the published application from build stage (no source code, no PDB files)
COPY --from=build C:\\publish .

# ---- IIS Site Configuration ----
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedPipelineMode -Value Integrated; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# ---- Environment Variables ----
ENV DB_HOST="" \
    DB_PORT="1433" \
    DB_NAME="" \
    DB_USER="" \
    DB_PASSWORD="" \
    AWS_REGION="" \
    S3_BUCKET_NAME="" \
    ASPNET_ENV="Production"

# ---- Expose Port ----
EXPOSE 80
