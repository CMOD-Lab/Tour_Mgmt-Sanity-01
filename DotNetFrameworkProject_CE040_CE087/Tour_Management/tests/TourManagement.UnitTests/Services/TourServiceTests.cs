using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for TourService.
/// </summary>
public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    /// <summary>Initializes test dependencies.</summary>
    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new() { Id = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach tour", IsActive = true },
            new() { Id = 2, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000, Locations = "Kashmir", TourInfo = "Mountain tour", IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach tour", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnTour()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "Delhi",
            Days = 3,
            Price = 10000,
            Locations = "Delhi, Agra",
            TourInfo = "Heritage tour"
        };

        var createdTour = new Tour
        {
            Id = 1,
            TourName = tour.TourName,
            Place = tour.Place,
            Days = tour.Days,
            Price = tour.Price,
            Locations = tour.Locations,
            TourInfo = tour.TourInfo,
            IsActive = true
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _service.CreateAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}
