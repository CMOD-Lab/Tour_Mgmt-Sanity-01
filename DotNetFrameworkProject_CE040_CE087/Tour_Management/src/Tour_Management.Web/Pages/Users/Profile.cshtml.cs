using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>
/// User profile page model.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    public UserProfileViewModel? UserProfile { get; set; }

    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Users/Login");

        try
        {
            var user = await _userService.GetUserByEmailAsync(email, cancellationToken);
            if (user != null)
            {
                // Manual ViewModel mapping
                UserProfile = new UserProfileViewModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Street = user.Street,
                    City = user.City,
                    State = user.State
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for user: {Email}", email);
        }

        return Page();
    }
}
