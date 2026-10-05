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
/// Unit tests for BookingService.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mockLogger.Object);
    }

    // ─── GetAllBookingsAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { TourId = 1, TourName = "Goa Tour", Email = "a@test.com", FirstName = "Alice" },
            new Booking { TourId = 2, TourName = "Kerala Tour", Email = "b@test.com", FirstName = "Bob" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllBookingsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.TourName == "Goa Tour");
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetAllBookingsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllBookingsAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllBookingsAsync());
    }

    // ─── GetBookingsByEmailAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetBookingsByEmailAsync_WithValidEmail_ShouldReturnBookings()
    {
        // Arrange
        var email = "alice@test.com";
        var bookings = new List<Booking>
        {
            new Booking { TourId = 1, TourName = "Goa Tour", Email = email, FirstName = "Alice" }
        };
        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsByEmailAsync(email);

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be(email);
    }

    [Fact]
    public async Task GetBookingsByEmailAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nobody@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetBookingsByEmailAsync("nobody@test.com");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBookingsByEmailAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetBookingsByEmailAsync("test@test.com"));
    }

    // ─── CreateBookingAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateBookingAsync_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var booking = new Booking
        {
            TourId = 1,
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "alice@test.com",
            FirstName = "Alice"
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking b, CancellationToken _) => b);

        // Act
        var result = await _service.CreateBookingAsync(booking);

        // Assert
        result.Should().NotBeNull();
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturnCreatedBooking()
    {
        // Arrange
        var booking = new Booking { TourName = "Kerala Tour", Email = "bob@test.com", FirstName = "Bob" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _service.CreateBookingAsync(booking);

        // Assert
        result.TourName.Should().Be("Kerala Tour");
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        var booking = new Booking { TourName = "Tour", Email = "x@test.com", FirstName = "X" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateBookingAsync(booking));
    }

    // ─── DeleteBookingAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteBookingAsync_WithValidId_ShouldDeleteBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 1, TourName = "Goa Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBookingAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteBookingAsync(999));
    }

    [Fact]
    public async Task DeleteBookingAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteBookingAsync(1));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var booking = new Booking { TourId = 5, TourName = "Test Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(5, token)).ReturnsAsync(booking);
        _mockRepository.Setup(r => r.DeleteAsync(5, token)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBookingAsync(5, token);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(5, token), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(5, token), Times.Once);
    }
}
