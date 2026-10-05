using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for Booking entity.
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<BookingRepository> _logger;

    /// <summary>
    /// Initializes a new instance of BookingRepository.
    /// </summary>
    public BookingRepository(TourManagementDbContext context, ILogger<BookingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings from database");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Email == email && b.IsActive)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);
            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding booking for tour {TourName}", booking.TourName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking> UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);
            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", booking.BookingId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _context.Bookings.FindAsync(new object[] { id }, cancellationToken);
            if (booking != null)
            {
                booking.IsActive = false;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .AsNoTracking()
                .AnyAsync(b => b.BookingId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.IsActive && (
                    b.TourName.ToLower().Contains(term)
                    || b.Email.ToLower().Contains(term)
                    || b.FirstName.ToLower().Contains(term)))
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
