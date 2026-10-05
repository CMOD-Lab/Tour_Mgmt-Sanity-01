using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages;

/// <summary>
/// Page model for the home/index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Recent tours to display on the home page.
    /// </summary>
    public IEnumerable<TourViewModel> RecentTours { get; set; } = new List<TourViewModel>();

    /// <summary>
    /// Initializes a new instance of IndexModel.
    /// </summary>
    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the home page.
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            var tours = await _tourService.GetAllAsync();
            RecentTours = tours.Take(6).Select(t => new TourViewModel
            {
                TourId = t.TourId,
                TourName = t.TourName,
                Place = t.Place,
                Days = t.Days,
                Price = t.Price,
                Locations = t.Locations,
                TourInfo = t.TourInfo,
                Pic = t.Pic,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page tours");
            RecentTours = new List<TourViewModel>();
        }
    }
}
