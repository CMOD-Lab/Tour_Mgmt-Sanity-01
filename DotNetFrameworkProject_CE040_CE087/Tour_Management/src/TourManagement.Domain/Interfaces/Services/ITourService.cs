using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Tour business operations.
/// </summary>
public interface ITourService
{
    /// <summary>Gets all active tours.</summary>
    Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a tour by its identifier.</summary>
    Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new tour.</summary>
    Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing tour.</summary>
    Task<Tour?> UpdateAsync(int id, Tour tour, CancellationToken cancellationToken = default);

    /// <summary>Deletes a tour by its identifier.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Searches tours by name or place.</summary>
    Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
