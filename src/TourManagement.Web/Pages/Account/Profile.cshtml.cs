using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Account;

/// <summary>
/// Page model for the user profile page.
/// </summary>
[Authorize]
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>
    /// The current user's profile.
    /// </summary>
    public UserProfileViewModel? Profile { get; set; }

    /// <summary>
    /// Initializes a new instance of ProfileModel.
    /// </summary>
    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the profile page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Profile = new UserProfileViewModel
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
            _logger.LogError(ex, "Error loading profile page");
            return RedirectToPage("/Index");
        }
    }
}
