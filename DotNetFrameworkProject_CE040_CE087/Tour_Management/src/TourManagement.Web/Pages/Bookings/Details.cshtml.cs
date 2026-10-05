using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>Page model for booking details.</summary>
public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    public BookingDetailsViewModel? Booking { get; private set; }

    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
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

            Booking = new BookingDetailsViewModel
            {
                Id = booking.Id,
                TourName = booking.TourName,
                Place = booking.Place,
                Email = booking.Email,
                FirstName = booking.FirstName,
                BookingDate = booking.BookingDate,
                CreatedDate = booking.CreatedDate,
                IsActive = booking.IsActive
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID {BookingId}", id);
            return RedirectToPage("MyBookings");
        }
    }
}
