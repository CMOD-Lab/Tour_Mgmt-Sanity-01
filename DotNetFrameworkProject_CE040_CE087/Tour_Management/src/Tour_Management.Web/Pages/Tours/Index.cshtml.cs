using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Tours index page model.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();
    public string? SearchTerm { get; set; }

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        SearchTerm = searchTerm;
        try
        {
            var tours = string.IsNullOrWhiteSpace(searchTerm)
                ? await _tourService.GetAllToursAsync(cancellationToken)
                : await _tourService.SearchToursAsync(searchTerm, cancellationToken);

            // Manual ViewModel mapping
            Tours = tours.Select(t => new TourViewModel
            {
                TourId = t.TourId,
                TourName = t.TourName,
                Place = t.Place,
                Days = t.Days,
                Price = t.Price,
                Locations = t.Locations,
                TourInfo = t.TourInfo,
                Pic = t.Pic
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
        }
    }
}
