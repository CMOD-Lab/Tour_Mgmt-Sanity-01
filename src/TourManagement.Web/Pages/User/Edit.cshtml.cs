using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.User;

/// <summary>
/// Page model for editing user profile.
/// </summary>
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserEditViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null) return RedirectToPage("/User/Login");

            // Manually map DTO to ViewModel
            Input = new UserEditViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Dob = user.Dob,
                Street = user.Street,
                City = user.City,
                State = user.State
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit, email {Email}.", email);
            return RedirectToPage("/User/Profile");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Manually map ViewModel to DTO
            var dto = new UserUpdateDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            await _userService.UpdateAsync(email, dto, cancellationToken);

            // Update session with new first name
            HttpContext.Session.SetString("UserFirstName", Input.FirstName);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToPage("/User/Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for {Email}.", email);
            TempData["Error"] = "An error occurred while updating your profile.";
            return Page();
        }
    }
}
