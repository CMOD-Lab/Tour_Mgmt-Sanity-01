using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    /// <summary>Gets or sets the login input model.</summary>
    [BindProperty]
    public LoginViewModel LoginInput { get; set; } = new();

    /// <summary>Gets the error message to display.</summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>Initializes a new instance of <see cref="LoginModel"/>.</summary>
    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the login page.</summary>
    public IActionResult OnGet(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
            return RedirectToPage("Profile");

        LoginInput.ReturnUrl = returnUrl;
        return Page();
    }

    /// <summary>Handles POST requests to authenticate a user.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var user = await _userService.ValidateCredentialsAsync(
                LoginInput.Email, LoginInput.Password, cancellationToken);

            if (user is null)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            // Store user info in session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            if (!string.IsNullOrEmpty(LoginInput.ReturnUrl) && Url.IsLocalUrl(LoginInput.ReturnUrl))
                return Redirect(LoginInput.ReturnUrl);

            return RedirectToPage("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", LoginInput.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}
