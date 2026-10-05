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
/// Unit tests for BookingService.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    /// <summary>
    /// Initializes test dependencies.
    /// </summary>
    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that GetAllAsync returns all bookings.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Email = "user1@test.com", FirstName = "John", IsActive = true },
            new Booking { BookingId = 2, TourName = "Goa Tour", Place = "Goa", Email = "user2@test.com", FirstName = "Jane", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that CreateAsync creates a booking successfully.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateBooking()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Email = "user@test.com",
            FirstName = "John"
        };

        var createdBooking = new Booking
        {
            BookingId = 1,
            TourName = createDto.TourName,
            Place = createDto.Place,
            Email = createDto.Email,
            FirstName = createDto.FirstName,
            BookingDate = DateTime.UtcNow,
            IsActive = true
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("Kashmir Tour");
        result.BookingId.Should().Be(1);
    }

    /// <summary>
    /// Tests that GetByUserEmailAsync returns bookings for a specific user.
    /// </summary>
    [Fact]
    public async Task GetByUserEmailAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var email = "user@test.com";
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Kashmir Tour", Email = email, IsActive = true },
            new Booking { BookingId = 2, TourName = "Goa Tour", Email = email, IsActive = true }
        };

        _mockRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByUserEmailAsync(email);

        // Assert
        result.Should().HaveCount(2);
        result.All(b => b.Email == email).Should().BeTrue();
    }
}
