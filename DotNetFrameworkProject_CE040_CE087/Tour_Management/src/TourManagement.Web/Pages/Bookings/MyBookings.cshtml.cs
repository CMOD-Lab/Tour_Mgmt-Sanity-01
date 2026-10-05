using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for viewing the current user's bookings.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();
    public bool IsLoggedIn { get; set; }

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var email = HttpContext.Session.GetString("UserEmail");
            IsLoggedIn = !string.IsNullOrEmpty(email);

            if (IsLoggedIn && email != null)
            {
                Bookings = await _bookingService.GetByEmailAsync(email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user bookings");
            Bookings = Enumerable.Empty<BookingDto>();
        }
    }
}
