using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Edit tour page model.
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public EditTourViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            var tour = await _tourService.GetTourByIdAsync(id, cancellationToken);
            if (tour == null)
                return NotFound();

            // Manual ViewModel mapping
            Input = new EditTourViewModel
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit: {TourId}", id);
            return RedirectToPage("/Tours/Manage");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? picFileName = Input.Pic;

            // Handle new file upload
            if (Input.PicFile != null && Input.PicFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);
                picFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.PicFile.FileName);
                var filePath = Path.Combine(uploadsFolder, picFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.PicFile.CopyToAsync(stream, cancellationToken);
            }

            // Manual ViewModel to entity mapping
            var tour = new Tour
            {
                TourId = Input.TourId,
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName,
                IsActive = true
            };

            await _tourService.UpdateTourAsync(tour, cancellationToken);
            _logger.LogInformation("Tour updated: {TourId}", Input.TourId);
            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("/Tours/Manage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour: {TourId}", Input.TourId);
            ErrorMessage = "An error occurred while updating the tour. Please try again.";
            return Page();
        }
    }
}
