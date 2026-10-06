using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tours list page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            SearchTerm = searchTerm ?? string.Empty;
            IEnumerable<TourDto> tours;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                tours = await _tourService.SearchAsync(searchTerm, cancellationToken);
            }
            else
            {
                tours = await _tourService.GetAllAsync(cancellationToken);
            }

            Tours = tours.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = new List<TourViewModel>();
        }
    }

    private static TourViewModel MapToViewModel(TourDto dto) => new()
    {
        Id = dto.Id,
        TourName = dto.TourName,
        Place = dto.Place,
        Days = dto.Days,
        Price = dto.Price,
        Locations = dto.Locations,
        TourInfo = dto.TourInfo,
        PicturePath = dto.PicturePath,
        IsActive = dto.IsActive,
        CreatedDate = dto.CreatedDate
    };
}
