using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>
/// User registration page model.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    [BindProperty]
    public RegisterViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
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
            // Map ViewModel to domain entity
            var user = new UserInfo
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                DateOfBirth = Input.DateOfBirth,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            await _userService.RegisterUserAsync(user, cancellationToken);
            _logger.LogInformation("New user registered: {Email}", Input.Email);
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("/Users/Login");
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", Input.Email);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
