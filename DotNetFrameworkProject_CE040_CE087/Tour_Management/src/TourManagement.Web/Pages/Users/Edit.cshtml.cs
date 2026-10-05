using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for editing user profile.
/// </summary>
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public new UserEditViewModel User { get; set; } = new();

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Manually map DTO to ViewModel
            User = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Street = user.Street,
                City = user.City,
                State = user.State,
                IsActive = user.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit with ID {UserId}", id);
            return RedirectToPage("Profile");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var dto = new UserUpdateDto
            {
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Gender = User.Gender,
                DateOfBirth = User.DateOfBirth,
                Street = User.Street,
                City = User.City,
                State = User.State,
                IsActive = User.IsActive
            };

            await _userService.UpdateAsync(User.Id, dto);
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", User.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again.");
            return Page();
        }
    }
}
