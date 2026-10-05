# Tour Management System - .NET 8 Migration

## Overview
This is a Tour Management System migrated from ASP.NET Web Forms (.NET 4.7.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

- **Tour_Management.Domain** - Domain entities, interfaces, and exceptions
- **Tour_Management.Application** - Business logic services and AutoMapper profiles
- **Tour_Management.Infrastructure** - EF Core data access, repositories
- **Tour_Management.Web** - ASP.NET Core Razor Pages UI

## Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

## Setup Instructions

1. Clone the repository
2. Update the connection string in `src/Tour_Management.Web/appsettings.json`
3. Run database migrations:
   ```bash
   cd src/Tour_Management.Web
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run --project src/Tour_Management.Web
   ```

## Default Admin Credentials
- Email: `admin@gmail.com`
- Password: `admin`

## Features
- User registration and login
- Admin login and management
- Tour browsing and searching
- Tour booking management
- Admin: Add/Edit/Delete tours
- Admin: View all bookings
- Admin: Manage users

## Running Tests
```bash
dotnet test tests/Tour_Management.UnitTests
```

## Migration Notes
See `docs/MIGRATION_NOTES.md` for details on what was migrated from Web Forms.
