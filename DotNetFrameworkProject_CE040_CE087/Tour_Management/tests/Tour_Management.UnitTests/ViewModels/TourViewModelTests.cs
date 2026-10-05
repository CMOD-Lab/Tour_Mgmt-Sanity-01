using FluentAssertions;
using Tour_Management.Web.ViewModels;
using Xunit;

namespace Tour_Management.UnitTests.ViewModels;

/// <summary>
/// Unit tests for Tour ViewModels.
/// </summary>
public class TourViewModelTests
{
    // ─── TourViewModel ─────────────────────────────────────────────────────────

    [Fact]
    public void TourViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new TourViewModel();

        // Assert
        vm.TourId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Days.Should().Be(0);
        vm.Price.Should().Be(0);
        vm.Locations.Should().Be(string.Empty);
        vm.TourInfo.Should().Be(string.Empty);
        vm.Pic.Should().BeNull();
    }

    [Fact]
    public void TourViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var vm = new TourViewModel
        {
            TourId = 1,
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga, Calangute",
            TourInfo = "Beautiful beach tour",
            Pic = "goa.jpg"
        };

        // Assert
        vm.TourId.Should().Be(1);
        vm.TourName.Should().Be("Goa Tour");
        vm.Place.Should().Be("Goa");
        vm.Days.Should().Be(5);
        vm.Price.Should().Be(15000m);
        vm.Locations.Should().Be("Baga, Calangute");
        vm.TourInfo.Should().Be("Beautiful beach tour");
        vm.Pic.Should().Be("goa.jpg");
    }

    // ─── CreateTourViewModel ───────────────────────────────────────────────────

    [Fact]
    public void CreateTourViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CreateTourViewModel();

        // Assert
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Days.Should().Be(0);
        vm.Price.Should().Be(0);
        vm.Locations.Should().Be(string.Empty);
        vm.TourInfo.Should().Be(string.Empty);
        vm.PicFile.Should().BeNull();
    }

    [Fact]
    public void CreateTourViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var vm = new CreateTourViewModel
        {
            TourName = "Kerala Tour",
            Place = "Kerala",
            Days = 7,
            Price = 20000m,
            Locations = "Kochi, Munnar",
            TourInfo = "Backwaters and hills"
        };

        // Assert
        vm.TourName.Should().Be("Kerala Tour");
        vm.Place.Should().Be("Kerala");
        vm.Days.Should().Be(7);
        vm.Price.Should().Be(20000m);
        vm.Locations.Should().Be("Kochi, Munnar");
        vm.TourInfo.Should().Be("Backwaters and hills");
    }

    // ─── EditTourViewModel ─────────────────────────────────────────────────────

    [Fact]
    public void EditTourViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new EditTourViewModel();

        // Assert
        vm.TourId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Days.Should().Be(0);
        vm.Price.Should().Be(0);
        vm.Locations.Should().Be(string.Empty);
        vm.TourInfo.Should().Be(string.Empty);
        vm.Pic.Should().BeNull();
        vm.PicFile.Should().BeNull();
    }

    [Fact]
    public void EditTourViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var vm = new EditTourViewModel
        {
            TourId = 42,
            TourName = "Updated Tour",
            Place = "Updated Place",
            Days = 10,
            Price = 30000m,
            Locations = "Updated Locations",
            TourInfo = "Updated Info",
            Pic = "existing.jpg"
        };

        // Assert
        vm.TourId.Should().Be(42);
        vm.TourName.Should().Be("Updated Tour");
        vm.Place.Should().Be("Updated Place");
        vm.Days.Should().Be(10);
        vm.Price.Should().Be(30000m);
        vm.Locations.Should().Be("Updated Locations");
        vm.TourInfo.Should().Be("Updated Info");
        vm.Pic.Should().Be("existing.jpg");
    }

    [Theory]
    [InlineData(1, 1000.00)]
    [InlineData(7, 15000.50)]
    [InlineData(30, 99999.99)]
    public void CreateTourViewModel_WithVariousDaysAndPrices_ShouldRetainValues(int days, double price)
    {
        // Arrange & Act
        var vm = new CreateTourViewModel { Days = days, Price = (decimal)price };

        // Assert
        vm.Days.Should().Be(days);
        vm.Price.Should().Be((decimal)price);
    }
}
