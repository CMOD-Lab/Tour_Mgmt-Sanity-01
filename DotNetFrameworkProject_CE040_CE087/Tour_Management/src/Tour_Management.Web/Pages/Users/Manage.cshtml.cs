using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Web.Pages.Users;

/// <summary>
/// Admin user management page model.
/// </summary>
public class ManageModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ManageModel> _logger;

    public IEnumerable<UserInfo> Users { get; set; } = new List<UserInfo>();

    public ManageModel(IUserService userService, ILogger<ManageModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            Users = await _userService.GetAllUsersAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            return RedirectToPage("/Admin/Login");

        try
        {
            await _userService.DeleteUserAsync(email, cancellationToken);
            TempData["SuccessMessage"] = "User deleted successfully.";
            _logger.LogInformation("User {Email} deleted by admin", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {Email}", email);
            TempData["ErrorMessage"] = "Error deleting user.";
        }

        return RedirectToPage();
    }
}
