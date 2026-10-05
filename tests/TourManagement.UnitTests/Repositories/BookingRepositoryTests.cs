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
/// Integration-style unit tests for BookingRepository using InMemory database.
/// </summary>
public class BookingRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly BookingRepository _repository;
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;

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

    private Booking CreateSampleBooking(string tourName = "Test Tour", string email = "user@test.com", bool isActive = true) => new Booking
    {
        TourName = tourName,
        Place = "Test Place",
        Email = email,
        FirstName = "Test User",
        BookingDate = DateTime.UtcNow,
        IsActive = isActive
    };

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveBookings()
    {
        _context.Bookings.AddRange(
            CreateSampleBooking("Active Tour 1", isActive: true),
            CreateSampleBooking("Active Tour 2", isActive: true),
            CreateSampleBooking("Inactive Tour", isActive: false)
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.All(b => b.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_WhenNoBookings_ShouldReturnEmptyList()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEmpty();
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        var booking = CreateSampleBooking("Kashmir Tour");
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(booking.BookingId);

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
    public async Task GetByIdAsync_ShouldReturnInactiveBookingToo()
    {
        var booking = CreateSampleBooking("Inactive Tour", isActive: false);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(booking.BookingId);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }

    // ─── GetByUserEmailAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserEmailAsync_ShouldReturnBookingsForUser()
    {
        _context.Bookings.AddRange(
            CreateSampleBooking("Tour A", "alice@test.com"),
            CreateSampleBooking("Tour B", "alice@test.com"),
            CreateSampleBooking("Tour C", "bob@test.com")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserEmailAsync("alice@test.com");

        result.Should().HaveCount(2);
        result.All(b => b.Email == "alice@test.com").Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserEmailAsync_ShouldReturnOnlyActiveBookings()
    {
        _context.Bookings.AddRange(
            CreateSampleBooking("Active Tour", "alice@test.com", isActive: true),
            CreateSampleBooking("Inactive Tour", "alice@test.com", isActive: false)
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserEmailAsync("alice@test.com");

        result.Should().HaveCount(1);
        result.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        var result = await _repository.GetByUserEmailAsync("nobody@test.com");
        result.Should().BeEmpty();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistBooking()
    {
        var booking = CreateSampleBooking("New Tour");

        var result = await _repository.AddAsync(booking);

        result.BookingId.Should().BeGreaterThan(0);
        _context.Bookings.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnCreatedBooking()
    {
        var booking = CreateSampleBooking("Returned Tour");

        var result = await _repository.AddAsync(booking);

        result.Should().NotBeNull();
        result.TourName.Should().Be("Returned Tour");
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var booking = CreateSampleBooking("Original Tour");
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        booking.TourName = "Updated Tour";
        await _repository.UpdateAsync(booking);

        var updated = await _context.Bookings.FindAsync(booking.BookingId);
        updated!.TourName.Should().Be("Updated Tour");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveFalse()
    {
        var booking = CreateSampleBooking("To Delete");
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(booking.BookingId);

        var deleted = await _context.Bookings.FindAsync(booking.BookingId);
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
        var booking = CreateSampleBooking("Existing Tour");
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(booking.BookingId);

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
    public async Task SearchAsync_ByTourName_ShouldReturnMatchingBookings()
    {
        _context.Bookings.AddRange(
            CreateSampleBooking("Kashmir Tour"),
            CreateSampleBooking("Goa Beach Tour"),
            CreateSampleBooking("Rajasthan Tour")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("kashmir");

        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task SearchAsync_ByEmail_ShouldReturnMatchingBookings()
    {
        _context.Bookings.AddRange(
            CreateSampleBooking("Tour A", "alice@test.com"),
            CreateSampleBooking("Tour B", "bob@test.com")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("alice");

        result.Should().HaveCount(1);
        result.First().Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _context.Bookings.Add(CreateSampleBooking("Kashmir Tour"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("zzznomatch");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_ShouldBeCaseInsensitive()
    {
        _context.Bookings.Add(CreateSampleBooking("Kashmir Tour"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("KASHMIR");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveBookings()
    {
        _context.Bookings.Add(CreateSampleBooking("Kashmir Tour", isActive: false));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("kashmir");

        result.Should().BeEmpty();
    }
}
