using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// Page model for booking details.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    public Booking? Booking { get; set; }

    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var adminEmail = HttpContext.Session.GetString("AdminEmail");

        if (string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(adminEmail))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            Booking = await _bookingService.GetBookingByIdAsync(id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID {BookingId}", id);
            return RedirectToPage("MyBookings");
        }
    }
}
