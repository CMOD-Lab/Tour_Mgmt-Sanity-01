using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for the admin bookings list.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
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
            _logger.LogError(ex, "Error loading bookings list");
            Bookings = new List<BookingViewModel>();
            return Page();
        }
    }
}
