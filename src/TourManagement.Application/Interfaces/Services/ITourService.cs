using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for Tour operations.
/// </summary>
public interface ITourService
{
    /// <summary>
    /// Gets all active tours asynchronously.
    /// </summary>
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tour by ID asynchronously.
    /// </summary>
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new tour asynchronously.
    /// </summary>
    Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tour asynchronously.
    /// </summary>
    Task<TourDto> UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a tour by ID asynchronously.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches tours by name or place asynchronously.
    /// </summary>
    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
