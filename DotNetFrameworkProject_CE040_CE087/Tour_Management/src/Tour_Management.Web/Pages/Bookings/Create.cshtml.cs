using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// Create booking page model.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public CreateBookingViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public void OnGet(string? tourName)
    {
        if (!string.IsNullOrEmpty(tourName))
            Input.TourName = tourName;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manual ViewModel to entity mapping
            var booking = new Booking
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName
            };

            await _bookingService.CreateBookingAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created for tour: {TourName}", Input.TourName);
            TempData["SuccessMessage"] = "Booking successful!";
            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while creating the booking. Please try again.";
            return Page();
        }
    }
}
