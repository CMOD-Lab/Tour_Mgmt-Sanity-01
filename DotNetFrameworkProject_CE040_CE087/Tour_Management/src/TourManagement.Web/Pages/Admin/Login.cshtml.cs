using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    /// <summary>Gets or sets the admin login input model.</summary>
    [BindProperty]
    public AdminLoginViewModel AdminInput { get; set; } = new();

    /// <summary>Gets the error message to display.</summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>Initializes a new instance of <see cref="LoginModel"/>.</summary>
    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the admin login page.</summary>
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") == "true")
            return RedirectToPage("Dashboard");

        return Page();
    }

    /// <summary>Handles POST requests to authenticate an admin.</summary>
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var adminEmail = _configuration["AppSettings:AdminEmail"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AppSettings:AdminPassword"] ?? "admin";

        if (AdminInput.Email == adminEmail && AdminInput.Password == adminPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            HttpContext.Session.SetString("UserEmail", AdminInput.Email);
            HttpContext.Session.SetString("UserName", "Administrator");
            _logger.LogInformation("Admin logged in successfully");
            return RedirectToPage("Dashboard");
        }

        ErrorMessage = "Invalid admin credentials.";
        _logger.LogWarning("Failed admin login attempt for email {Email}", AdminInput.Email);
        return Page();
    }
}
