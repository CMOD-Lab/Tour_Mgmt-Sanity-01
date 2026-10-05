using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Total number of tours.
    /// </summary>
    public int TotalTours { get; set; }

    /// <summary>
    /// Total number of bookings.
    /// </summary>
    public int TotalBookings { get; set; }

    /// <summary>
    /// Total number of registered users.
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Initializes a new instance of IndexModel.
    /// </summary>
    public IndexModel(
        ITourService tourService,
        IBookingService bookingService,
        IUserService userService,
        ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _bookingService = bookingService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the admin dashboard.
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            var tours = await _tourService.GetAllAsync();
            TotalTours = tours.Count();

            var bookings = await _bookingService.GetAllAsync();
            TotalBookings = bookings.Count();

            var users = await _userService.GetAllAsync();
            TotalUsers = users.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }
    }
}
