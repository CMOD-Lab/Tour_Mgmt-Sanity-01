using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for Booking operations.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Gets all bookings asynchronously.
    /// </summary>
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a booking by ID asynchronously.
    /// </summary>
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all bookings for a specific user asynchronously.
    /// </summary>
    Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new booking asynchronously.
    /// </summary>
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a booking by ID asynchronously.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches bookings by tour name or user email asynchronously.
    /// </summary>
    Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
