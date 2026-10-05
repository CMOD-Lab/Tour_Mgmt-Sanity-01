using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Booking business operations.
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default);
}
