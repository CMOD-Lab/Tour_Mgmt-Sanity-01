namespace TourManagement.Application.DTOs;

/// <summary>
/// Data Transfer Object for Booking read operations.
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? TourId { get; set; }
    public int? UserId { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new Booking.
/// </summary>
public class BookingCreateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int? TourId { get; set; }
    public int? UserId { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing Booking.
/// </summary>
public class BookingUpdateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
