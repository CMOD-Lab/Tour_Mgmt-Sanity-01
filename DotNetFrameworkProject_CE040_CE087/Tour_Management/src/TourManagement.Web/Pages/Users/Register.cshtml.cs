using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user registration.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    /// <summary>Gets or sets the registration input model.</summary>
    [BindProperty]
    public RegisterViewModel RegisterInput { get; set; } = new();

    /// <summary>Initializes a new instance of <see cref="RegisterModel"/>.</summary>
    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the registration page.</summary>
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
            return RedirectToPage("Profile");

        return Page();
    }

    /// <summary>Handles POST requests to register a new user.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var user = new UserInfo
            {
                Email = RegisterInput.Email,
                FirstName = RegisterInput.FirstName,
                LastName = RegisterInput.LastName,
                Gender = RegisterInput.Gender,
                DateOfBirth = RegisterInput.DateOfBirth,
                Street = RegisterInput.Street,
                City = RegisterInput.City,
                State = RegisterInput.State,
                CreatedBy = "self"
            };

            var created = await _userService.CreateAsync(user, RegisterInput.Password, cancellationToken);
            _logger.LogInformation("New user registered: {Email}", created.Email);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("Login");
        }
        catch (DuplicateEntityException)
        {
            ModelState.AddModelError("RegisterInput.Email", "This email address is already registered.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", RegisterInput.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
