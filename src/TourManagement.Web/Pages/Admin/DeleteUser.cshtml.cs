using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for deleting a user (admin only).
/// </summary>
public class DeleteUserModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteUserModel> _logger;

    public DeleteUserModel(IUserService userService, ILogger<DeleteUserModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public UserDto? UserToDelete { get; set; }

    public async Task<IActionResult> OnGetAsync(string email, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            UserToDelete = await _userService.GetByEmailAsync(email, cancellationToken);
            if (UserToDelete == null) return NotFound();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for delete, email {Email}.", email);
            return RedirectToPage("/Admin/Dashboard");
        }
    }

    public async Task<IActionResult> OnPostAsync(string email, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            await _userService.DeleteAsync(email, cancellationToken);
            TempData["Success"] = $"User {email} deleted successfully!";
            return RedirectToPage("/Admin/Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}.", email);
            TempData["Error"] = "An error occurred while deleting the user.";
            return RedirectToPage("/Admin/Dashboard");
        }
    }
}
