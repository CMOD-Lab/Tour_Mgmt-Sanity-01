using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>Page model for cancelling/deleting a booking.</summary>
public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    [BindProperty]
    public BookingDeleteViewModel? Booking { get; set; }

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id, cancellationToken);
            if (booking is null) return NotFound();

            Booking = new BookingDeleteViewModel
            {
                Id = booking.Id,
                TourName = booking.TourName,
                Email = booking.Email,
                FirstName = booking.FirstName,
                BookingDate = booking.BookingDate
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for deletion {BookingId}", id);
            return RedirectToPage("MyBookings");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (Booking is null) return RedirectToPage("MyBookings");

        try
        {
            await _bookingService.DeleteAsync(Booking.Id, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", Booking.Id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("MyBookings");
        }
    }
}
