using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>Page model for all bookings list (admin).</summary>
public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<BookingListViewModel> Bookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();
    public string? SearchTerm { get; private set; }

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            SearchTerm = searchTerm;
            var bookings = string.IsNullOrWhiteSpace(searchTerm)
                ? await _bookingService.GetAllAsync(cancellationToken)
                : await _bookingService.SearchAsync(searchTerm, cancellationToken);

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
            _logger.LogError(ex, "Error loading all bookings");
            return Page();
        }
    }
}
