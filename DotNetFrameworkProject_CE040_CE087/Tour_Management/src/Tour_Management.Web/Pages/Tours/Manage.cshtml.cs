using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Tour management page model (admin).
/// </summary>
public class ManageModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<ManageModel> _logger;

    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();

    public ManageModel(ITourService tourService, ILogger<ManageModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            var tours = await _tourService.GetAllToursAsync(cancellationToken);
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
            _logger.LogError(ex, "Error loading tours for management");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            await _tourService.DeleteTourAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
            _logger.LogInformation("Tour {TourId} deleted by admin", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour {TourId}", id);
            TempData["ErrorMessage"] = "Error deleting tour.";
        }

        return RedirectToPage();
    }
}
