using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for creating a new tour.
/// </summary>
public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the tour input model.</summary>
    [BindProperty]
    public TourCreateViewModel TourInput { get; set; } = new();

    /// <summary>Initializes a new instance of <see cref="CreateModel"/>.</summary>
    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the create tour page.</summary>
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        return Page();
    }

    /// <summary>Handles POST requests to create a new tour.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? pictureName = null;

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

            var tour = new Tour
            {
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                PictureName = pictureName,
                CreatedBy = HttpContext.Session.GetString("UserEmail") ?? "admin"
            };

            var created = await _tourService.CreateAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created successfully with ID {TourId}", created.Id);

            TempData["SuccessMessage"] = $"Tour '{created.TourName}' added successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour. Please try again.");
            return Page();
        }
    }
}
