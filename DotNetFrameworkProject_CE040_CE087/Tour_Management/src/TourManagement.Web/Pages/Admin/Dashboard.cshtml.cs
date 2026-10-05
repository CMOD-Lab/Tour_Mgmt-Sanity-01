using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
public class DashboardModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly ILogger<DashboardModel> _logger;

    public bool IsAdmin { get; set; }
    public int TotalTours { get; set; }
    public int TotalBookings { get; set; }
    public int TotalUsers { get; set; }

    public DashboardModel(
        ITourService tourService,
        IBookingService bookingService,
        IUserService userService,
        ILogger<DashboardModel> logger)
    {
        _tourService = tourService;
        _bookingService = bookingService;
        _userService = userService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

        if (IsAdmin)
        {
            try
            {
                var tours = await _tourService.GetAllAsync();
                var bookings = await _bookingService.GetAllAsync();
                var users = await _userService.GetAllAsync();

                TotalTours = tours.Count();
                TotalBookings = bookings.Count();
                TotalUsers = users.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard statistics");
            }
        }
    }
}
