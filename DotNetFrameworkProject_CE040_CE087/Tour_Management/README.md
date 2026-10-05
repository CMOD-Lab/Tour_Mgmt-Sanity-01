# Tour Management System - .NET 8

A modern Tour Management web application built with ASP.NET Core 8 Razor Pages, following Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with four main layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities, interfaces, exceptions
│   ├── TourManagement.Application/     # Business logic, services, DTOs, validators
│   ├── TourManagement.Infrastructure/  # EF Core, repositories, data access
│   └── TourManagement.Web/             # Razor Pages UI, ViewModels, Program.cs
├── tests/
│   ├── TourManagement.UnitTests/       # Unit tests for services
│   └── TourManagement.IntegrationTests/ # Integration tests for repositories
└── docs/
    ├── ARCHITECTURE.md
    ├── MIGRATION_NOTES.md
    └── BUILD_VERIFICATION.md
```

## Features

- **Tour Management**: Full CRUD for tour packages (Index, Create, Details, Edit, Delete)
- **User Management**: Registration, login, profile management
- **Booking System**: Book tours, view/manage bookings
- **Admin Panel**: Admin login, dashboard, user management
- **Image Upload**: Tour picture upload support

## Technology Stack

- **Framework**: ASP.NET Core 8 Razor Pages
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server (LocalDB for development)
- **Mapping**: AutoMapper 12.0.1
- **Validation**: FluentValidation 11.9.0
- **Logging**: Serilog 8.0.0
- **Testing**: xUnit, Moq, FluentAssertions

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server or LocalDB

### Running the Application

1. Clone/navigate to the project directory
2. Update the connection string in `src/TourManagement.Web/appsettings.json`
3. Run database migrations:
   ```bash
   cd src/TourManagement.Web
   dotnet ef database update --project ../TourManagement.Infrastructure
   ```
4. Run the application:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

### Running Tests

```bash
dotnet test
```

## Migration from ASP.NET Web Forms 4.7.2

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8. See `docs/MIGRATION_NOTES.md` for details.

## Default Admin Credentials

- Email: `admin@gmail.com`
- Password: `admin`

> **Note**: Change these credentials in `appsettings.json` before deploying to production.
