# Architecture Documentation

## Clean Architecture Overview

The Tour Management System follows Clean Architecture principles with four distinct layers:

```
Tour_Management.sln
├── src/
│   ├── Tour_Management.Domain          # Core business entities and interfaces
│   ├── Tour_Management.Application     # Business logic and services
│   ├── Tour_Management.Infrastructure  # Data access and EF Core
│   └── Tour_Management.Web             # Razor Pages UI
└── tests/
    └── Tour_Management.UnitTests       # Unit tests
```

## Layer Responsibilities

### Domain Layer
- Domain entities (Tour, Booking, UserInfo)
- Repository interfaces (ITourRepository, IBookingRepository, IUserRepository)
- Service interfaces (ITourService, IBookingService, IUserService)
- Domain exceptions (NotFoundException)

### Application Layer
- Service implementations (TourService, BookingService, UserService)
- AutoMapper profiles
- DI extension methods

### Infrastructure Layer
- EF Core DbContext (TourManagementDbContext)
- Entity configurations (Fluent API)
- Repository implementations
- DI extension methods

### Web Layer
- Razor Pages (UI)
- ViewModels (manually mapped from/to DTOs)
- Program.cs (application startup)
- Static files (CSS, JS)

## Dependency Flow
```
Web → Application → Domain
Web → Infrastructure → Domain
Infrastructure → Application
```

## Key Patterns Used
- Repository Pattern
- Service Layer Pattern
- Dependency Injection
- Clean Architecture
- Async/Await throughout
- Manual ViewModel mapping in Web layer
