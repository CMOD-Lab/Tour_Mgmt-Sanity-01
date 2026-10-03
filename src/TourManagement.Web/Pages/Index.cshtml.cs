using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages;

/// <summary>
/// Page model for the home/index page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Home page accessed.");
    }
}
