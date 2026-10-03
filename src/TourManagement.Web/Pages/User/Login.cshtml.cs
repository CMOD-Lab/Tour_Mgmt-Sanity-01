using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.User;

/// <summary>
/// Page model for user login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserLoginViewModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
        {
            return RedirectToPage("/User/Profile");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var user = await _userService.AuthenticateAsync(Input.Email, Input.Password, cancellationToken);

            if (user == null)
            {
                TempData["Error"] = "Invalid email or password. Please try again.";
                return Page();
            }

            // Store user info in session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFirstName", user.FirstName);

            _logger.LogInformation("User {Email} logged in successfully.", user.Email);
            return RedirectToPage("/User/Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login for {Email}.", Input.Email);
            TempData["Error"] = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}
