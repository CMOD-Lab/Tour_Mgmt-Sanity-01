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
            Locations = "North Goa, South Goa",
            TourInfo = "Beautiful beach tour"
        };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.TourId.Should().BeGreaterThan(0);
        var dbTour = await _context.Tours.FindAsync(result.TourId);
        dbTour.Should().NotBeNull();
        dbTour!.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "Place 1", Days = 3, Price = 10000, Locations = "L1", TourInfo = "Info 1" },
            new Tour { TourName = "Tour 2", Place = "Place 2", Days = 5, Price = 20000, Locations = "L2", TourInfo = "Info 2" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTourFromDatabase()
    {
        // Arrange
        var tour = new Tour { TourName = "Delete Me", Place = "Place", Days = 2, Price = 5000, Locations = "L", TourInfo = "Info" };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tour.TourId);

        // Assert
        var dbTour = await _context.Tours.FindAsync(tour.TourId);
        dbTour.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Goa Beach Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach" },
            new Tour { TourName = "Kashmir Snow Tour", Place = "Kashmir", Days = 7, Price = 25000, Locations = "Kashmir", TourInfo = "Snow" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("goa");

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Goa Beach Tour");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
