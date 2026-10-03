# Tour Management System - .NET 8

A modern Tour Management web application built with ASP.NET Core 8 Razor Pages, following Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with four layers:

- **TourManagement.Domain** - Domain entities, interfaces, and exceptions
- **TourManagement.Application** - Business logic, services, DTOs, validators, AutoMapper profiles
- **TourManagement.Infrastructure** - EF Core DbContext, repositories, data configurations
- **TourManagement.Web** - ASP.NET Core 8 Razor Pages UI, ViewModels, Program.cs

## Features

- **User Management**: Registration, login, profile management
- **Tour Management**: Browse, search, create, edit, delete tours (admin)
- **Booking System**: Book tours, view/cancel bookings
- **Admin Dashboard**: Manage tours, users, and bookings
- **Session-based Authentication**: User and admin sessions

## Technology Stack

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- AutoMapper 12.0
- FluentValidation 11.9
- Serilog 8.0
- Bootstrap 5.3
- xUnit + Moq + FluentAssertions (testing)

## Setup Instructions

1. **Prerequisites**: .NET 8 SDK, SQL Server (or LocalDB)

2. **Configure Connection String** in `src/TourManagement.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=tourdb;Trusted_Connection=True;"
   }
   ```

3. **Run Database Migrations**:
   ```bash
   cd src/TourManagement.Web
   dotnet ef migrations add InitialCreate --project ../TourManagement.Infrastructure
   dotnet ef database update --project ../TourManagement.Infrastructure
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

5. **Run Tests**:
   ```bash
   dotnet test
   ```

## Default Admin Credentials

- Email: `admin@gmail.com`
- Password: `admin`

## Migration Notes

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8.
See `docs/MIGRATION_NOTES.md` for details.
