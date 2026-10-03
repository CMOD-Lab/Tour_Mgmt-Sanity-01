using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
public class DashboardModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(
        ITourService tourService,
        IUserService userService,
        IBookingService bookingService,
        ILogger<DashboardModel> logger)
    {
        _tourService = tourService;
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<TourDto> Tours { get; set; } = Enumerable.Empty<TourDto>();
    public IEnumerable<UserDto> Users { get; set; } = Enumerable.Empty<UserDto>();
    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();
    public int TourCount { get; set; }
    public int UserCount { get; set; }
    public int BookingCount { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            Tours = await _tourService.GetAllAsync(cancellationToken);
            Users = await _userService.GetAllAsync(cancellationToken);
            Bookings = await _bookingService.GetAllAsync(cancellationToken);

            TourCount = Tours.Count();
            UserCount = Users.Count();
            BookingCount = Bookings.Count();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard.");
            TempData["Error"] = "An error occurred while loading the dashboard.";
            return Page();
        }
    }
}
