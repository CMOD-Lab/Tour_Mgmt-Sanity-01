using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Booking;

/// <summary>
/// Page model for cancelling a booking.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public BookingDto? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            Booking = await _bookingService.GetByIdAsync(id, cancellationToken);
            if (Booking == null) return NotFound();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for cancel, ID {BookingId}.", id);
            return RedirectToPage("/Booking/MyBookings");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            await _bookingService.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToPage("/Booking/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with ID {BookingId}.", id);
            TempData["Error"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("/Booking/MyBookings");
        }
    }
}
