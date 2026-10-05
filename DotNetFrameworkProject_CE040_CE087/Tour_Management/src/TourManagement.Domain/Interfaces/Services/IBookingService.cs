using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Booking business operations.
/// </summary>
public interface IBookingService
{
    /// <summary>Gets all active bookings.</summary>
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a booking by its identifier.</summary>
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets all bookings for a specific user email.</summary>
    Task<IEnumerable<Booking>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new booking.</summary>
    Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing booking.</summary>
    Task<Booking?> UpdateAsync(int id, Booking booking, CancellationToken cancellationToken = default);

    /// <summary>Deletes a booking by its identifier.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches bookings by tour name or email.</summary>
    Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
