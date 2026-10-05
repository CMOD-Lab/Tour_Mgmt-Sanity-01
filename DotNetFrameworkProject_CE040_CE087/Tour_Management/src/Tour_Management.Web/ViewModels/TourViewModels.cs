using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour.
/// </summary>
public class TourViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Days")]
    public int Days { get; set; }

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Picture")]
    public string? Pic { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for creating a tour.
/// </summary>
public class TourCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Tour information cannot exceed 2000 characters")]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Tour Picture")]
    public IFormFile? PicFile { get; set; }
}

/// <summary>
/// ViewModel for editing a tour.
/// </summary>
public class TourEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Tour information cannot exceed 2000 characters")]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Current Picture")]
    public string? ExistingPic { get; set; }

    [Display(Name = "New Tour Picture")]
    public IFormFile? PicFile { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
