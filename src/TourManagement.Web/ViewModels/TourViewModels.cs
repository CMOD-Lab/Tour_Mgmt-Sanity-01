using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour.
/// </summary>
public class TourViewModel
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? Pic { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// ViewModel for creating a new tour.
/// </summary>
public class TourCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required")]
    [MaxLength(100)]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [MaxLength(100)]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations are required")]
    [MaxLength(200)]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour information is required")]
    [MaxLength(500)]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Tour Picture")]
    public IFormFile? PicFile { get; set; }

    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for editing an existing tour.
/// </summary>
public class TourEditViewModel
{
    public int TourId { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [MaxLength(100)]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [MaxLength(100)]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations are required")]
    [MaxLength(200)]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour information is required")]
    [MaxLength(500)]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    public string? ExistingPic { get; set; }

    [Display(Name = "New Tour Picture")]
    public IFormFile? PicFile { get; set; }

    public bool IsActive { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for tour list page.
/// </summary>
public class TourListViewModel
{
    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();
    public string? SearchTerm { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for tour details page.
/// </summary>
public class TourDetailsViewModel
{
    public TourViewModel Tour { get; set; } = new TourViewModel();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
