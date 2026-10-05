using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for displaying a user's bookings.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    /// <summary>Gets the list of bookings.</summary>
    public IEnumerable<BookingListViewModel> Bookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of <see cref="MyBookingsModel"/>.</summary>
    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the my bookings page.</summary>
    public async Task<IActionResult> OnGetAsync(string? email = null, CancellationToken cancellationToken = default)
    {
        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        var targetEmail = email ?? sessionEmail;

        if (string.IsNullOrEmpty(targetEmail))
            return RedirectToPage("/Users/Login");

        try
        {
            var bookings = await _bookingService.GetByEmailAsync(targetEmail, cancellationToken);
            Bookings = bookings.Select(b => new BookingListViewModel
            {
                Id = b.Id,
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
            _logger.LogError(ex, "Error loading bookings for email {Email}", targetEmail);
            return Page();
        }
    }
}
