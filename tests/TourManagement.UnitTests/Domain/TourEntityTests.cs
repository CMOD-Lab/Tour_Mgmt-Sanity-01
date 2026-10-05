using TourManagement.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for Tour entity.
/// </summary>
public class TourEntityTests
{
    // ─── Constructor / Default Values ─────────────────────────────────────────

    [Fact]
    public void Tour_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var tour = new Tour();

        // Assert
        tour.TourId.Should().Be(0);
        tour.TourName.Should().Be(string.Empty);
        tour.Place.Should().Be(string.Empty);
        tour.Days.Should().Be(0);
        tour.Price.Should().Be(0m);
        tour.Locations.Should().Be(string.Empty);
        tour.TourInfo.Should().Be(string.Empty);
        tour.Pic.Should().BeNull();
        tour.IsActive.Should().BeTrue();
        tour.Bookings.Should().NotBeNull();
        tour.Bookings.Should().BeEmpty();
    }

    [Fact]
    public void Tour_CreatedDate_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var tour = new Tour();
        var after = DateTime.UtcNow.AddSeconds(1);

        tour.CreatedDate.Should().BeAfter(before).And.BeBefore(after);
    }

    // ─── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void Tour_SetProperties_ShouldRetainValues()
    {
        var tour = new Tour
        {
            TourId = 42,
            TourName = "Himalayan Trek",
            Place = "Himachal Pradesh",
            Days = 10,
            Price = 25000.50m,
            Locations = "Manali, Spiti, Kaza",
            TourInfo = "High altitude adventure",
            Pic = "himalaya.jpg",
            IsActive = false,
            CreatedDate = new DateTime(2024, 3, 15)
        };

        tour.TourId.Should().Be(42);
        tour.TourName.Should().Be("Himalayan Trek");
        tour.Place.Should().Be("Himachal Pradesh");
        tour.Days.Should().Be(10);
        tour.Price.Should().Be(25000.50m);
        tour.Locations.Should().Be("Manali, Spiti, Kaza");
        tour.TourInfo.Should().Be("High altitude adventure");
        tour.Pic.Should().Be("himalaya.jpg");
        tour.IsActive.Should().BeFalse();
        tour.CreatedDate.Should().Be(new DateTime(2024, 3, 15));
    }

    [Fact]
    public void Tour_Bookings_ShouldBeInitializedAsEmptyList()
    {
        var tour = new Tour();

        tour.Bookings.Should().NotBeNull();
        tour.Bookings.Should().BeOfType<List<Booking>>();
    }

    [Fact]
    public void Tour_Bookings_ShouldAllowAddingItems()
    {
        var tour = new Tour { TourId = 1, TourName = "Test Tour" };
        var booking = new Booking { BookingId = 1, TourName = "Test Tour" };

        tour.Bookings.Add(booking);

        tour.Bookings.Should().HaveCount(1);
        tour.Bookings.First().BookingId.Should().Be(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void Tour_Days_ShouldAcceptVariousValues(int days)
    {
        var tour = new Tour { Days = days };
        tour.Days.Should().Be(days);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(100.00)]
    [InlineData(99999.99)]
    public void Tour_Price_ShouldAcceptVariousDecimalValues(decimal price)
    {
        var tour = new Tour { Price = price };
        tour.Price.Should().Be(price);
    }

    [Fact]
    public void Tour_Pic_ShouldBeNullable()
    {
        var tourWithPic = new Tour { Pic = "photo.jpg" };
        var tourWithoutPic = new Tour { Pic = null };

        tourWithPic.Pic.Should().Be("photo.jpg");
        tourWithoutPic.Pic.Should().BeNull();
    }
}
