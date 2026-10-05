using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Tour details page model.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    public TourViewModel? Tour { get; set; }

    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetTourByIdAsync(id, cancellationToken);
            if (tour != null)
            {
                Tour = new TourViewModel
                {
                    TourId = tour.TourId,
                    TourName = tour.TourName,
                    Place = tour.Place,
                    Days = tour.Days,
                    Price = tour.Price,
                    Locations = tour.Locations,
                    TourInfo = tour.TourInfo,
                    Pic = tour.Pic
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
        }

        return Page();
    }
}
