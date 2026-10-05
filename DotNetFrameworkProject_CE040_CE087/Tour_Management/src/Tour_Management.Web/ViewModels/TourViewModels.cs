using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour.
/// </summary>
public class TourViewModel
{
    public int TourId { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Days")]
    public int Days { get; set; }

    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Price { get; set; }

    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Image")]
    public string? Pic { get; set; }
}

/// <summary>
/// ViewModel for creating a new tour.
/// </summary>
public class CreateTourViewModel
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
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Tour info cannot exceed 1000 characters")]
    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Tour Image")]
    public IFormFile? PicFile { get; set; }
}

/// <summary>
/// ViewModel for editing an existing tour.
/// </summary>
public class EditTourViewModel
{
    public int TourId { get; set; }

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
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Tour info cannot exceed 1000 characters")]
    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;

    public string? Pic { get; set; }

    [Display(Name = "New Tour Image")]
    public IFormFile? PicFile { get; set; }
}
