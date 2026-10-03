using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tour;

/// <summary>
/// Page model for deleting a tour (admin only).
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public TourDto? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (Tour == null) return NotFound();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete, ID {TourId}.", id);
            return RedirectToPage("/Admin/Dashboard");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            await _tourService.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Tour deleted successfully!";
            return RedirectToPage("/Admin/Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}.", id);
            TempData["Error"] = "An error occurred while deleting the tour.";
            return RedirectToPage("/Admin/Dashboard");
        }
    }
}
