using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages;

/// <summary>
/// Page model for the home/index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<Tour> FeaturedTours { get; set; } = new List<Tour>();

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var allTours = await _tourService.GetAllToursAsync();
            FeaturedTours = allTours.Take(6);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured tours on home page");
            FeaturedTours = new List<Tour>();
        }
    }
}
