using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data;
using Tour_Management.Infrastructure.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Repositories;

/// <summary>
/// Unit tests for TourRepository using InMemory database.
/// </summary>
public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly TourRepository _repository;

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

    // ─── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Active Tour", Place = "Goa", IsActive = true },
            new Tour { TourId = 2, TourName = "Inactive Tour", Place = "Delhi", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Active Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnToursOrderedByName()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Zebra Tour", Place = "Z", IsActive = true },
            new Tour { TourId = 2, TourName = "Alpha Tour", Place = "A", IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result[0].TourName.Should().Be("Alpha Tour");
        result[1].TourName.Should().Be("Zebra Tour");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoTours_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 10, TourName = "Goa Tour", Place = "Goa", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(10);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 20, TourName = "Inactive Tour", Place = "X", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(20);

        // Assert
        result.Should().BeNull();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistTour()
    {
        // Arrange
        var tour = new Tour { TourId = 30, TourName = "New Tour", Place = "Kerala", Days = 5, Price = 15000, IsActive = true };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        _context.Tours.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnSameTour()
    {
        // Arrange
        var tour = new Tour { TourId = 31, TourName = "Test Tour", Place = "Test", Days = 3, Price = 5000, IsActive = true };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.TourId.Should().Be(31);
        result.TourName.Should().Be("Test Tour");
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTourInDatabase()
    {
        // Arrange
        var tour = new Tour { TourId = 40, TourName = "Old Name", Place = "Old Place", Days = 3, Price = 5000, IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var updatedTour = new Tour { TourId = 40, TourName = "New Name", Place = "New Place", Days = 5, Price = 10000, IsActive = true };

        // Act
        var result = await _repository.UpdateAsync(updatedTour);

        // Assert
        result.TourName.Should().Be("New Name");
        var fromDb = await _context.Tours.FindAsync(40);
        fromDb!.TourName.Should().Be("New Name");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var tour = new Tour { TourId = 50, TourName = "To Delete", Place = "X", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _repository.DeleteAsync(50);

        // Assert
        var fromDb = await _context.Tours.FindAsync(50);
        fromDb!.IsActive.Should().BeFalse();
        fromDb.ModifiedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldNotThrow()
    {
        // Act & Assert
        var action = async () => await _repository.DeleteAsync(9999);
        await action.Should().NotThrowAsync();
    }

    // ─── ExistsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ShouldReturnTrue()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 60, TourName = "Existing Tour", Place = "X", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(60);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(9999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 70, TourName = "Inactive Tour", Place = "X", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(70);

        // Assert
        result.Should().BeFalse();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnMatchingTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourId = 80, TourName = "Goa Beach Tour", Place = "Goa", Locations = "Baga", IsActive = true },
            new Tour { TourId = 81, TourName = "Kerala Tour", Place = "Kerala", Locations = "Kochi", IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Goa");

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Contain("Goa");
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnMatchingTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourId = 90, TourName = "Tour A", Place = "Kashmir", Locations = "Dal Lake", IsActive = true },
            new Tour { TourId = 91, TourName = "Tour B", Place = "Goa", Locations = "Baga", IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Kashmir");

        // Assert
        result.Should().HaveCount(1);
        result.First().Place.Should().Be("Kashmir");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 100, TourName = "Goa Tour", Place = "Goa", Locations = "Baga", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("XYZ_NO_MATCH");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        // Arrange
        _context.Tours.Add(new Tour { TourId = 110, TourName = "Inactive Goa Tour", Place = "Goa", Locations = "Baga", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Goa");

        // Assert
        result.Should().BeEmpty();
    }
}
