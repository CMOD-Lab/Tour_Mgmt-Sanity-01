using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for viewing all bookings (admin).
/// </summary>
[Authorize(Roles = "Admin")]
public class AllBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    /// <summary>
    /// All bookings.
    /// </summary>
    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    /// <summary>
    /// Current search term.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Initializes a new instance of AllBookingsModel.
    /// </summary>
    public AllBookingsModel(IBookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the all bookings page.
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            IEnumerable<TourManagement.Application.DTOs.BookingDto> bookings;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                bookings = await _bookingService.SearchAsync(SearchTerm);
            }
            else
            {
                bookings = await _bookingService.GetAllAsync();
            }

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings");
            Bookings = new List<BookingViewModel>();
        }
    }
}
