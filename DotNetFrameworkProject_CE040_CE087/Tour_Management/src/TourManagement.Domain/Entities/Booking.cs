namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking in the Tour Management system.
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    // Foreign keys (optional navigation)
    public int? TourId { get; set; }
    public Tour? Tour { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }
}
