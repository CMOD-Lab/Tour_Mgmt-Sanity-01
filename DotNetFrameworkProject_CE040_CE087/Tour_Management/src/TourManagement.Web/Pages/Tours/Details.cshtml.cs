using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tour details page.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>Gets the tour details to display.</summary>
    public TourDetailsViewModel? Tour { get; private set; }

    /// <summary>Initializes a new instance of <see cref="DetailsModel"/>.</summary>
    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the tour details page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
                return NotFound();

            Tour = new TourDetailsViewModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                PictureName = tour.PictureName,
                CreatedDate = tour.CreatedDate,
                IsActive = tour.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }
}
