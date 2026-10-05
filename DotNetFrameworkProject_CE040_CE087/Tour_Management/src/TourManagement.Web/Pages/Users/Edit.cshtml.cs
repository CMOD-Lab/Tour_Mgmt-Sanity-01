using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

/// <summary>Page model for editing a user.</summary>
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public UserEditInputModel UserInput { get; set; } = new();

    public EditModel(IUserService userService, ILogger<EditModel> logger)
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

        UserInput = new UserEditInputModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Street = user.Street,
            City = user.City,
            State = user.State,
            IsActive = user.IsActive
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        if (!ModelState.IsValid) return Page();

        try
        {
            var userUpdate = new UserInfo
            {
                FirstName = UserInput.FirstName,
                LastName = UserInput.LastName,
                Gender = UserInput.Gender,
                DateOfBirth = UserInput.DateOfBirth,
                Street = UserInput.Street,
                City = UserInput.City,
                State = UserInput.State,
                IsActive = UserInput.IsActive,
                ModifiedBy = HttpContext.Session.GetString("UserEmail") ?? "admin"
            };

            await _userService.UpdateAsync(UserInput.Id, userUpdate, cancellationToken);
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", UserInput.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
            return Page();
        }
    }
}

/// <summary>Input model for editing a user.</summary>
public class UserEditInputModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Gender { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(200)]
    public string? Street { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
