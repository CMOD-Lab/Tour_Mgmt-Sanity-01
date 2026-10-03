using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.User;

/// <summary>
/// Page model for viewing user profile.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public UserDto? UserProfile { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/User/Login");
        }

        try
        {
            UserProfile = await _userService.GetByEmailAsync(email, cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for {Email}.", email);
            TempData["Error"] = "An error occurred while loading your profile.";
            return Page();
        }
    }
}
