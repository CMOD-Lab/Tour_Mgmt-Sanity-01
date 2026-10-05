using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Application.Services;

/// <summary>
/// Service implementation for Booking business operations.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    public async Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Retrieves bookings by user email.
    /// </summary>
    public async Task<IEnumerable<Booking>> GetBookingsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for email: {Email}", email);
            return await _bookingRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating booking for tour: {TourName}", booking.TourName);
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            return await _bookingRepository.AddAsync(booking, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", booking.TourName);
            throw;
        }
    }

    /// <summary>
    /// Deletes a booking by its ID.
    /// </summary>
    public async Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);
            var existing = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new NotFoundException(nameof(Booking), id);

            await _bookingRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }
}
