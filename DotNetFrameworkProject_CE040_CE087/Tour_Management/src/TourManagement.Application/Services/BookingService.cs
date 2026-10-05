using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingService> _logger;

    /// <summary>Initializes a new instance of <see cref="BookingService"/>.</summary>
    public BookingService(IBookingRepository bookingRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            return await _bookingRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID {BookingId}", id);
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for email {Email}", email);
            return await _bookingRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour: {TourName}", booking.TourName);
            booking.CreatedDate = DateTime.UtcNow;
            booking.BookingDate = DateTime.UtcNow;
            booking.IsActive = true;
            var created = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created with ID {BookingId}", created.Id);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", booking.TourName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking?> UpdateAsync(int id, Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID {BookingId}", id);
            var existing = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found for update", id);
                return null;
            }

            existing.TourName = booking.TourName;
            existing.Place = booking.Place;
            existing.Email = booking.Email;
            existing.FirstName = booking.FirstName;
            existing.IsActive = booking.IsActive;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = booking.ModifiedBy;

            var updated = await _bookingRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Booking with ID {BookingId} updated successfully", id);
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);
            var result = await _bookingRepository.DeleteAsync(id, cancellationToken);
            if (result)
                _logger.LogInformation("Booking with ID {BookingId} deleted successfully", id);
            else
                _logger.LogWarning("Booking with ID {BookingId} not found for deletion", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            return await _bookingRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
