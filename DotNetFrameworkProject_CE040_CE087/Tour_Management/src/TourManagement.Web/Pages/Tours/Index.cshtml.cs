using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the Tours index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<TourDto> Tours { get; set; } = Enumerable.Empty<TourDto>();
    public string? SearchTerm { get; set; }

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? searchTerm = null)
    {
        try
        {
            SearchTerm = searchTerm;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Tours = await _tourService.SearchAsync(searchTerm);
            }
            else
            {
                Tours = await _tourService.GetAllAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = Enumerable.Empty<TourDto>();
        }
    }
}
