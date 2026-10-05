using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>
/// Create tour page model.
/// </summary>
public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public CreateTourViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

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
            string? picFileName = null;

            // Handle file upload
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
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName
            };

            await _tourService.CreateTourAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created: {TourName}", Input.TourName);
            TempData["SuccessMessage"] = "Tour added successfully!";
            return RedirectToPage("/Tours/Manage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while adding the tour. Please try again.";
            return Page();
        }
    }
}
