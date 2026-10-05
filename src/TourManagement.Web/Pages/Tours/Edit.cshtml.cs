using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for editing an existing tour.
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    /// <summary>
    /// Bound input model for the edit tour form.
    /// </summary>
    [BindProperty]
    public TourEditViewModel Input { get; set; } = new TourEditViewModel();

    /// <summary>
    /// Initializes a new instance of EditModel.
    /// </summary>
    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the edit tour page.
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

            Input = new TourEditViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                ExistingPic = tour.Pic,
                IsActive = tour.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for editing, ID {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }

    /// <summary>
    /// Handles POST requests for the edit tour form.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            string? picFileName = Input.ExistingPic;

            if (Input.PicFile != null && Input.PicFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.PicFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await Input.PicFile.CopyToAsync(fileStream);
                picFileName = uniqueFileName;
            }

            var updateDto = new TourUpdateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName,
                IsActive = Input.IsActive
            };

            await _tourService.UpdateAsync(Input.TourId, updateDto);
            _logger.LogInformation("Tour updated with ID {TourId}", Input.TourId);

            TempData["SuccessMessage"] = $"Tour '{Input.TourName}' was updated successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", Input.TourId);
            Input.ErrorMessage = "An error occurred while updating the tour. Please try again.";
            return Page();
        }
    }
}
