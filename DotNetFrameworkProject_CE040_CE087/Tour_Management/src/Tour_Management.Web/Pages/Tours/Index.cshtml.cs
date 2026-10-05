using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Page model for the tours listing page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<Tour> Tours { get; set; } = new List<Tour>();
    public string SearchTerm { get; set; } = string.Empty;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? searchTerm = null)
    {
        try
        {
            SearchTerm = searchTerm ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Tours = await _tourService.SearchToursAsync(searchTerm);
            }
            else
            {
                Tours = await _tourService.GetAllToursAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = new List<Tour>();
        }
    }
}
