using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Application.Mappings;
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
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, IsActive = true },
            new Tour { Id = 2, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000, IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);

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
        var tour = new Tour { Id = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(tour);

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
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetTourByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateTourAsync_ShouldCreateAndReturnTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Place = "Kerala", Days = 4, Price = 12000 };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default)).ReturnsAsync(tour);

        // Act
        var result = await _service.CreateTourAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithValidId_ShouldDeleteTour()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteTourAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTourAsync(999));
    }
}
