using TourManagement.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for Booking entity.
/// </summary>
public class BookingEntityTests
{
    // ─── Constructor / Default Values ─────────────────────────────────────────

    [Fact]
    public void Booking_DefaultConstructor_ShouldSetDefaultValues()
    {
        var booking = new Booking();

        booking.BookingId.Should().Be(0);
        booking.TourName.Should().Be(string.Empty);
        booking.Place.Should().Be(string.Empty);
        booking.Email.Should().Be(string.Empty);
        booking.FirstName.Should().Be(string.Empty);
        booking.IsActive.Should().BeTrue();
        booking.UserEmail.Should().BeNull();
        booking.User.Should().BeNull();
    }

    [Fact]
    public void Booking_BookingDate_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var booking = new Booking();
        var after = DateTime.UtcNow.AddSeconds(1);

        booking.BookingDate.Should().BeAfter(before).And.BeBefore(after);
    }

    // ─── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void Booking_SetProperties_ShouldRetainValues()
    {
        var date = new DateTime(2024, 7, 20);
        var booking = new Booking
        {
            BookingId = 99,
            TourName = "Kerala Backwaters",
            Place = "Kerala",
            Email = "traveler@test.com",
            FirstName = "Priya",
            BookingDate = date,
            IsActive = false,
            UserEmail = "traveler@test.com"
        };

        booking.BookingId.Should().Be(99);
        booking.TourName.Should().Be("Kerala Backwaters");
        booking.Place.Should().Be("Kerala");
        booking.Email.Should().Be("traveler@test.com");
        booking.FirstName.Should().Be("Priya");
        booking.BookingDate.Should().Be(date);
        booking.IsActive.Should().BeFalse();
        booking.UserEmail.Should().Be("traveler@test.com");
    }

    [Fact]
    public void Booking_UserNavigation_ShouldBeNullByDefault()
    {
        var booking = new Booking();
        booking.User.Should().BeNull();
    }

    [Fact]
    public void Booking_UserNavigation_ShouldAllowAssignment()
    {
        var user = new UserInfo { Email = "u@t.com", FirstName = "Test", LastName = "User" };
        var booking = new Booking { User = user, UserEmail = "u@t.com" };

        booking.User.Should().NotBeNull();
        booking.User!.Email.Should().Be("u@t.com");
    }

    [Fact]
    public void Booking_IsActive_ShouldDefaultToTrue()
    {
        var booking = new Booking();
        booking.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Booking_IsActive_CanBeSetToFalse()
    {
        var booking = new Booking { IsActive = false };
        booking.IsActive.Should().BeFalse();
    }
}
