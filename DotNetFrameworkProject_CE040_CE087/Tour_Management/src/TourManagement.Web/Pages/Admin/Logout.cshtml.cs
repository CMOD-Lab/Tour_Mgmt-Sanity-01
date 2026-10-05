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
        var email = HttpContext.Session.GetString("AdminEmail");
        HttpContext.Session.Remove("IsAdmin");
        HttpContext.Session.Remove("AdminEmail");
        _logger.LogInformation("Admin {Email} logged out", email);
        TempData["SuccessMessage"] = "Admin logged out successfully.";
        return RedirectToPage("/Index");
    }
}
