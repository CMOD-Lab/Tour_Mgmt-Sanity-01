using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Tour entity CRUD operations.
/// </summary>
public interface ITourRepository
{
    /// <summary>Gets all active tours.</summary>
    Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a tour by its identifier.</summary>
    Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new tour.</summary>
    Task<Tour> AddAsync(Tour tour, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing tour.</summary>
    Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default);

    /// <summary>Deletes a tour by its identifier.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a tour exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches tours by name or place.</summary>
    Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
