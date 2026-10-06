using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
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
    public UserEditViewModel Input { get; set; } = new();

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("Login");
        }

        try
        {
            var dto = await _userService.GetByIdAsync(id, cancellationToken);
            if (dto == null)
            {
                return NotFound();
            }

            Input = new UserEditViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Street = dto.Street,
                City = dto.City,
                State = dto.State
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit, ID {UserId}", id);
            return RedirectToPage("Profile");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var dto = new UserUpdateDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                DateOfBirth = Input.DateOfBirth,
                Street = Input.Street,
                City = Input.City,
                State = Input.State,
                IsActive = true
            };

            await _userService.UpdateAsync(Input.Id, dto, cancellationToken);
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again.");
            return Page();
        }
    }
}
