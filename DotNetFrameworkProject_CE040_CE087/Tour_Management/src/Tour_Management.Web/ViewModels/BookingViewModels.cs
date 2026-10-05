using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a booking.
/// </summary>
public class BookingViewModel
{
    public int TourId { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Booking Date")]
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// ViewModel for creating a new booking.
/// </summary>
public class CreateBookingViewModel
{
    [Required(ErrorMessage = "Your name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Your Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    [Display(Name = "Your City")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Mobile Number")]
    public string Email { get; set; } = string.Empty;
}
