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
/// Extended unit tests for TourService covering all methods and edge cases.
/// </summary>
public class TourServiceExtendedTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceExtendedTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

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
        var tours = new List<Tour>
        {
            new Tour { TourId = 10, TourName = "Manali Tour", Place = "Manali", Days = 6, Price = 18000m, Locations = "Solang, Rohtang", TourInfo = "Snow tour", Pic = "manali.jpg", IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = (await _service.GetAllAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].TourId.Should().Be(10);
        result[0].TourName.Should().Be("Manali Tour");
        result[0].Price.Should().Be(18000m);
        result[0].Pic.Should().Be("manali.jpg");
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetByIdAsync(1)).Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldMapAllFields()
    {
        var tour = new Tour { TourId = 5, TourName = "Rajasthan Tour", Place = "Jaipur", Days = 8, Price = 22000m, Locations = "Jaipur, Jodhpur", TourInfo = "Desert tour", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _service.GetByIdAsync(5);

        result.Should().NotBeNull();
        result!.TourId.Should().Be(5);
        result.Place.Should().Be("Jaipur");
        result.Days.Should().Be(8);
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        var dto = new TourCreateDto { TourName = "T", Place = "P", Days = 1, Price = 100, Locations = "L", TourInfo = "I" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.CreateAsync(dto)).Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CreateAsync_ShouldSetIsActiveTrue()
    {
        var dto = new TourCreateDto { TourName = "Active Tour", Place = "P", Days = 3, Price = 5000, Locations = "L", TourInfo = "I" };
        Tour? captured = null;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Callback<Tour, CancellationToken>((t, _) => captured = t)
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedDate()
    {
        var dto = new TourCreateDto { TourName = "Date Tour", Place = "P", Days = 2, Price = 3000, Locations = "L", TourInfo = "I" };
        Tour? captured = null;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Callback<Tour, CancellationToken>((t, _) => captured = t)
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var before = DateTime.UtcNow.AddSeconds(-1);
        await _service.CreateAsync(dto);
        var after = DateTime.UtcNow.AddSeconds(1);

        captured!.CreatedDate.Should().BeAfter(before).And.BeBefore(after);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdateTour()
    {
        var existing = new Tour { TourId = 1, TourName = "Old Name", Place = "Old Place", Days = 3, Price = 5000, Locations = "L", TourInfo = "I", IsActive = true };
        var dto = new TourUpdateDto { TourName = "New Name", Place = "New Place", Days = 5, Price = 9000, Locations = "L2", TourInfo = "I2", IsActive = true };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var result = await _service.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result.TourName.Should().Be("New Name");
        result.Days.Should().Be(5);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await _service.Invoking(s => s.UpdateAsync(999, new TourUpdateDto())).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        var existing = new Tour { TourId = 1, TourName = "T", Place = "P", Days = 1, Price = 100, Locations = "L", TourInfo = "I", IsActive = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.UpdateAsync(1, new TourUpdateDto())).Should().ThrowAsync<Exception>();
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
    public async Task DeleteAsync_WhenRepositoryThrowsOnDelete_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.DeleteAsync(1)).Should().ThrowAsync<Exception>();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnFilteredTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 15000, Locations = "Srinagar", TourInfo = "Beautiful", IsActive = true }
        };
        _mockRepository.Setup(r => r.SearchAsync("kashmir", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _service.SearchAsync("kashmir");

        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.SearchAsync("zzz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

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
