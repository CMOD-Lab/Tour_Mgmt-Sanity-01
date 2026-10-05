using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin user management.
/// </summary>
public class UsersModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersModel> _logger;

    public IEnumerable<UserDto> Users { get; set; } = Enumerable.Empty<UserDto>();
    public bool IsAdmin { get; set; }

    public UsersModel(IUserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

        if (IsAdmin)
        {
            try
            {
                Users = await _userService.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users for admin");
                Users = Enumerable.Empty<UserDto>();
            }
        }
    }
}
