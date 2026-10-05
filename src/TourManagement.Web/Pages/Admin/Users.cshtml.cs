using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for managing users (admin).
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersModel> _logger;

    /// <summary>
    /// All users.
    /// </summary>
    public IEnumerable<UserProfileViewModel> Users { get; set; } = new List<UserProfileViewModel>();

    /// <summary>
    /// Current search term.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Initializes a new instance of UsersModel.
    /// </summary>
    public UsersModel(IUserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the users management page.
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            IEnumerable<TourManagement.Application.DTOs.UserDto> users;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                users = await _userService.SearchAsync(SearchTerm);
            }
            else
            {
                users = await _userService.GetAllAsync();
            }

            Users = users.Select(u => new UserProfileViewModel
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                Dob = u.Dob,
                Street = u.Street,
                City = u.City,
                State = u.State
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            Users = new List<UserProfileViewModel>();
        }
    }
}
