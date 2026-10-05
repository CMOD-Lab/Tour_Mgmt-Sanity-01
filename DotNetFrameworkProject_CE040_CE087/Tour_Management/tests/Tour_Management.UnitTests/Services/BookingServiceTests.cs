using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tour_Management.Application.Mappings;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Services;

/// <summary>
/// Unit tests for BookingService.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourName = "Goa Tour", Email = "user@test.com", IsActive = true },
            new Booking { Id = 2, TourName = "Kashmir Tour", Email = "user2@test.com", IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllBookingsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateAndReturnBooking()
    {
        // Arrange
        var booking = new Booking
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@test.com",
            FirstName = "John"
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default)).ReturnsAsync(booking);

        // Act
        var result = await _service.CreateBookingAsync(booking);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("Goa Tour");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), default), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByEmailAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var email = "user@test.com";
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourName = "Goa Tour", Email = email, IsActive = true }
        };
        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsByEmailAsync(email);

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be(email);
    }
}
