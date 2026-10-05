using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Tour entity operations.
/// </summary>
public interface ITourRepository
{
    Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tour> AddAsync(Tour tour, CancellationToken cancellationToken = default);
    Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
