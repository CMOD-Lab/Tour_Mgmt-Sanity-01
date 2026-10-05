using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a booking.
/// </summary>
public class BookingViewModel
{
    public int BookingId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for creating a new booking.
/// </summary>
public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required")]
    [MaxLength(100)]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [MaxLength(100)]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(50)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for booking list page.
/// </summary>
public class BookingListViewModel
{
    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    public string? SearchTerm { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAdminView { get; set; }
}

/// <summary>
/// ViewModel for booking details page.
/// </summary>
public class BookingDetailsViewModel
{
    public BookingViewModel Booking { get; set; } = new BookingViewModel();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
