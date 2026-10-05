using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

/// <summary>Page model for deleting a user.</summary>
public class DeleteModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteModel> _logger;

    [BindProperty]
    public int UserId { get; set; }

    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }

    public DeleteModel(IUserService userService, ILogger<DeleteModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user is null) return NotFound();

        UserId = user.Id;
        UserName = $"{user.FirstName} {user.LastName}";
        UserEmail = user.Email;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            await _userService.DeleteAsync(UserId, cancellationToken);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", UserId);
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            return RedirectToPage("Index");
        }
    }
}
