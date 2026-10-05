using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour package in the Tour Management system.
/// </summary>
public class Tour
{
    /// <summary>
    /// Unique identifier for the tour.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TourId { get; set; }

    /// <summary>
    /// Name of the tour.
    /// </summary>
    [MaxLength(100)]
    public string TourName { get; set; } = string.Empty;

    /// <summary>
    /// Main place/destination of the tour.
    /// </summary>
    [MaxLength(100)]
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Number of days for the tour.
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Price of the tour.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Locations covered in the tour.
    /// </summary>
    [MaxLength(200)]
    public string Locations { get; set; } = string.Empty;

    /// <summary>
    /// Detailed information about the tour.
    /// </summary>
    [MaxLength(500)]
    public string TourInfo { get; set; } = string.Empty;

    /// <summary>
    /// Picture filename for the tour.
    /// </summary>
    [MaxLength(200)]
    public string? Pic { get; set; }

    /// <summary>
    /// Whether the tour is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date the tour was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property - bookings for this tour.
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
