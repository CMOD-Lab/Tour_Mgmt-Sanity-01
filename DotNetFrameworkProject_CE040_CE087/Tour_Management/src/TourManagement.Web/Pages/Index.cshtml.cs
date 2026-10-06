using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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

    public IEnumerable<TourViewModel> FeaturedTours { get; set; } = new List<TourViewModel>();

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            FeaturedTours = tours.Take(6).Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured tours on home page");
            FeaturedTours = new List<TourViewModel>();
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
