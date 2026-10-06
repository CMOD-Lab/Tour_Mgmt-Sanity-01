using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for TourRepository using in-memory database.
/// </summary>
public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        var mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTourToDatabase()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            Locations = "Test Location",
            TourInfo = "Test Info",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "test"
        };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Active Tour", Place = "Place", Days = 3, Price = 500, Locations = "Loc", TourInfo = "Info", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
            new Tour { TourName = "Inactive Tour", Place = "Place", Days = 3, Price = 500, Locations = "Loc", TourInfo = "Info", IsActive = false, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Active Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour", Place = "Place", Days = 3, Price = 500, Locations = "Loc", TourInfo = "Info", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(tour.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour", Place = "Place", Days = 3, Price = 500, Locations = "Loc", TourInfo = "Info", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await _context.Tours.FindAsync(tour.Id);
        deletedTour.Should().NotBeNull();
        deletedTour!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Goa Beach Tour", Place = "Goa", Days = 5, Price = 1000, Locations = "Goa Beach", TourInfo = "Beach tour", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
            new Tour { TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 2000, Locations = "Kashmir Valley", TourInfo = "Mountain tour", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Goa");

        // Assert
        result.Should().HaveCount(1);
        result.First().Place.Should().Be("Goa");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
