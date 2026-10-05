using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a booking.
/// </summary>
public class BookingViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for creating a booking (Order).
/// </summary>
public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
    [Display(Name = "Place/City")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    public int? TourId { get; set; }
}
