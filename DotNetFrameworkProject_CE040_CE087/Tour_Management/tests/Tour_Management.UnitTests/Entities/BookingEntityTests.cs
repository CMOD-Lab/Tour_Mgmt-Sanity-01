using FluentAssertions;
using Tour_Management.Domain.Entities;
using Xunit;

namespace Tour_Management.UnitTests.Entities;

/// <summary>
/// Unit tests for Booking entity.
/// </summary>
public class BookingEntityTests
{
    [Fact]
    public void Booking_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var booking = new Booking();

        // Assert
        booking.TourId.Should().Be(0);
        booking.TourName.Should().Be(string.Empty);
        booking.Place.Should().Be(string.Empty);
        booking.Email.Should().Be(string.Empty);
        booking.FirstName.Should().Be(string.Empty);
        booking.IsActive.Should().BeTrue();
        booking.Tour.Should().BeNull();
        booking.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void Booking_CreatedDate_ShouldBeSetToUtcNow()
    {
        // Act
        var before = DateTime.UtcNow.AddSeconds(-1);
        var booking = new Booking();
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        booking.CreatedDate.Should().BeAfter(before);
        booking.CreatedDate.Should().BeBefore(after);
    }

    [Fact]
    public void Booking_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var booking = new Booking
        {
            TourId = 5,
            TourName = "Kerala Tour",
            Place = "Kochi",
            Email = "user@test.com",
            FirstName = "John",
            IsActive = true
        };

        // Assert
        booking.TourId.Should().Be(5);
        booking.TourName.Should().Be("Kerala Tour");
        booking.Place.Should().Be("Kochi");
        booking.Email.Should().Be("user@test.com");
        booking.FirstName.Should().Be("John");
        booking.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Booking_Tour_NavigationProperty_CanBeSet()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Test Tour" };
        var booking = new Booking { TourId = 1 };

        // Act
        booking.Tour = tour;

        // Assert
        booking.Tour.Should().NotBeNull();
        booking.Tour!.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public void Booking_IsActive_CanBeSetToFalse()
    {
        // Arrange
        var booking = new Booking { IsActive = true };

        // Act
        booking.IsActive = false;

        // Assert
        booking.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var modifiedDate = DateTime.UtcNow;

        // Act
        booking.ModifiedDate = modifiedDate;

        // Assert
        booking.ModifiedDate.Should().Be(modifiedDate);
    }

    [Theory]
    [InlineData("alice@example.com")]
    [InlineData("bob.smith@company.org")]
    [InlineData("test+tag@domain.co.uk")]
    public void Booking_Email_ShouldAcceptVariousFormats(string email)
    {
        // Arrange & Act
        var booking = new Booking { Email = email };

        // Assert
        booking.Email.Should().Be(email);
    }
}
