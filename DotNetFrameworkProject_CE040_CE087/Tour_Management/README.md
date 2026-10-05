# Tour Management System - .NET 8

A modern Tour Management web application built with ASP.NET Core 8 Razor Pages, following Clean Architecture principles.

## Architecture

This application follows Clean Architecture with four layers:

- **Domain** (`src/Tour_Management.Domain`) - Entities, interfaces, domain exceptions
- **Application** (`src/Tour_Management.Application`) - Business logic, services, DTOs, AutoMapper profiles
- **Infrastructure** (`src/Tour_Management.Infrastructure`) - EF Core DbContext, repositories, data configurations
- **Web** (`src/Tour_Management.Web`) - Razor Pages, ViewModels, static files

## Features

- **Tour Management**: Full CRUD operations for tours (Admin only)
- **User Registration & Login**: Secure user authentication with password hashing
- **Booking System**: Users can book tours and view their bookings
- **Admin Dashboard**: Admin can manage tours, users, and view all bookings
- **Search**: Search tours by name, place, or locations

## Technology Stack

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0 (with InMemory database for development)
- AutoMapper 12.0
- FluentValidation 11.9
- Serilog for logging
- Bootstrap 5 for UI

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (optional - InMemory database used by default)

### Running the Application

```bash
cd src/Tour_Management.Web
dotnet run
```

### With SQL Server

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TourManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Admin Login
- Email: `admin@gmail.com`
- Password: `admin`

## Running Tests

```bash
dotnet test tests/Tour_Management.UnitTests/
```

## Migration Notes

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8.
See `docs/MIGRATION_NOTES.md` for details.
