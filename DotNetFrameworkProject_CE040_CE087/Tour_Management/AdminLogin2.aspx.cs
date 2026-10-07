// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC)
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
    public class AdminLogin2Model : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        // Replaces Web Forms Page_Load event handler
        public void OnGet()
        {
        }

        // Replaces Web Forms Button1_Click / Page_Load login logic
        public IActionResult OnPost()
        {
            if (Password == "admin" && Email == "admin@gmail.com")
            {
                // Replaces Response.Redirect / Server.Transfer with ASP.NET Core redirect
                return RedirectToPage("/AdminProfile");
            }

            ErrorMessage = "Invalid credentials. Please try again.";
            return Page();
        }
    }
}
