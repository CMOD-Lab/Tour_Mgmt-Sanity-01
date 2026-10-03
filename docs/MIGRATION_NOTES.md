# Migration Notes

## Overview

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8 with Clean Architecture.

## What Was Migrated

### Pages Migrated (Web Forms → Razor Pages)
| Web Forms Page | Razor Page |
|---|---|
| userlogin.aspx | Pages/User/Login.cshtml |
| SignUpForm.aspx | Pages/User/Register.cshtml |
| MainProfilePage.aspx | Pages/User/Profile.cshtml |
| AdminLogin2.aspx | Pages/Admin/Login.cshtml |
| AdminProfile.aspx | Pages/Admin/Dashboard.cshtml |
| AddTour.aspx | Pages/Tour/Create.cshtml |
| DisplayTours.aspx | Pages/Tour/Index.cshtml |
| TourCrud.aspx | Pages/Admin/Dashboard.cshtml (tours section) |
| usercrud.aspx | Pages/Admin/Dashboard.cshtml (users section) |
| Order.aspx | Pages/Booking/Create.cshtml |
| mybooking.aspx | Pages/Booking/MyBookings.cshtml |
| allbooking.aspx | Pages/Admin/Dashboard.cshtml (bookings section) |

## Key Differences from Web Forms

1. **No ViewState** - Replaced with model binding and TempData
2. **No Code-Behind** - Replaced with PageModel classes
3. **No Server Controls** - Replaced with HTML Tag Helpers
4. **No System.Web** - Replaced with ASP.NET Core equivalents
5. **No Web.config** - Replaced with appsettings.json
6. **No Global.asax** - Replaced with Program.cs
7. **No ADO.NET** - Replaced with Entity Framework Core 8
8. **No packages.config** - Replaced with SDK-style .csproj PackageReference

## Breaking Changes

- Session management uses `ISession` instead of `HttpSessionState`
- File uploads use `IFormFile` instead of `FileUpload` server control
- Authentication uses session-based approach (can be upgraded to ASP.NET Core Identity)
- Connection strings in appsettings.json instead of Web.config

## Configuration Changes

- `Web.config` → `appsettings.json`
- Connection string format updated for EF Core
- Admin credentials stored in appsettings.json

## Known Issues

- Password storage is plain text (same as original) - should be hashed with BCrypt in production
- Session-based auth should be upgraded to ASP.NET Core Identity for production use

## Future Improvements

1. Implement BCrypt password hashing
2. Migrate to ASP.NET Core Identity for proper authentication
3. Add JWT tokens for API support
4. Implement pagination for large datasets
5. Add image optimization for tour pictures
