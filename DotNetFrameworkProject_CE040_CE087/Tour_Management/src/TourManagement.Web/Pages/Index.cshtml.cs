using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages;

/// <summary>
/// Page model for the home/index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Gets the featured tours to display on the home page.</summary>
    public IEnumerable<TourListViewModel> FeaturedTours { get; private set; } = Enumerable.Empty<TourListViewModel>();

    /// <summary>Initializes a new instance of <see cref="IndexModel"/>.</summary>
    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the home page.</summary>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            FeaturedTours = tours.Take(6).Select(t => new TourListViewModel
            {
                Id = t.Id,
                TourName = t.TourName,
                Place = t.Place,
                Days = t.Days,
                Price = t.Price,
                Locations = t.Locations,
                PictureName = t.PictureName,
                IsActive = t.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured tours for home page");
            FeaturedTours = Enumerable.Empty<TourListViewModel>();
        }
    }
}
