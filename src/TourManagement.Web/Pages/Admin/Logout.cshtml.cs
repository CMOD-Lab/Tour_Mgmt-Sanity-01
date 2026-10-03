using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin logout.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("IsAdmin");
        _logger.LogInformation("Admin logged out.");
        TempData["Success"] = "Admin logged out successfully.";
        return RedirectToPage("/Admin/Login");
    }
}
