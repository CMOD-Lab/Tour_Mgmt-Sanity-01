# Migration Notes

## What Was Migrated

### Pages Migrated (Web Forms → Razor Pages)
| Web Forms Page | Razor Page |
|---|---|
| userlogin.aspx | Pages/Users/Login.cshtml |
| SignUpForm.aspx | Pages/Users/Register.cshtml |
| MainProfilePage.aspx | Pages/Index.cshtml |
| AdminLogin2.aspx | Pages/Admin/Login.cshtml |
| AdminProfile.aspx | Pages/Admin/Profile.cshtml |
| AddTour.aspx | Pages/Tours/Create.cshtml |
| DisplayTours.aspx | Pages/Tours/Index.cshtml |
| TourCrud.aspx | Pages/Tours/Manage.cshtml |
| Order.aspx | Pages/Bookings/Create.cshtml |
| mybooking.aspx | Pages/Bookings/MyBookings.cshtml |
| allbooking.aspx | Pages/Bookings/AllBookings.cshtml |
| usercrud.aspx | Pages/Users/Manage.cshtml |

## Key Differences from Web Forms

1. **No ViewState** - Replaced with form model binding
2. **No Code-Behind** - Replaced with PageModel classes
3. **No Server Controls** - Replaced with HTML Tag Helpers
4. **No SqlDataSource** - Replaced with EF Core repositories
5. **No Response.Write** - Replaced with TempData messages
6. **No Server.MapPath** - Replaced with IWebHostEnvironment
7. **No Session["key"]** - Replaced with HttpContext.Session
8. **No ConfigurationManager** - Replaced with IConfiguration

## Breaking Changes

- Database schema columns mapped via EF Core configurations
- Authentication is session-based (not Forms Authentication)
- File uploads use IFormFile instead of FileUpload control

## Configuration Changes

- `Web.config` → `appsettings.json`
- Connection string format updated for EF Core
- Admin credentials stored in appsettings.json

## Known Issues

- The original database used LocalDB with a hardcoded path; update the connection string
- Password storage is plain text (as in original) - should be hashed in production
- Session-based auth should be replaced with ASP.NET Core Identity for production

## Future Improvements

1. Implement proper password hashing (BCrypt)
2. Add ASP.NET Core Identity for authentication
3. Add pagination for large datasets
4. Implement proper CSRF protection
5. Add input sanitization
6. Implement proper file upload validation
