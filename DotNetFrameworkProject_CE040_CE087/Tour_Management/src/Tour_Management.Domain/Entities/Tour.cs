namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a tour entity in the domain.
/// </summary>
public class Tour
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the tour name.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of days.</summary>
    public int Days { get; set; }

    /// <summary>Gets or sets the price.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the locations covered.</summary>
    public string Locations { get; set; } = string.Empty;

    /// <summary>Gets or sets the tour information/description.</summary>
    public string TourInfo { get; set; } = string.Empty;

    /// <summary>Gets or sets the picture filename.</summary>
    public string? Pic { get; set; }

    /// <summary>Gets or sets the creation date.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last modified date.</summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Gets or sets whether the tour is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property for bookings.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
