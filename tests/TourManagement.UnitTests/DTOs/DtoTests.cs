using TourManagement.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.DTOs;

/// <summary>
/// Unit tests for Tour DTOs.
/// </summary>
public class TourDtoTests
{
    // ─── TourDto ──────────────────────────────────────────────────────────────

    [Fact]
    public void TourDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new TourDto();

        dto.TourId.Should().Be(0);
        dto.TourName.Should().Be(string.Empty);
        dto.Place.Should().Be(string.Empty);
        dto.Days.Should().Be(0);
        dto.Price.Should().Be(0m);
        dto.Locations.Should().Be(string.Empty);
        dto.TourInfo.Should().Be(string.Empty);
        dto.Pic.Should().BeNull();
        dto.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TourDto_SetProperties_ShouldRetainValues()
    {
        var dto = new TourDto
        {
            TourId = 1,
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Days = 7,
            Price = 15000m,
            Locations = "Srinagar, Gulmarg",
            TourInfo = "Beautiful Kashmir",
            Pic = "kashmir.jpg",
            IsActive = true,
            CreatedDate = new DateTime(2024, 1, 1)
        };

        dto.TourId.Should().Be(1);
        dto.TourName.Should().Be("Kashmir Tour");
        dto.Place.Should().Be("Kashmir");
        dto.Days.Should().Be(7);
        dto.Price.Should().Be(15000m);
        dto.Locations.Should().Be("Srinagar, Gulmarg");
        dto.TourInfo.Should().Be("Beautiful Kashmir");
        dto.Pic.Should().Be("kashmir.jpg");
        dto.IsActive.Should().BeTrue();
        dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
    }

    // ─── TourCreateDto ────────────────────────────────────────────────────────

    [Fact]
    public void TourCreateDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new TourCreateDto();

        dto.TourName.Should().Be(string.Empty);
        dto.Place.Should().Be(string.Empty);
        dto.Days.Should().Be(0);
        dto.Price.Should().Be(0m);
        dto.Locations.Should().Be(string.Empty);
        dto.TourInfo.Should().Be(string.Empty);
        dto.Pic.Should().BeNull();
    }

    [Fact]
    public void TourCreateDto_SetProperties_ShouldRetainValues()
    {
        var dto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "Delhi",
            Days = 3,
            Price = 8000m,
            Locations = "Delhi, Agra",
            TourInfo = "Golden Triangle",
            Pic = "delhi.jpg"
        };

        dto.TourName.Should().Be("New Tour");
        dto.Place.Should().Be("Delhi");
        dto.Days.Should().Be(3);
        dto.Price.Should().Be(8000m);
        dto.Locations.Should().Be("Delhi, Agra");
        dto.TourInfo.Should().Be("Golden Triangle");
        dto.Pic.Should().Be("delhi.jpg");
    }

    // ─── TourUpdateDto ────────────────────────────────────────────────────────

    [Fact]
    public void TourUpdateDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new TourUpdateDto();

        dto.TourName.Should().Be(string.Empty);
        dto.Place.Should().Be(string.Empty);
        dto.Days.Should().Be(0);
        dto.Price.Should().Be(0m);
        dto.Locations.Should().Be(string.Empty);
        dto.TourInfo.Should().Be(string.Empty);
        dto.Pic.Should().BeNull();
        dto.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TourUpdateDto_SetProperties_ShouldRetainValues()
    {
        var dto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Mumbai",
            Days = 4,
            Price = 11000m,
            Locations = "Mumbai, Pune",
            TourInfo = "City tour",
            Pic = "mumbai.jpg",
            IsActive = true
        };

        dto.TourName.Should().Be("Updated Tour");
        dto.IsActive.Should().BeTrue();
    }
}

/// <summary>
/// Unit tests for User DTOs.
/// </summary>
public class UserDtoTests
{
    [Fact]
    public void UserDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new UserDto();

