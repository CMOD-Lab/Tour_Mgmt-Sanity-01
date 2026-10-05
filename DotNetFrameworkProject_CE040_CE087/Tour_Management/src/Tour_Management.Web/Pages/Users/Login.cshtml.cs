using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>
/// User login page model.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var user = await _userService.AuthenticateAsync(Input.Email, Input.Password, cancellationToken);
            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FirstName);
                _logger.LogInformation("User {Email} logged in successfully", Input.Email);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid email or password.";
            _logger.LogWarning("Failed login attempt for email: {Email}", Input.Email);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", Input.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}
