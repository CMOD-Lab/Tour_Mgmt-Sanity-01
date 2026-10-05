using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

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
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Active Tour", Place = "Goa", Days = 5, Price = 10000, IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Inactive Tour", Place = "Delhi", Days = 3, Price = 8000, IsActive = false, CreatedDate = DateTime.UtcNow }
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
        var tour = new Tour { TourName = "Kerala Tour", Place = "Kerala", Days = 6, Price = 20000, IsActive = true, CreatedDate = DateTime.UtcNow };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(tour.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Kerala Tour");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Tour to Delete", Place = "Mumbai", Days = 4, Price = 12000, IsActive = true, CreatedDate = DateTime.UtcNow };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await _context.Tours.FindAsync(tour.Id);
        deletedTour.Should().NotBeNull();
        deletedTour!.IsActive.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
