using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user registration.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    [BindProperty]
    public UserRegisterViewModel Register { get; set; } = new();

    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var dto = new UserCreateDto
            {
                Email = Register.Email,
                FirstName = Register.FirstName,
                LastName = Register.LastName,
                Gender = Register.Gender,
                Password = Register.Password,
                DateOfBirth = Register.DateOfBirth,
                Street = Register.Street,
                City = Register.City,
                State = Register.State
            };

            await _userService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            _logger.LogInformation("New user registered: {Email}", Register.Email);
            return RedirectToPage("Login");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", Register.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
