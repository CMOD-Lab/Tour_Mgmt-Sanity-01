using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the user profile page.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>Gets the user profile to display.</summary>
    public UserProfileViewModel? Profile { get; private set; }

    /// <summary>Gets the recent bookings for the user.</summary>
    public IEnumerable<BookingListViewModel> RecentBookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of <see cref="ProfileModel"/>.</summary>
    public ProfileModel(IUserService userService, IBookingService bookingService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the profile page.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("Login");

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                return RedirectToPage("Login");

            Profile = new UserProfileViewModel
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

            var bookings = await _bookingService.GetByEmailAsync(email, cancellationToken);
            RecentBookings = bookings.Take(5).Select(b => new BookingListViewModel
            {
                Id = b.Id,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                BookingDate = b.BookingDate,
                IsActive = b.IsActive
            });

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for user {Email}", email);
            return RedirectToPage("/Index");
        }
    }
}
