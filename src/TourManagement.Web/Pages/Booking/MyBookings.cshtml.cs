using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Booking;

/// <summary>
/// Page model for viewing user's bookings.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            Bookings = await _bookingService.GetByEmailAsync(email, cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for {Email}.", email);
            TempData["Error"] = "An error occurred while loading your bookings.";
            return Page();
        }
    }
}
