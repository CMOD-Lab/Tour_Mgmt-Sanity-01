using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for deleting a tour.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>Gets or sets the tour to delete.</summary>
    [BindProperty]
    public TourDeleteViewModel? Tour { get; set; }

    /// <summary>Initializes a new instance of <see cref="DeleteModel"/>.</summary>
    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the delete tour confirmation page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
                return NotFound();

            Tour = new TourDeleteViewModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion with ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    /// <summary>Handles POST requests to delete a tour.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        if (Tour is null)
            return RedirectToPage("Index");

        try
        {
            var result = await _tourService.DeleteAsync(Tour.Id, cancellationToken);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", Tour.Id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("Index");
        }
    }
}
