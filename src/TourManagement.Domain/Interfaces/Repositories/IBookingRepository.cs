using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Booking entity.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Gets all bookings asynchronously.
    /// </summary>
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a booking by ID asynchronously.
    /// </summary>
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all bookings for a specific user asynchronously.
    /// </summary>
    Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new booking asynchronously.
    /// </summary>
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    Task<Booking> UpdateAsync(Booking booking, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a booking by ID asynchronously.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a booking exists by ID asynchronously.
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches bookings by tour name or user email asynchronously.
    /// </summary>
    Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
