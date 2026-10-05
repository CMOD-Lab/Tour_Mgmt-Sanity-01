using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>Page model for user details.</summary>
public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DetailsModel> _logger;

    public new UserProfileViewModel? User { get; private set; }

    public DetailsModel(IUserService userService, ILogger<DetailsModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user is null) return NotFound();

            User = new UserProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Street = user.Street,
                City = user.City,
                State = user.State,
                Role = user.Role
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for ID {UserId}", id);
            return RedirectToPage("Index");
        }
    }
}
