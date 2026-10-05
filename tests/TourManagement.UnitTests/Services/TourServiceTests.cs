using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for TourService.
/// </summary>
public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    /// <summary>
    /// Initializes test dependencies.
    /// </summary>
    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that GetAllAsync returns all tours.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 15000, Locations = "Srinagar, Gulmarg", TourInfo = "Beautiful Kashmir tour", IsActive = true },
            new Tour { TourId = 2, TourName = "Goa Beach Tour", Place = "Goa", Days = 5, Price = 12000, Locations = "North Goa, South Goa", TourInfo = "Relaxing beach tour", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().TourName.Should().Be("Kashmir Tour");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct tour.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 15000, Locations = "Srinagar", TourInfo = "Beautiful tour", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TourId.Should().Be(1);
        result.TourName.Should().Be("Kashmir Tour");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null for non-existent tour.
    /// </summary>
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

    /// <summary>
    /// Tests that CreateAsync creates a new tour successfully.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateTour()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "Delhi",
            Days = 3,
            Price = 8000,
            Locations = "Delhi, Agra",
            TourInfo = "Golden Triangle tour"
        };

        var createdTour = new Tour
        {
            TourId = 1,
            TourName = createDto.TourName,
            Place = createDto.Place,
            Days = createDto.Days,
            Price = createDto.Price,
            Locations = createDto.Locations,
            TourInfo = createDto.TourInfo,
            IsActive = true
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        result.TourId.Should().Be(1);
    }

    /// <summary>
    /// Tests that DeleteAsync throws NotFoundException for non-existent tour.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(999))
            .Should().ThrowAsync<NotFoundException>();
    }
}
