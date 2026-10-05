using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Tour business operations.
/// </summary>
public interface ITourService
{
    Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default);
    Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default);
    Task<Tour> UpdateTourAsync(Tour tour, CancellationToken cancellationToken = default);
    Task DeleteTourAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default);
}
