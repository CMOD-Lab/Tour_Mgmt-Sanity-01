using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour.
/// </summary>
public class TourViewModel
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string? Locations { get; set; }
    public string? TourInfo { get; set; }
    public string? PicturePath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// ViewModel for creating a new tour.
/// </summary>
public class TourCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required.")]
    [StringLength(200, ErrorMessage = "Tour name must not exceed 200 characters.")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required.")]
    [StringLength(200, ErrorMessage = "Place must not exceed 200 characters.")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required.")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365.")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations must not exceed 500 characters.")]
    public string? Locations { get; set; }

    [StringLength(2000, ErrorMessage = "Tour info must not exceed 2000 characters.")]
    [Display(Name = "Tour Information")]
    public string? TourInfo { get; set; }

    [Display(Name = "Tour Picture")]
    public IFormFile? PictureFile { get; set; }
}

/// <summary>
/// ViewModel for editing an existing tour.
/// </summary>
public class TourEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour name is required.")]
    [StringLength(200, ErrorMessage = "Tour name must not exceed 200 characters.")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required.")]
    [StringLength(200, ErrorMessage = "Place must not exceed 200 characters.")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required.")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365.")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations must not exceed 500 characters.")]
    public string? Locations { get; set; }

    [StringLength(2000, ErrorMessage = "Tour info must not exceed 2000 characters.")]
    [Display(Name = "Tour Information")]
    public string? TourInfo { get; set; }

    public string? ExistingPicturePath { get; set; }

    [Display(Name = "New Tour Picture")]
    public IFormFile? PictureFile { get; set; }

    public bool IsActive { get; set; }
}
