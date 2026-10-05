using TourManagement.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for UserInfo entity.
/// </summary>
public class UserInfoEntityTests
{
    // ─── Constructor / Default Values ─────────────────────────────────────────

    [Fact]
    public void UserInfo_DefaultConstructor_ShouldSetDefaultValues()
    {
        var user = new UserInfo();

        user.Email.Should().Be(string.Empty);
        user.FirstName.Should().Be(string.Empty);
        user.LastName.Should().Be(string.Empty);
        user.Gender.Should().Be(string.Empty);
        user.Password.Should().Be(string.Empty);
        user.Street.Should().Be(string.Empty);
        user.City.Should().Be(string.Empty);
        user.State.Should().Be(string.Empty);
        user.Bookings.Should().NotBeNull();
        user.Bookings.Should().BeEmpty();
    }

    // ─── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void UserInfo_SetProperties_ShouldRetainValues()
    {
        var dob = new DateTime(1990, 6, 15);
        var user = new UserInfo
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "hashedpassword",
            Dob = dob,
            Street = "123 Main Street",
            City = "New York",
            State = "NY"
        };

        user.Email.Should().Be("john.doe@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Gender.Should().Be("Male");
        user.Password.Should().Be("hashedpassword");
        user.Dob.Should().Be(dob);
        user.Street.Should().Be("123 Main Street");
        user.City.Should().Be("New York");
        user.State.Should().Be("NY");
    }

    [Fact]
    public void UserInfo_Bookings_ShouldBeInitializedAsEmptyList()
    {
        var user = new UserInfo();

        user.Bookings.Should().NotBeNull();
        user.Bookings.Should().BeOfType<List<Booking>>();
    }

    [Fact]
    public void UserInfo_Bookings_ShouldAllowAddingItems()
    {
        var user = new UserInfo { Email = "test@test.com" };
        var booking = new Booking { BookingId = 1, TourName = "Test Tour" };

        user.Bookings.Add(booking);

        user.Bookings.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Other")]
    public void UserInfo_Gender_ShouldAcceptVariousValues(string gender)
    {
        var user = new UserInfo { Gender = gender };
        user.Gender.Should().Be(gender);
    }

    [Fact]
    public void UserInfo_Dob_ShouldAcceptVariousDates()
    {
        var dob = new DateTime(2000, 12, 31);
        var user = new UserInfo { Dob = dob };
        user.Dob.Should().Be(dob);
    }

    [Fact]
    public void UserInfo_Email_IsUsedAsPrimaryKey()
    {
        // Email is the [Key] property
        var user = new UserInfo { Email = "unique@test.com" };
        user.Email.Should().Be("unique@test.com");
    }
}
