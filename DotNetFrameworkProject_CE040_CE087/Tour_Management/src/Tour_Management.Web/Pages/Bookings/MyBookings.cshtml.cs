using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// My bookings page model.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Users/Login");

        try
        {
            var bookings = await _bookingService.GetBookingsByEmailAsync(email, cancellationToken);
            Bookings = bookings.Select(b => new BookingViewModel
            {
                TourId = b.TourId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                CreatedDate = b.CreatedDate
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user: {Email}", email);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Users/Login");

        try
        {
            await _bookingService.DeleteBookingAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
            _logger.LogInformation("Booking {BookingId} cancelled by user {Email}", id, email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            TempData["ErrorMessage"] = "Error cancelling booking.";
        }

        return RedirectToPage();
    }
}
