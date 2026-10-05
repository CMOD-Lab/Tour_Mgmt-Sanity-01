using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking (Order page).
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the booking input model.</summary>
    [BindProperty]
    public BookingCreateViewModel BookingInput { get; set; } = new();

    /// <summary>Initializes a new instance of <see cref="CreateModel"/>.</summary>
    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the booking creation page.</summary>
    public IActionResult OnGet(int? tourId = null, string? tourName = null, string? place = null)
    {
        BookingInput.TourId = tourId;
        BookingInput.TourName = tourName ?? string.Empty;
        BookingInput.Place = place ?? string.Empty;

        // Pre-fill email from session if logged in
        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        if (!string.IsNullOrEmpty(sessionEmail))
            BookingInput.Email = sessionEmail;

        var sessionName = HttpContext.Session.GetString("UserName");
        if (!string.IsNullOrEmpty(sessionName))
            BookingInput.FirstName = sessionName.Split(' ')[0];

        return Page();
    }

    /// <summary>Handles POST requests to create a new booking.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var booking = new Booking
            {
                TourName = BookingInput.TourName,
                Place = BookingInput.Place,
                Email = BookingInput.Email,
                FirstName = BookingInput.FirstName,
                TourId = BookingInput.TourId,
                CreatedBy = HttpContext.Session.GetString("UserEmail") ?? BookingInput.Email
            };

            var created = await _bookingService.CreateAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created with ID {BookingId}", created.Id);

            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("MyBookings", new { email = BookingInput.Email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", BookingInput.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            return Page();
        }
    }
}
