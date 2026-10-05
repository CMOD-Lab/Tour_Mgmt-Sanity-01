# =============================================================================
# Stage 1: Build
# Uses the full .NET Framework SDK image to compile the ASP.NET Web Forms app
# =============================================================================
FROM mcr.microsoft.com/dotnet/framework/sdk:4.7.2 AS build

WORKDIR /src

# Copy NuGet packages config first for layer caching
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/packages.config ./Tour_Management/packages.config

# Restore NuGet packages
RUN nuget restore Tour_Management/packages.config -PackagesDirectory packages

# Copy the full project source
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/ ./Tour_Management/
COPY DotNetFrameworkProject_CE040_CE087/packages/ ./packages/

# Build and publish the web application
RUN msbuild Tour_Management/Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:PrecompileBeforePublish=true \
    /p:EnableUpdateable=false

# =============================================================================
# Stage 2: Runtime
# Uses the explicit base image: mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
# Hosts the application under IIS
# =============================================================================
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime

WORKDIR /inetpub/wwwroot

# Copy published output from build stage
COPY --from=build C:\publish .

# Set environment variable for ASP.NET configuration
ENV ASPNET_ENV=Production

# Expose HTTP port (IIS default)
EXPOSE 80

# IIS is started automatically by the base image entrypoint
