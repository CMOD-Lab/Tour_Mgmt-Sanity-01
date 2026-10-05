using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Delete tour page model.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    public TourViewModel? Tour { get; set; }

    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

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
            _logger.LogError(ex, "Error loading tour for delete: {TourId}", id);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            await _tourService.DeleteTourAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
            _logger.LogInformation("Tour {TourId} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour {TourId}", id);
            TempData["ErrorMessage"] = "Error deleting tour.";
        }

        return RedirectToPage("/Tours/Manage");
    }
}
