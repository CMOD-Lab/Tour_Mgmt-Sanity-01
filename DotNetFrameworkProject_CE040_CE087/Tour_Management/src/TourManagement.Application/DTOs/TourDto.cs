namespace TourManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Tour read operations.
/// </summary>
public class TourDto
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
/// Data Transfer Object for creating a new Tour.
/// </summary>
public class TourCreateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string? Locations { get; set; }
    public string? TourInfo { get; set; }
    public string? PicturePath { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing Tour.
/// </summary>
public class TourUpdateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string? Locations { get; set; }
    public string? TourInfo { get; set; }
    public string? PicturePath { get; set; }
    public bool IsActive { get; set; }
}
