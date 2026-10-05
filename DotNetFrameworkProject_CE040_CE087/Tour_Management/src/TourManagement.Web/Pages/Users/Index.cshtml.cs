using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the users list (admin).
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Gets the list of users.</summary>
    public IEnumerable<UserListViewModel> Users { get; private set; } = Enumerable.Empty<UserListViewModel>();

    /// <summary>Gets the current search term.</summary>
    public string? SearchTerm { get; private set; }

    /// <summary>Initializes a new instance of <see cref="IndexModel"/>.</summary>
    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the users list page.</summary>
    public async Task<IActionResult> OnGetAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            SearchTerm = searchTerm;
            var users = string.IsNullOrWhiteSpace(searchTerm)
                ? await _userService.GetAllAsync(cancellationToken)
                : await _userService.SearchAsync(searchTerm, cancellationToken);

            Users = users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            });

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            return Page();
        }
    }
}
