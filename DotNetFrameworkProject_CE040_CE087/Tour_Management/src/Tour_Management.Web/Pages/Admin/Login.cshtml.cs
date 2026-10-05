using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Admin;

/// <summary>
/// Admin login page model.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AdminCredentials:Password"] ?? "admin";

        if (Input.Email == adminEmail && Input.Password == adminPassword)
        {
            HttpContext.Session.SetString("AdminEmail", Input.Email);
            _logger.LogInformation("Admin logged in successfully");
            return RedirectToPage("/Admin/Profile");
        }

        ErrorMessage = "Invalid admin credentials.";
        _logger.LogWarning("Failed admin login attempt for email: {Email}", Input.Email);
        return Page();
    }
}
