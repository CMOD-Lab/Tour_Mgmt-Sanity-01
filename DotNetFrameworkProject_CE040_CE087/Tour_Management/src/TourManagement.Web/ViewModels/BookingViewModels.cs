using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a booking.
/// </summary>
public class BookingViewModel
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// ViewModel for creating a new booking.
/// </summary>
public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required.")]
    [StringLength(200, ErrorMessage = "Tour name must not exceed 200 characters.")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required.")]
    [StringLength(200, ErrorMessage = "Place must not exceed 200 characters.")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    [Display(Name = "Your Name")]
    public string FirstName { get; set; } = string.Empty;

    public int? TourId { get; set; }
}
