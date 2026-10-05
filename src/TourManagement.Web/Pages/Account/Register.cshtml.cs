using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Account;

/// <summary>
/// Page model for the registration page.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    /// <summary>
    /// Bound input model for the registration form.
    /// </summary>
    [BindProperty]
    public RegisterViewModel Input { get; set; } = new RegisterViewModel();

    /// <summary>
    /// Initializes a new instance of RegisterModel.
    /// </summary>
    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the registration page.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Handles POST requests for the registration form.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var createDto = new UserCreateDto
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            await _userService.CreateAsync(createDto);
            _logger.LogInformation("New user registered with email {Email}", Input.Email);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("/Account/Login");
        }
        catch (DuplicateEntityException)
        {
            Input.ErrorMessage = $"An account with email '{Input.Email}' already exists.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", Input.Email);
            Input.ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
