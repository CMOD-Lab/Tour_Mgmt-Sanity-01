using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking made by a user.
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier for the booking.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookingId { get; set; }

    /// <summary>
    /// Name of the tour booked.
    /// </summary>
    [MaxLength(100)]
    public string TourName { get; set; } = string.Empty;

    /// <summary>
    /// Place/destination of the tour.
    /// </summary>
    [MaxLength(100)]
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Email of the user who made the booking.
    /// </summary>
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name of the person who made the booking.
    /// </summary>
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Date the booking was created.
    /// </summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether the booking is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Foreign key to UserInfo.
    /// </summary>
    [ForeignKey(nameof(User))]
    public string? UserEmail { get; set; }

    /// <summary>
    /// Navigation property - user who made the booking.
    /// </summary>
    public UserInfo? User { get; set; }
}
