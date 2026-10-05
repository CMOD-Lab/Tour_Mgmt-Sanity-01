using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user profile.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    public new UserDto? User { get; set; }

    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                User = await _userService.GetByIdAsync(userId.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile");
        }
    }
}
