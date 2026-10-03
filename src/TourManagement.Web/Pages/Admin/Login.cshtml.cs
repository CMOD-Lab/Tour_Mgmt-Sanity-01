using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty]
    public AdminLoginViewModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") == "true")
        {
            return RedirectToPage("/Admin/Dashboard");
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AdminCredentials:Password"] ?? "admin";

        if (Input.Email == adminEmail && Input.Password == adminPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            _logger.LogInformation("Admin logged in successfully.");
            return RedirectToPage("/Admin/Dashboard");
        }

        TempData["Error"] = "Invalid admin credentials.";
        return Page();
    }
}
