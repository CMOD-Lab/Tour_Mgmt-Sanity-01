using FluentAssertions;
using Tour_Management.Web.ViewModels;
using Xunit;

namespace Tour_Management.UnitTests.ViewModels;

/// <summary>
/// Unit tests for Booking ViewModels.
/// </summary>
public class BookingViewModelTests
{
    // ─── BookingViewModel ──────────────────────────────────────────────────────

    [Fact]
    public void BookingViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new BookingViewModel();

        // Assert
        vm.TourId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.CreatedDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void BookingViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var createdDate = new DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        var vm = new BookingViewModel
        {
            TourId = 5,
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@test.com",
            FirstName = "John",
            CreatedDate = createdDate
        };

        // Assert
        vm.TourId.Should().Be(5);
        vm.TourName.Should().Be("Goa Tour");
        vm.Place.Should().Be("Goa");
        vm.Email.Should().Be("user@test.com");
        vm.FirstName.Should().Be("John");
        vm.CreatedDate.Should().Be(createdDate);
    }

    // ─── CreateBookingViewModel ────────────────────────────────────────────────

    [Fact]
    public void CreateBookingViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CreateBookingViewModel();

        // Assert
        vm.FirstName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.TourName.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
    }

    [Fact]
    public void CreateBookingViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var vm = new CreateBookingViewModel
        {
            FirstName = "Alice",
            Place = "Mumbai",
            TourName = "Kerala Tour",
            Email = "9876543210"
        };

        // Assert
        vm.FirstName.Should().Be("Alice");
        vm.Place.Should().Be("Mumbai");
        vm.TourName.Should().Be("Kerala Tour");
        vm.Email.Should().Be("9876543210");
    }

    [Theory]
    [InlineData("Alice", "Mumbai", "Goa Tour", "9876543210")]
    [InlineData("Bob", "Delhi", "Kashmir Tour", "1234567890")]
    [InlineData("Charlie", "Bangalore", "Kerala Tour", "5555555555")]
    public void CreateBookingViewModel_WithVariousInputs_ShouldRetainValues(
        string firstName, string place, string tourName, string email)
    {
        // Arrange & Act
        var vm = new CreateBookingViewModel
        {
            FirstName = firstName,
            Place = place,
            TourName = tourName,
            Email = email
        };

        // Assert
        vm.FirstName.Should().Be(firstName);
        vm.Place.Should().Be(place);
        vm.TourName.Should().Be(tourName);
        vm.Email.Should().Be(email);
    }

    [Fact]
    public void BookingViewModel_CreatedDate_ShouldBeSettable()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var vm = new BookingViewModel();

        // Act
        vm.CreatedDate = date;

        // Assert
        vm.CreatedDate.Should().Be(date);
    }
}
