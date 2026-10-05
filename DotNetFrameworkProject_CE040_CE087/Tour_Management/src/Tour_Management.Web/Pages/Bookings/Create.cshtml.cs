using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking (Order).
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    public Tour? SelectedTour { get; set; }

    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? tourId = null)
    {
        if (tourId.HasValue)
        {
            SelectedTour = await _tourService.GetTourByIdAsync(tourId.Value);
            if (SelectedTour != null)
            {
                Input.TourId = tourId;
                Input.TourName = SelectedTour.TourName;
                Input.Place = SelectedTour.Place;
            }
        }

        // Pre-fill email if user is logged in
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (!string.IsNullOrEmpty(userEmail))
        {
            Input.Email = userEmail;
            var userName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(userName))
            {
                Input.FirstName = userName.Split(' ')[0];
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            if (Input.TourId.HasValue)
            {
                SelectedTour = await _tourService.GetTourByIdAsync(Input.TourId.Value);
            }
            return Page();
        }

        try
        {
            // Manually map ViewModel to domain entity
            var booking = new Booking
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName,
                TourId = Input.TourId,
                BookingDate = DateTime.UtcNow,
                IsActive = true
            };

            await _bookingService.CreateBookingAsync(booking);
            _logger.LogInformation("Booking created for tour {TourName} by {Email}", Input.TourName, Input.Email);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour {TourName}", Input.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            if (Input.TourId.HasValue)
            {
                SelectedTour = await _tourService.GetTourByIdAsync(Input.TourId.Value);
            }
            return Page();
        }
    }
}
