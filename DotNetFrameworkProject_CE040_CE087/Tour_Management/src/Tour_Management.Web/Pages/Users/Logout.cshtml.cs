using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages.Users;

/// <summary>
/// User logout page model.
/// </summary>
public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("UserEmail");
        HttpContext.Session.Remove("UserName");
        return RedirectToPage("/Users/Login");
    }
}
