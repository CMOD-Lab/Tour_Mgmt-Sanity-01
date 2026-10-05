using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for editing an existing tour.
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Gets or sets the tour edit input model.</summary>
    [BindProperty]
    public TourEditViewModel TourInput { get; set; } = new();

    /// <summary>Initializes a new instance of <see cref="EditModel"/>.</summary>
    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the edit tour page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
                return NotFound();

            TourInput = new TourEditViewModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                ExistingPictureName = tour.PictureName,
                IsActive = tour.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    /// <summary>Handles POST requests to update a tour.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? pictureName = TourInput.ExistingPictureName;

            if (TourInput.PictureFile != null && TourInput.PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{TourInput.PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await TourInput.PictureFile.CopyToAsync(stream, cancellationToken);
                pictureName = uniqueFileName;
            }

            var tourUpdate = new Tour
            {
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                PictureName = pictureName,
                IsActive = TourInput.IsActive,
                ModifiedBy = HttpContext.Session.GetString("UserEmail") ?? "admin"
            };

            var updated = await _tourService.UpdateAsync(TourInput.Id, tourUpdate, cancellationToken);
            if (updated is null)
                return NotFound();

            TempData["SuccessMessage"] = $"Tour '{updated.TourName}' updated successfully!";
            return RedirectToPage("Details", new { id = TourInput.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", TourInput.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour. Please try again.");
            return Page();
        }
    }
}