        dto.Email.Should().Be(string.Empty);
        dto.FirstName.Should().Be(string.Empty);
        dto.LastName.Should().Be(string.Empty);
        dto.Gender.Should().Be(string.Empty);
        dto.Street.Should().Be(string.Empty);
        dto.City.Should().Be(string.Empty);
        dto.State.Should().Be(string.Empty);
    }

    [Fact]
    public void UserDto_SetProperties_ShouldRetainValues()
    {
        var dto = new UserDto
        {
            Email = "alice@test.com",
            FirstName = "Alice",
            LastName = "Smith",
            Gender = "Female",
            Dob = new DateTime(1990, 1, 15),
            Street = "1 Main St",
            City = "New York",
            State = "NY"
        };

        dto.Email.Should().Be("alice@test.com");
        dto.FirstName.Should().Be("Alice");
        dto.LastName.Should().Be("Smith");
        dto.Gender.Should().Be("Female");
        dto.Dob.Should().Be(new DateTime(1990, 1, 15));
        dto.Street.Should().Be("1 Main St");
        dto.City.Should().Be("New York");
        dto.State.Should().Be("NY");
    }

    [Fact]
    public void UserCreateDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new UserCreateDto();

        dto.Email.Should().Be(string.Empty);
        dto.Password.Should().Be(string.Empty);
        dto.FirstName.Should().Be(string.Empty);
    }

    [Fact]
    public void UserCreateDto_SetProperties_ShouldRetainValues()
    {
        var dto = new UserCreateDto
        {
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User",
            Gender = "Male",
            Password = "Password123",
            Dob = new DateTime(1995, 3, 20),
            Street = "5 Elm St",
            City = "Chicago",
            State = "IL"
        };

        dto.Email.Should().Be("new@test.com");
        dto.Password.Should().Be("Password123");
        dto.FirstName.Should().Be("New");
    }

    [Fact]
    public void UserUpdateDto_PasswordIsNullable()
    {
        var dto = new UserUpdateDto { Password = null };
        dto.Password.Should().BeNull();
    }

    [Fact]
    public void UserLoginDto_SetProperties_ShouldRetainValues()
    {
        var dto = new UserLoginDto { Email = "login@test.com", Password = "pass" };

        dto.Email.Should().Be("login@test.com");
        dto.Password.Should().Be("pass");
    }
}

/// <summary>
/// Unit tests for Booking DTOs.
/// </summary>
public class BookingDtoTests
{
    [Fact]
    public void BookingDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new BookingDto();

        dto.BookingId.Should().Be(0);
        dto.TourName.Should().Be(string.Empty);
        dto.Place.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.FirstName.Should().Be(string.Empty);
        dto.IsActive.Should().BeFalse();
    }

    [Fact]
    public void BookingDto_SetProperties_ShouldRetainValues()
    {
        var dto = new BookingDto
        {
            BookingId = 1,
            TourName = "Kerala Tour",
            Place = "Kerala",
            Email = "user@test.com",
            FirstName = "Priya",
            BookingDate = new DateTime(2024, 8, 10),
            IsActive = true
        };

        dto.BookingId.Should().Be(1);
        dto.TourName.Should().Be("Kerala Tour");
        dto.Place.Should().Be("Kerala");
        dto.Email.Should().Be("user@test.com");
        dto.FirstName.Should().Be("Priya");
        dto.BookingDate.Should().Be(new DateTime(2024, 8, 10));
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void BookingCreateDto_DefaultConstructor_ShouldSetDefaultValues()
    {
        var dto = new BookingCreateDto();

        dto.TourName.Should().Be(string.Empty);
        dto.Place.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.FirstName.Should().Be(string.Empty);
    }

    [Fact]
    public void BookingCreateDto_SetProperties_ShouldRetainValues()
    {
        var dto = new BookingCreateDto
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "traveler@test.com",
            FirstName = "John"
        };

        dto.TourName.Should().Be("Goa Tour");
        dto.Place.Should().Be("Goa");
        dto.Email.Should().Be("traveler@test.com");
        dto.FirstName.Should().Be("John");
    }

    [Fact]
    public void BookingUpdateDto_SetProperties_ShouldRetainValues()
    {
        var dto = new BookingUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Email = "u@t.com",
            FirstName = "Updated",
            IsActive = false
        };

        dto.TourName.Should().Be("Updated Tour");
        dto.IsActive.Should().BeFalse();
    }
}
