using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the admin users list.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<UserProfileViewModel> Users { get; set; } = new List<UserProfileViewModel>();

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            Users = users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                Street = u.Street,
                City = u.City,
                State = u.State,
                IsAdmin = u.IsAdmin,
                CreatedDate = u.CreatedDate
            }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            Users = new List<UserProfileViewModel>();
            return Page();
        }
    }
}
