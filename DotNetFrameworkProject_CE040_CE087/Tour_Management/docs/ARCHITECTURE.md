# Architecture Documentation

## Clean Architecture Overview

The Tour Management application follows Clean Architecture principles with four distinct layers:

```
┌─────────────────────────────────────────────────────────┐
│                    Web Layer (UI)                        │
│         TourManagement.Web (Razor Pages)                │
├─────────────────────────────────────────────────────────┤
│                 Application Layer                        │
│    TourManagement.Application (Services, DTOs)          │
├─────────────────────────────────────────────────────────┤
│                Infrastructure Layer                      │
│  TourManagement.Infrastructure (EF Core, Repositories)  │
├─────────────────────────────────────────────────────────┤
│                   Domain Layer                           │
│      TourManagement.Domain (Entities, Interfaces)       │
└─────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain Layer
- Domain entities (Tour, User, Booking)
- Repository interfaces (ITourRepository, IUserRepository, IBookingRepository)
- Service interfaces (ITourService, IUserService, IBookingService)
- Domain exceptions (NotFoundException)
- No external dependencies

### Application Layer
- Service implementations (TourService, UserService, BookingService)
- Data Transfer Objects (DTOs)
- AutoMapper profiles
- FluentValidation validators
- Depends on: Domain

### Infrastructure Layer
- EF Core DbContext (TourManagementDbContext)
- Repository implementations
- Entity configurations
- Depends on: Domain, Application

### Web Layer
- Razor Pages (Index, Create, Details, Edit, Delete for each entity)
- ViewModels (manually mapped from/to DTOs)
- Program.cs (DI configuration, middleware)
- Static files (CSS, JS)
- Depends on: Application, Infrastructure

## Dependency Flow

```
Web → Application → Domain ← Infrastructure
```

## Key Design Decisions

1. **Manual ViewModel Mapping**: ViewModels in the Web layer are manually mapped to/from DTOs (no AutoMapper in Web layer)
2. **Soft Delete**: Entities are soft-deleted (IsActive = false) rather than hard-deleted
3. **Session Authentication**: Simple session-based auth (suitable for migration; upgrade to Identity for production)
4. **Async/Await**: All I/O operations are async throughout all layers
5. **Dependency Injection**: All services registered via extension methods
