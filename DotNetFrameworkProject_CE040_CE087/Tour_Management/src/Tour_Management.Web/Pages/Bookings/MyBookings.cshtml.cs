using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// Page model for viewing user's own bookings.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            Bookings = await _bookingService.GetBookingsByEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user {Email}", email);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            await _bookingService.DeleteBookingAsync(id);
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with ID {BookingId}", id);
            TempData["ErrorMessage"] = "Error cancelling booking.";
        }

        return RedirectToPage();
    }
}
