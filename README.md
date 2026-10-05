# Tour Management System - .NET 8 Migration

## Overview
This is a Tour Management System migrated from ASP.NET Web Forms (.NET 4.7.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities, interfaces, exceptions
│   ├── TourManagement.Application/     # Business logic, services, DTOs, mappings
│   ├── TourManagement.Infrastructure/  # EF Core, repositories, data access
│   └── TourManagement.Web/             # Razor Pages UI, ViewModels, Program.cs
└── tests/
    └── TourManagement.UnitTests/       # xUnit unit tests
```

## Features
- **User Management**: Registration, login, profile management
- **Tour Management**: CRUD operations for tour packages with image upload
- **Booking System**: Book tours, view personal bookings
- **Admin Dashboard**: Manage all tours, bookings, and users

## Technology Stack
- **.NET 8** with ASP.NET Core Razor Pages
- **Entity Framework Core 8.0** for data access
- **AutoMapper 13.0** for object mapping
- **BCrypt.Net** for password hashing
- **Serilog** for structured logging
- **Cookie Authentication** for user sessions
- **Bootstrap 5** for UI

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Configuration
1. Update the connection string in `src/TourManagement.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=TourManagementDb;..."
   }
   ```

2. Run database migrations:
   ```bash
   cd src/TourManagement.Web
   dotnet ef database update
   ```

### Running the Application
```bash
cd src/TourManagement.Web
dotnet run
```

### Running Tests
```bash
dotnet test tests/TourManagement.UnitTests/
```

## Build
```bash
dotnet build TourManagement.sln
```

## Migration Notes
- Migrated from ASP.NET Web Forms to Razor Pages
- Replaced ADO.NET with Entity Framework Core 8
- Replaced Forms Authentication with Cookie Authentication
- Replaced Web.config with appsettings.json
- Replaced System.Web with ASP.NET Core equivalents
- Added BCrypt password hashing (plain text passwords in original)
- Added proper async/await patterns throughout
- Added dependency injection
- Added structured logging with Serilog
