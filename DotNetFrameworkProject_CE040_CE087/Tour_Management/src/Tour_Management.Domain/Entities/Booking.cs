namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a booking entity in the domain.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the tour name.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who booked.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the booker.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the tour foreign key.</summary>
    public int? TourId { get; set; }

    /// <summary>Gets or sets the user foreign key.</summary>
    public int? UserId { get; set; }

    /// <summary>Gets or sets the booking date.</summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last modified date.</summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Gets or sets whether the booking is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property for tour.</summary>
    public Tour? Tour { get; set; }

    /// <summary>Navigation property for user.</summary>
    public UserInfo? User { get; set; }
}
