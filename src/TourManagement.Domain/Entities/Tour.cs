namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour package in the Tour Management system.
/// </summary>
public class Tour
{
    /// <summary>Gets or sets the tour identifier.</summary>
    public int TourId { get; set; }

    /// <summary>Gets or sets the tour name.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the main place/destination.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of days for the tour.</summary>
    public int Days { get; set; }

    /// <summary>Gets or sets the price of the tour.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the locations covered in the tour.</summary>
    public string Locations { get; set; } = string.Empty;

    /// <summary>Gets or sets detailed information about the tour.</summary>
    public string TourInfo { get; set; } = string.Empty;

    /// <summary>Gets or sets the picture filename for the tour.</summary>
    public string? Pic { get; set; }

    /// <summary>Navigation property: bookings for this tour.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
