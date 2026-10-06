using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

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

    public int TotalTours { get; set; }
    public int TotalBookings { get; set; }
    public int TotalUsers { get; set; }
    public IEnumerable<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();

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

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
            var users = await _userService.GetAllAsync(cancellationToken);

            TotalTours = tours.Count();
            TotalBookings = bookings.Count();
            TotalUsers = users.Count();

            RecentBookings = bookings
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    TourName = b.TourName,
                    Place = b.Place,
                    Email = b.Email,
                    FirstName = b.FirstName,
                    IsActive = b.IsActive,
                    CreatedDate = b.CreatedDate
                }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            return Page();
        }
    }
}
