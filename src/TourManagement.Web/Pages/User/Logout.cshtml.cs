using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.User;

/// <summary>
/// Page model for user logout.
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
        var email = HttpContext.Session.GetString("UserEmail");
        HttpContext.Session.Remove("UserEmail");
        HttpContext.Session.Remove("UserFirstName");
        _logger.LogInformation("User {Email} logged out.", email);
        TempData["Success"] = "You have been logged out successfully.";
        return RedirectToPage("/User/Login");
    }
}
