using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages;

/// <summary>
/// Index page model.
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
        _logger.LogInformation("Home page accessed");
    }
}
