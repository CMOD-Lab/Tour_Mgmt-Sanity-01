using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for displaying the current user's bookings.
/// </summary>
[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    /// <summary>
    /// The current user's bookings.
    /// </summary>
    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    /// <summary>
    /// Success message to display.
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of MyBookingsModel.
    /// </summary>
    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the my bookings page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            SuccessMessage = TempData["SuccessMessage"]?.ToString();

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Account/Login");
            }

            var bookings = await _bookingService.GetByUserEmailAsync(email);
            Bookings = bookings.Select(b => new BookingViewModel
            {
                BookingId = b.BookingId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                BookingDate = b.BookingDate,
                IsActive = b.IsActive
            });

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading my bookings page");
            return RedirectToPage("/Index");
        }
    }
}
