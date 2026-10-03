namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a user in the Tour Management system.
/// </summary>
public class UserInfo
{
    /// <summary>Gets or sets the user's email address (primary key).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's first name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's last name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's gender.</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's hashed password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's date of birth.</summary>
    public DateTime Dob { get; set; }

    /// <summary>Gets or sets the user's street address.</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's city.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's state.</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Navigation property: bookings made by this user.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
