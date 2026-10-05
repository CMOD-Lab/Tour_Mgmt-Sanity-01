using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour in a list.
/// </summary>
public class TourListViewModel
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string? PictureName { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for displaying tour details.
/// </summary>
public class TourDetailsViewModel
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PictureName { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
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

    [Required(ErrorMessage = "Locations are required.")]
    [StringLength(500, ErrorMessage = "Locations must not exceed 500 characters.")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour information is required.")]
    [StringLength(2000, ErrorMessage = "Tour information must not exceed 2000 characters.")]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "Locations are required.")]
    [StringLength(500, ErrorMessage = "Locations must not exceed 500 characters.")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour information is required.")]
    [StringLength(2000, ErrorMessage = "Tour information must not exceed 2000 characters.")]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    public string? ExistingPictureName { get; set; }

    [Display(Name = "New Tour Picture")]
    public IFormFile? PictureFile { get; set; }

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// ViewModel for deleting a tour (confirmation page).
/// </summary>
public class TourDeleteViewModel
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
}
