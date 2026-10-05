using FluentAssertions;
using Tour_Management.Domain.Entities;
using Xunit;

namespace Tour_Management.UnitTests.Entities;

/// <summary>
/// Unit tests for Tour entity.
/// </summary>
public class TourEntityTests
{
    [Fact]
    public void Tour_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var tour = new Tour();

        // Assert
        tour.TourId.Should().Be(0);
        tour.TourName.Should().Be(string.Empty);
        tour.Place.Should().Be(string.Empty);
        tour.Days.Should().Be(0);
        tour.Price.Should().Be(0);
        tour.Locations.Should().Be(string.Empty);
        tour.TourInfo.Should().Be(string.Empty);
        tour.Pic.Should().BeNull();
        tour.IsActive.Should().BeTrue();
        tour.Bookings.Should().NotBeNull();
        tour.Bookings.Should().BeEmpty();
    }

    [Fact]
    public void Tour_CreatedDate_ShouldBeSetToUtcNow()
    {
        // Act
        var before = DateTime.UtcNow.AddSeconds(-1);
        var tour = new Tour();
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        tour.CreatedDate.Should().BeAfter(before);
        tour.CreatedDate.Should().BeBefore(after);
    }

    [Fact]
    public void Tour_ModifiedDate_ShouldBeNullByDefault()
    {
        // Act
        var tour = new Tour();

        // Assert
        tour.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void Tour_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var tour = new Tour
        {
            TourId = 42,
            TourName = "Goa Beach Tour",
            Place = "Goa",
            Days = 7,
            Price = 25000.50m,
            Locations = "Baga, Calangute, Anjuna",
            TourInfo = "Beautiful beach tour",
            Pic = "goa.jpg",
            IsActive = true
        };

        // Assert
        tour.TourId.Should().Be(42);
        tour.TourName.Should().Be("Goa Beach Tour");
        tour.Place.Should().Be("Goa");
        tour.Days.Should().Be(7);
        tour.Price.Should().Be(25000.50m);
        tour.Locations.Should().Be("Baga, Calangute, Anjuna");
        tour.TourInfo.Should().Be("Beautiful beach tour");
        tour.Pic.Should().Be("goa.jpg");
        tour.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Tour_Bookings_ShouldBeAddable()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Test Tour" };
        var booking = new Booking { TourId = 1, TourName = "Test Tour", Email = "test@test.com" };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        tour.Bookings.Should().HaveCount(1);
        tour.Bookings.First().Email.Should().Be("test@test.com");
    }

    [Fact]
    public void Tour_IsActive_CanBeSetToFalse()
    {
        // Arrange
        var tour = new Tour { IsActive = true };

        // Act
        tour.IsActive = false;

        // Assert
        tour.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var modifiedDate = DateTime.UtcNow;

        // Act
        tour.ModifiedDate = modifiedDate;

        // Assert
        tour.ModifiedDate.Should().Be(modifiedDate);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void Tour_Days_ShouldAcceptValidValues(int days)
    {
        // Arrange & Act
        var tour = new Tour { Days = days };

        // Assert
        tour.Days.Should().Be(days);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1000.00)]
    [InlineData(99999.99)]
    public void Tour_Price_ShouldAcceptValidValues(double price)
    {
        // Arrange & Act
        var tour = new Tour { Price = (decimal)price };

        // Assert
        tour.Price.Should().Be((decimal)price);
    }
}
