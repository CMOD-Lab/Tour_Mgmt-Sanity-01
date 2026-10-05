using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages;

/// <summary>
/// Page model for the home/index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<TourDto> FeaturedTours { get; set; } = Enumerable.Empty<TourDto>();

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var allTours = await _tourService.GetAllAsync();
            FeaturedTours = allTours.Take(6);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured tours on home page");
            FeaturedTours = Enumerable.Empty<TourDto>();
        }
    }
}
