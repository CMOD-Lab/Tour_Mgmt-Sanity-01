using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for booking details.
/// </summary>
[Authorize]
public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>
    /// The booking to display.
    /// </summary>
    public BookingViewModel? Booking { get; set; }

    /// <summary>
    /// Initializes a new instance of DetailsModel.
    /// </summary>
    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the booking details page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Ensure user can only see their own bookings (unless admin)
            if (!User.IsInRole("Admin") && booking.Email != User.Identity?.Name)
            {
                return Forbid();
            }

            Booking = new BookingViewModel
            {
                BookingId = booking.BookingId,
                TourName = booking.TourName,
                Place = booking.Place,
                Email = booking.Email,
                FirstName = booking.FirstName,
                BookingDate = booking.BookingDate,
                IsActive = booking.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID {BookingId}", id);
            return RedirectToPage("/Bookings/MyBookings");
        }
    }
}
