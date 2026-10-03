using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tour;

/// <summary>
/// Page model for listing all tours.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<TourDto> Tours { get; set; } = Enumerable.Empty<TourDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Tours = await _tourService.SearchAsync(SearchTerm, cancellationToken);
            }
            else
            {
                Tours = await _tourService.GetAllAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours.");
            TempData["Error"] = "An error occurred while loading tours.";
        }
    }
}
