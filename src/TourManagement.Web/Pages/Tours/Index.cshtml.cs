using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tours list page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// List of tours to display.
    /// </summary>
    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();

    /// <summary>
    /// Current search term.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Success message to display.
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of IndexModel.
    /// </summary>
    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the tours list page.
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            SuccessMessage = TempData["SuccessMessage"]?.ToString();

            IEnumerable<TourManagement.Application.DTOs.TourDto> tours;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                tours = await _tourService.SearchAsync(SearchTerm);
            }
            else
            {
                tours = await _tourService.GetAllAsync();
            }

            Tours = tours.Select(t => new TourViewModel
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
            _logger.LogError(ex, "Error loading tours list");
            Tours = new List<TourViewModel>();
        }
    }
}
