using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user profile.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    public UserProfileViewModel? Profile { get; set; }

    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("Login");
        }

        try
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdStr, out int userId))
            {
                var dto = await _userService.GetByIdAsync(userId, cancellationToken);
                if (dto != null)
                {
                    Profile = new UserProfileViewModel
                    {
                        Id = dto.Id,
                        Email = dto.Email,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Gender = dto.Gender,
                        DateOfBirth = dto.DateOfBirth,
                        Street = dto.Street,
                        City = dto.City,
                        State = dto.State,
                        IsAdmin = dto.IsAdmin
                    };
                }
            }
            else
            {
                // Admin user
                Profile = new UserProfileViewModel
                {
                    Email = userEmail,
                    FirstName = "Administrator",
                    LastName = "",
                    IsAdmin = true
                };
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for user: {Email}", userEmail);
            return RedirectToPage("/Index");
        }
    }
}
