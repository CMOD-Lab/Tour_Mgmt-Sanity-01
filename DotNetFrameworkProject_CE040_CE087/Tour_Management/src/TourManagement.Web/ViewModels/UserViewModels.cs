using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for user login.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// ViewModel for user registration.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name must not exceed 100 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name must not exceed 100 characters.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Gender must not exceed 20 characters.")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(200, ErrorMessage = "Street must not exceed 200 characters.")]
    public string? Street { get; set; }

    [StringLength(100, ErrorMessage = "City must not exceed 100 characters.")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "State must not exceed 100 characters.")]
    public string? State { get; set; }
}

/// <summary>
/// ViewModel for displaying user profile.
/// </summary>
public class UserProfileViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Role { get; set; } = "User";
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// ViewModel for listing users (admin).
/// </summary>
public class UserListViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// ViewModel for admin login.
/// </summary>
public class AdminLoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
