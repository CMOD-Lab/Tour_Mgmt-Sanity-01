namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour package in the system.
/// </summary>
public class Tour
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the tour name.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the main place/destination.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of days for the tour.</summary>
    public int Days { get; set; }

    /// <summary>Gets or sets the price of the tour.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the locations covered.</summary>
    public string Locations { get; set; } = string.Empty;

    /// <summary>Gets or sets the tour information/description.</summary>
    public string TourInfo { get; set; } = string.Empty;

    /// <summary>Gets or sets the picture filename.</summary>
    public string? PictureName { get; set; }

    /// <summary>Gets or sets the creation date.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last modified date.</summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Gets or sets whether the tour is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the creator.</summary>
    public string CreatedBy { get; set; } = "system";

    /// <summary>Gets or sets the last modifier.</summary>
    public string? ModifiedBy { get; set; }

    /// <summary>Navigation property for bookings.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
