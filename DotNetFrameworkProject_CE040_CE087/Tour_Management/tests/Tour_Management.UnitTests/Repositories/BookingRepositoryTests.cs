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
/// Unit tests for BookingRepository using InMemory database.
/// </summary>
public class BookingRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly BookingRepository _repository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _repository = new BookingRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // ─── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        _context.Bookings.AddRange(
            new Booking { TourId = 1, TourName = "Goa Tour", Email = "a@test.com", FirstName = "Alice", CreatedDate = DateTime.UtcNow.AddDays(-1) },
            new Booking { TourId = 2, TourName = "Kerala Tour", Email = "b@test.com", FirstName = "Bob", CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnBookingsOrderedByCreatedDateDescending()
    {
        // Arrange
        var older = new Booking { TourId = 1, TourName = "Old Tour", Email = "a@test.com", FirstName = "A", CreatedDate = DateTime.UtcNow.AddDays(-5) };
        var newer = new Booking { TourId = 2, TourName = "New Tour", Email = "b@test.com", FirstName = "B", CreatedDate = DateTime.UtcNow };
        _context.Bookings.AddRange(older, newer);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result[0].TourName.Should().Be("New Tour");
        result[1].TourName.Should().Be("Old Tour");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoBookings_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        _context.Bookings.Add(new Booking { TourId = 10, TourName = "Test Tour", Email = "test@test.com", FirstName = "Test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(10);

        // Assert
        result.Should().NotBeNull();
        result!.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        result.Should().BeNull();
    }

    // ─── GetByEmailAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnBookings()
    {
        // Arrange
        var email = "alice@test.com";
        _context.Bookings.AddRange(
            new Booking { TourId = 1, TourName = "Tour A", Email = email, FirstName = "Alice" },
            new Booking { TourId = 2, TourName = "Tour B", Email = "other@test.com", FirstName = "Other" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nobody@test.com");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnMultipleBookingsForSameEmail()
    {
        // Arrange
        var email = "multi@test.com";
        _context.Bookings.AddRange(
            new Booking { TourId = 1, TourName = "Tour 1", Email = email, FirstName = "Multi" },
            new Booking { TourId = 2, TourName = "Tour 2", Email = email, FirstName = "Multi" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().HaveCount(2);
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 20, TourName = "New Booking Tour", Email = "new@test.com", FirstName = "New" };

        // Act
        var result = await _repository.AddAsync(booking);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Booking Tour");
        _context.Bookings.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnSameBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 21, TourName = "Test Booking", Email = "test@test.com", FirstName = "Test" };

        // Act
        var result = await _repository.AddAsync(booking);

        // Assert
        result.TourId.Should().Be(21);
        result.Email.Should().Be("test@test.com");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        // Arrange
        var booking = new Booking { TourId = 30, TourName = "To Delete", Email = "del@test.com", FirstName = "Del" };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _repository.DeleteAsync(30);

        // Assert
        var fromDb = await _context.Bookings.FirstOrDefaultAsync(b => b.TourId == 30);
        fromDb.Should().BeNull();
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
    public async Task ExistsAsync_WithExistingBooking_ShouldReturnTrue()
    {
        // Arrange
        _context.Bookings.Add(new Booking { TourId = 40, TourName = "Existing", Email = "e@test.com", FirstName = "E" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(40);

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
}
