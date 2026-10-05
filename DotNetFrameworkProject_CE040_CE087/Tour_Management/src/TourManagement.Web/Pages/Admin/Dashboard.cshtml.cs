using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

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

    /// <summary>Gets the total number of tours.</summary>
    public int TotalTours { get; private set; }

    /// <summary>Gets the total number of users.</summary>
    public int TotalUsers { get; private set; }

    /// <summary>Gets the total number of bookings.</summary>
    public int TotalBookings { get; private set; }

    /// <summary>Initializes a new instance of <see cref="DashboardModel"/>.</summary>
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

    /// <summary>Handles GET requests for the admin dashboard.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("Login");

        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            var users = await _userService.GetAllAsync(cancellationToken);
            var bookings = await _bookingService.GetAllAsync(cancellationToken);

            TotalTours = tours.Count();
            TotalUsers = users.Count();
            TotalBookings = bookings.Count();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            return Page();
        }
    }
}
