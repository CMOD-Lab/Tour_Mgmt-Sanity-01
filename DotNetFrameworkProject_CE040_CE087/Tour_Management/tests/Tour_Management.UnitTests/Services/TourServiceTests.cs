using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Services;

/// <summary>
/// Unit tests for TourService.
/// </summary>
public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000 },
            new Tour { TourId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000 }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllToursAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.TourName == "Goa Tour");
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000 };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetTourByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetTourByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateTourAsync_ShouldCreateAndReturnTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Kerala Tour", Place = "Kerala", Days = 6, Price = 20000 };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.CreateTourAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("Kerala Tour");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithValidId_ShouldDeleteTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Goa Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteTourAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTourAsync(999));
    }
}
