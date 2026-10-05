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
/// Extended unit tests for BookingService covering all methods and edge cases.
/// </summary>
public class BookingServiceExtendedTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceExtendedTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetAllAsync()).Should().ThrowAsync<Exception>().WithMessage("DB error");
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapAllFields()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = "u@t.com", FirstName = "John", BookingDate = new DateTime(2024, 6, 1), IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = (await _service.GetAllAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].BookingId.Should().Be(1);
        result[0].TourName.Should().Be("Goa Tour");
        result[0].Email.Should().Be("u@t.com");
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        var booking = new Booking { BookingId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Email = "u@t.com", FirstName = "John", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.BookingId.Should().Be(1);
        result.TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetByIdAsync(1)).Should().ThrowAsync<Exception>();
    }

    // ─── GetByUserEmailAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserEmailAsync_WhenNoBookings_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.GetByUserEmailAsync("nobody@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _service.GetByUserEmailAsync("nobody@test.com");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserEmailAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetByUserEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetByUserEmailAsync("u@t.com")).Should().ThrowAsync<Exception>();
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ShouldSetIsActiveTrue()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J" };
        Booking? captured = null;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) => captured = b)
            .ReturnsAsync((Booking b, CancellationToken _) => b);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldSetBookingDate()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J" };
        Booking? captured = null;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((b, _) => captured = b)
            .ReturnsAsync((Booking b, CancellationToken _) => b);

        var before = DateTime.UtcNow.AddSeconds(-1);
        await _service.CreateAsync(dto);
        var after = DateTime.UtcNow.AddSeconds(1);

        captured!.BookingDate.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.CreateAsync(dto)).Should().ThrowAsync<Exception>();
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdateBooking()
    {
        var existing = new Booking { BookingId = 1, TourName = "Old Tour", Place = "Old Place", Email = "u@t.com", FirstName = "John", IsActive = true };
        var dto = new BookingUpdateDto { TourName = "New Tour", Place = "New Place", Email = "u@t.com", FirstName = "John", IsActive = true };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking b, CancellationToken _) => b);

        var result = await _service.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        await _service.Invoking(s => s.UpdateAsync(999, new BookingUpdateDto())).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        var existing = new Booking { BookingId = 1, TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.UpdateAsync(1, new BookingUpdateDto())).Should().ThrowAsync<Exception>();
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldCallRepositoryDelete()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await _service.Invoking(s => s.DeleteAsync(999)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrowsOnDelete_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.DeleteAsync(1)).Should().ThrowAsync<Exception>();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnFilteredBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Email = "u@t.com", FirstName = "John", IsActive = true }
        };
        _mockRepository.Setup(r => r.SearchAsync("kashmir", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.SearchAsync("kashmir");

        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.SearchAsync("zzz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _service.SearchAsync("zzz");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.SearchAsync("term")).Should().ThrowAsync<Exception>();
    }
}
