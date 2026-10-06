using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for the user's own bookings.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var bookings = await _bookingService.GetByEmailAsync(userEmail, cancellationToken);
            Bookings = bookings.Select(b => new BookingViewModel
            {
                Id = b.Id,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                IsActive = b.IsActive,
                CreatedDate = b.CreatedDate
            }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user: {Email}", userEmail);
            Bookings = new List<BookingViewModel>();
            return Page();
        }
    }
}
