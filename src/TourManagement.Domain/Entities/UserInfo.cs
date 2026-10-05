using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a user in the Tour Management system.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Primary key - user's email address.
    /// </summary>
    [Key]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's gender (Male/Female).
    /// </summary>
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// User's hashed password.
    /// </summary>
    [MaxLength(256)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's date of birth.
    /// </summary>
    public DateTime Dob { get; set; }

    /// <summary>
    /// User's street address.
    /// </summary>
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// User's city.
    /// </summary>
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// User's state.
    /// </summary>
    [MaxLength(50)]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property - bookings made by this user.
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
