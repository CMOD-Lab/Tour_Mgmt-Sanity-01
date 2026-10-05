using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
[Authorize]
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>
    /// Bound input model for the booking form.
    /// </summary>
    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new BookingCreateViewModel();

    /// <summary>
    /// Initializes a new instance of CreateModel.
    /// </summary>
    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the booking creation page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int? tourId)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? string.Empty;
            var firstName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

            Input.Email = userEmail;
            Input.FirstName = firstName;

            if (tourId.HasValue)
            {
                var tour = await _tourService.GetByIdAsync(tourId.Value);
                if (tour != null)
                {
                    Input.TourName = tour.TourName;
                    Input.Place = tour.Place;
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking creation page");
            return RedirectToPage("/Tours/Index");
        }
    }

    /// <summary>
    /// Handles POST requests for the booking form.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var createDto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName
            };

            var booking = await _bookingService.CreateAsync(createDto);
            _logger.LogInformation("Booking created with ID {BookingId} for {Email}", booking.BookingId, booking.Email);

            TempData["SuccessMessage"] = $"Your booking for '{booking.TourName}' was confirmed successfully!";
            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            Input.ErrorMessage = "An error occurred while creating your booking. Please try again.";
            return Page();
        }
    }
}
