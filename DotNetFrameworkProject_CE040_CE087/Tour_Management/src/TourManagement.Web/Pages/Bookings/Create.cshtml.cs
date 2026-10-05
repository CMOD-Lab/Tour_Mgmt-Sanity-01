using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking (order).
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public BookingCreateViewModel Booking { get; set; } = new();

    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(int? tourId = null)
    {
        // Pre-fill session email if logged in
        var email = HttpContext.Session.GetString("UserEmail");
        var name = HttpContext.Session.GetString("UserName");

        if (!string.IsNullOrEmpty(email))
            Booking.Email = email;
        if (!string.IsNullOrEmpty(name))
            Booking.FirstName = name;

        // Pre-fill tour info if tourId provided
        if (tourId.HasValue)
        {
            Booking.TourId = tourId;
            var tour = await _tourService.GetByIdAsync(tourId.Value);
            if (tour != null)
            {
                Booking.TourName = tour.TourName;
                Booking.Place = tour.Place;
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var dto = new BookingCreateDto
            {
                TourName = Booking.TourName,
                Place = Booking.Place,
                Email = Booking.Email,
                FirstName = Booking.FirstName,
                TourId = Booking.TourId
            };

            await _bookingService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            _logger.LogInformation("New booking created for tour: {TourName}", Booking.TourName);
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", Booking.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while creating your booking. Please try again.");
            return Page();
        }
    }
}
