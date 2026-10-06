using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? tourId = null, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("/Users/Login");
        }

        // Pre-fill user email from session
        Input.Email = HttpContext.Session.GetString("UserEmail") ?? string.Empty;
        Input.FirstName = HttpContext.Session.GetString("UserName") ?? string.Empty;

        // Pre-fill tour details if tourId provided
        if (tourId.HasValue)
        {
            Input.TourId = tourId;
            var tour = await _tourService.GetByIdAsync(tourId.Value, cancellationToken);
            if (tour != null)
            {
                Input.TourName = tour.TourName;
                Input.Place = tour.Place;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("/Users/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            int? userId = int.TryParse(userIdStr, out int uid) ? uid : null;

            var dto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName,
                TourId = Input.TourId,
                UserId = userId
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            return Page();
        }
    }
}
