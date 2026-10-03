using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Booking;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly IUserService _userService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IBookingService bookingService,
        ITourService tourService,
        IUserService userService,
        ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    public TourDto? SelectedTour { get; set; }

    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            // Pre-fill user info
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user != null)
            {
                Input.Email = user.Email;
                Input.FirstName = user.FirstName;
            }

            // Pre-fill tour info if tourId provided
            if (tourId.HasValue)
            {
                SelectedTour = await _tourService.GetByIdAsync(tourId.Value, cancellationToken);
                if (SelectedTour != null)
                {
                    Input.TourName = SelectedTour.TourName;
                    Input.Place = SelectedTour.Place;
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking form.");
            return RedirectToPage("/Tour/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Manually map ViewModel to DTO
            var dto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            TempData["Success"] = "Booking confirmed successfully!";
            return RedirectToPage("/Booking/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking.");
            TempData["Error"] = "An error occurred while creating the booking.";
            return Page();
        }
    }
}
