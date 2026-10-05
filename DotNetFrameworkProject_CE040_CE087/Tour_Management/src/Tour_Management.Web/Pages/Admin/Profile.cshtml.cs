using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages.Admin;

/// <summary>
/// Admin profile page model.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(ILogger<ProfileModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        return Page();
    }
}
