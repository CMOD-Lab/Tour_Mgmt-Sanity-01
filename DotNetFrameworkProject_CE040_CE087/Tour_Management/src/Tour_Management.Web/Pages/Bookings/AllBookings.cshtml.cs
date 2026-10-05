using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>
/// All bookings page model (admin).
/// </summary>
public class AllBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    public AllBookingsModel(IBookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            var bookings = await _bookingService.GetAllBookingsAsync(cancellationToken);
            Bookings = bookings.Select(b => new BookingViewModel
            {
                TourId = b.TourId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                CreatedDate = b.CreatedDate
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings");
        }

        return Page();
    }
}
