namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking made by a user.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the booking identifier.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the tour name booked.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who booked.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the user who booked.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the booking date.</summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation property: the user who made this booking.</summary>
    public UserInfo? User { get; set; }
}
