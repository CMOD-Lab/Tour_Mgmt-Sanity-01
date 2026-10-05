using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>
/// ViewModel for user login.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for user registration.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    [Display(Name = "Gender")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
    [Display(Name = "Street")]
    public string Street { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
    [Display(Name = "State")]
    public string State { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for displaying user profile.
/// </summary>
public class UserProfileViewModel
{
    public string Email { get; set; } = string.Empty;

    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Gender")]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Street")]
    public string Street { get; set; } = string.Empty;

    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "State")]
    public string State { get; set; } = string.Empty;
}
