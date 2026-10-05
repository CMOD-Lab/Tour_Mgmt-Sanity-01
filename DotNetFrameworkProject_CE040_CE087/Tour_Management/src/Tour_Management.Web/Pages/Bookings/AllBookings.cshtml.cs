using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// Page model for admin view of all bookings.
/// </summary>
public class AllBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public AllBookingsModel(IBookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            Bookings = await _bookingService.GetAllBookingsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings for admin");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            await _bookingService.DeleteBookingAsync(id);
            TempData["SuccessMessage"] = "Booking deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            TempData["ErrorMessage"] = "Error deleting booking.";
        }

        return RedirectToPage();
    }
}
