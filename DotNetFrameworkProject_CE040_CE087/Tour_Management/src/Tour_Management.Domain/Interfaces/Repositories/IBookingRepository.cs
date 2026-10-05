using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Booking entity CRUD operations.
/// </summary>
public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
