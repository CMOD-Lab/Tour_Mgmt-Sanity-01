using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages.Admin;

/// <summary>
/// Admin logout page model.
/// </summary>
public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("AdminEmail");
        return RedirectToPage("/Admin/Login");
    }
}
