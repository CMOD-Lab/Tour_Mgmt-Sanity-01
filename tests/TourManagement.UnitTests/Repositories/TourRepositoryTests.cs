using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Repositories;

/// <summary>
/// Integration-style unit tests for TourRepository using InMemory database.
/// </summary>
public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private Tour CreateSampleTour(string name = "Test Tour", bool isActive = true) => new Tour
    {
        TourName = name,
        Place = "Test Place",
        Days = 5,
        Price = 10000m,
        Locations = "Location A, Location B",
        TourInfo = "Test tour information",
        IsActive = isActive
    };

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        _context.Tours.AddRange(
            CreateSampleTour("Active Tour 1", isActive: true),
            CreateSampleTour("Active Tour 2", isActive: true),
            CreateSampleTour("Inactive Tour", isActive: false)
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.All(t => t.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnToursOrderedByName()
    {
        _context.Tours.AddRange(
            CreateSampleTour("Zebra Tour"),
            CreateSampleTour("Apple Tour"),
            CreateSampleTour("Mango Tour")
        );
        await _context.SaveChangesAsync();

        var result = (await _repository.GetAllAsync()).ToList();

        result[0].TourName.Should().Be("Apple Tour");
        result[1].TourName.Should().Be("Mango Tour");
        result[2].TourName.Should().Be("Zebra Tour");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoTours_ShouldReturnEmptyList()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEmpty();
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        var tour = CreateSampleTour("Kashmir Tour");
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.TourId);

        result.Should().NotBeNull();
        result!.TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var result = await _repository.GetByIdAsync(9999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnInactiveTourToo()
    {
        var tour = CreateSampleTour("Inactive Tour", isActive: false);
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.TourId);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistTour()
    {
        var tour = CreateSampleTour("New Tour");

        var result = await _repository.AddAsync(tour);

        result.TourId.Should().BeGreaterThan(0);
        _context.Tours.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnCreatedTour()
    {
        var tour = CreateSampleTour("Returned Tour");

        var result = await _repository.AddAsync(tour);

        result.Should().NotBeNull();
        result.TourName.Should().Be("Returned Tour");
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var tour = CreateSampleTour("Original Name");
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        tour.TourName = "Updated Name";
        await _repository.UpdateAsync(tour);

        var updated = await _context.Tours.FindAsync(tour.TourId);
        updated!.TourName.Should().Be("Updated Name");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveFalse()
    {
        var tour = CreateSampleTour("To Delete");
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(tour.TourId);

        var deleted = await _context.Tours.FindAsync(tour.TourId);
        deleted!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldNotThrow()
    {
        Func<Task> act = async () => await _repository.DeleteAsync(9999);
        await act.Should().NotThrowAsync();
    }

    // ─── ExistsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        var tour = CreateSampleTour("Existing Tour");
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(tour.TourId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ShouldReturnFalse()
    {
        var result = await _repository.ExistsAsync(9999);
        result.Should().BeFalse();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ByTourName_ShouldReturnMatchingTours()
    {
        _context.Tours.AddRange(
            CreateSampleTour("Kashmir Tour"),
            CreateSampleTour("Goa Beach Tour"),
            CreateSampleTour("Rajasthan Desert Tour")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("kashmir");

        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task SearchAsync_ByPlace_ShouldReturnMatchingTours()
    {
        var tour = new Tour { TourName = "Beach Holiday", Place = "Goa", Days = 5, Price = 12000, Locations = "North Goa", TourInfo = "Beach fun", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("goa");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _context.Tours.Add(CreateSampleTour("Kashmir Tour"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("zzznomatch");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_ShouldBeCaseInsensitive()
    {
        _context.Tours.Add(CreateSampleTour("Kashmir Tour"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("KASHMIR");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        _context.Tours.Add(CreateSampleTour("Kashmir Tour", isActive: false));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("kashmir");

        result.Should().BeEmpty();
    }
}
