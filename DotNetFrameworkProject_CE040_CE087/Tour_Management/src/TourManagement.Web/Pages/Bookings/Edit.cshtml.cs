using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for editing a booking.
/// </summary>
public class EditModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public BookingEditViewModel Booking { get; set; } = new();

    public EditModel(IBookingService bookingService, ILogger<EditModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            // Manually map DTO to ViewModel
            Booking = new BookingEditViewModel
            {
                Id = booking.Id,
                TourName = booking.TourName,
                Place = booking.Place,
                Email = booking.Email,
                FirstName = booking.FirstName,
                IsActive = booking.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for edit with ID {BookingId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var dto = new BookingUpdateDto
            {
                TourName = Booking.TourName,
                Place = Booking.Place,
                Email = Booking.Email,
                FirstName = Booking.FirstName,
                IsActive = Booking.IsActive
            };

            await _bookingService.UpdateAsync(Booking.Id, dto);
            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", Booking.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the booking. Please try again.");
            return Page();
        }
    }
}
