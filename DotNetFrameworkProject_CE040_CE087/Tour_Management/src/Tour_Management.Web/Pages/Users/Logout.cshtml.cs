using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages.Users;

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
        HttpContext.Session.Remove("UserName");
        HttpContext.Session.Remove("UserId");
        _logger.LogInformation("User {Email} logged out", email);
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToPage("/Index");
    }
}
