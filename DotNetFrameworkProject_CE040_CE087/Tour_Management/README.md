# Tour Management System - .NET 8 Migration

## Overview
This is a Tour Management System migrated from ASP.NET Web Forms (.NET 4.7.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities, interfaces, exceptions
│   ├── TourManagement.Application/     # Business logic services, validators
│   ├── TourManagement.Infrastructure/  # EF Core, repositories, data access
│   └── TourManagement.Web/             # Razor Pages UI, ViewModels
├── tests/
│   ├── TourManagement.UnitTests/       # Unit tests for services
│   └── TourManagement.IntegrationTests/ # Integration tests for repositories
└── docs/                               # Documentation
```

## Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB for development)

## Setup
1. Clone the repository
2. Update connection string in `src/TourManagement.Web/appsettings.json`
3. Run `dotnet restore`
4. Run `dotnet build`
5. Navigate to `src/TourManagement.Web` and run `dotnet run`

## Default Admin Credentials
- Email: `admin@gmail.com`
- Password: `admin`

## Features
- Tour management (CRUD)
- User registration and login
- Tour booking system
- Admin dashboard
- Session-based authentication

## Build
```bash
dotnet build TourManagement.sln
```

## Test
```bash
dotnet test TourManagement.sln
```
