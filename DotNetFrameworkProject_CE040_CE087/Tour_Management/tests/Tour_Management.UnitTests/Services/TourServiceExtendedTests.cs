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
/// Extended unit tests for TourService covering all methods and edge cases.
/// </summary>
public class TourServiceExtendedTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceExtendedTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockLogger.Object);
    }

    // ─── GetAllToursAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllToursAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _service.GetAllToursAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllToursAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllToursAsync());
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldCallRepositoryOnce()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _service.GetAllToursAsync();

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── GetTourByIdAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetTourByIdAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetTourByIdAsync(1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public async Task GetTourByIdAsync_WithVariousIds_ShouldCallRepositoryWithCorrectId(int id)
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        await _service.GetTourByIdAsync(id);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── CreateTourAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task CreateTourAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 3, Price = 5000 };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        // Act
        var result = await _service.CreateTourAsync(tour);

        // Assert
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateTourAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour", IsActive = false };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        // Act
        var result = await _service.CreateTourAsync(tour);

        // Assert
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTourAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateTourAsync(tour));
    }

    // ─── UpdateTourAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateTourAsync_WithValidTour_ShouldUpdateAndReturn()
    {
        // Arrange
        var existing = new Tour { TourId = 1, TourName = "Old Name" };
        var updated = new Tour { TourId = 1, TourName = "New Name", Place = "New Place", Days = 5, Price = 10000 };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        // Act
        var result = await _service.UpdateTourAsync(updated);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Name");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistentTour_ShouldThrowNotFoundException()
    {
        // Arrange
        var tour = new Tour { TourId = 999, TourName = "Ghost Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existing = new Tour { TourId = 1, TourName = "Old Name" };
        var tour = new Tour { TourId = 1, TourName = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        // Act
        var result = await _service.UpdateTourAsync(tour);

        // Assert
        result.ModifiedDate.Should().NotBeNull();
        result.ModifiedDate!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateTourAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        var existing = new Tour { TourId = 1, TourName = "Existing" };
        var tour = new Tour { TourId = 1, TourName = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateTourAsync(tour));
    }

    // ─── DeleteTourAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteTourAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteTourAsync(1));
    }

    // ─── SearchToursAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task SearchToursAsync_WithMatchingTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Beach Tour", Place = "Goa" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.SearchToursAsync("Goa");

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Contain("Goa");
    }

    [Fact]
    public async Task SearchToursAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("XYZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _service.SearchToursAsync("XYZ");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SearchToursAsync("test"));
    }

    [Fact]
    public async Task SearchToursAsync_ShouldCallRepositoryWithCorrectSearchTerm()
    {
        // Arrange
        var searchTerm = "Kashmir";
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _service.SearchToursAsync(searchTerm);

        // Assert
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }
}
