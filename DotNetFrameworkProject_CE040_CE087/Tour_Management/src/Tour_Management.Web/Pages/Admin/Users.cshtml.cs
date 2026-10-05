using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Admin;

/// <summary>
/// Page model for admin user management.
/// </summary>
public class UsersModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersModel> _logger;

    public IEnumerable<UserInfo> Users { get; set; } = new List<UserInfo>();

    public UsersModel(IUserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
        {
            return RedirectToPage("Login");
        }

        try
        {
            Users = await _userService.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users for admin");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
        {
            return RedirectToPage("Login");
        }

        try
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            TempData["ErrorMessage"] = "Error deleting user.";
        }

        return RedirectToPage();
    }
}
