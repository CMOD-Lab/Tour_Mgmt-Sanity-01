using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for all bookings (admin view).
/// </summary>
public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Bookings = await _bookingService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings");
            Bookings = Enumerable.Empty<BookingDto>();
        }
    }
}
