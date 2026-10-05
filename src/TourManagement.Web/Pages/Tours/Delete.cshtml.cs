using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for deleting a tour.
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>
    /// The tour to delete.
    /// </summary>
    public TourViewModel? Tour { get; set; }

    /// <summary>
    /// Initializes a new instance of DeleteModel.
    /// </summary>
    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the delete tour page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            Tour = new TourViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                Pic = tour.Pic,
                IsActive = tour.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion, ID {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }

    /// <summary>
    /// Handles POST requests for the delete tour form.
    /// </summary>
    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _tourService.DeleteAsync(id);
            _logger.LogInformation("Tour deleted with ID {TourId}", id);

            TempData["SuccessMessage"] = "Tour was deleted successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("/Tours/Index");
        }
    }
}
