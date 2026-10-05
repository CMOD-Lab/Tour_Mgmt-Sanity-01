namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a user entity in the domain.
/// </summary>
public class UserInfo
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the gender.</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>Gets or sets the hashed password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Gets or sets the date of birth.</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>Gets or sets the street address.</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Gets or sets the city.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the state.</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Gets or sets the role (User/Admin).</summary>
    public string Role { get; set; } = "User";

    /// <summary>Gets or sets the creation date.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last modified date.</summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Gets or sets whether the user is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property for bookings.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
