using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour business operations.
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourService> _logger;

    /// <summary>Initializes a new instance of <see cref="TourService"/>.</summary>
    public TourService(ITourRepository tourRepository, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            return await _tourRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID {TourId}", id);
            return await _tourRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", tour.TourName);
            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;
            var created = await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created with ID {TourId}", created.Id);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", tour.TourName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour?> UpdateAsync(int id, Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID {TourId}", id);
            var existing = await _tourRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found for update", id);
                return null;
            }

            existing.TourName = tour.TourName;
            existing.Place = tour.Place;
            existing.Days = tour.Days;
            existing.Price = tour.Price;
            existing.Locations = tour.Locations;
            existing.TourInfo = tour.TourInfo;
            existing.PictureName = tour.PictureName ?? existing.PictureName;
            existing.IsActive = tour.IsActive;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = tour.ModifiedBy;

            var updated = await _tourRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Tour with ID {TourId} updated successfully", id);
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID {TourId}", id);
            var result = await _tourRepository.DeleteAsync(id, cancellationToken);
            if (result)
                _logger.LogInformation("Tour with ID {TourId} deleted successfully", id);
            else
                _logger.LogWarning("Tour with ID {TourId} not found for deletion", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            return await _tourRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
