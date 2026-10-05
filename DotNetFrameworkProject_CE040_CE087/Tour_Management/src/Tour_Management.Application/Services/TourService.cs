using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Application.Services;

/// <summary>
/// Service implementation for Tour business operations.
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository tourRepository, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active tours.
    /// </summary>
    public async Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Retrieves a tour by its ID.
    /// </summary>
    public async Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Creates a new tour.
    /// </summary>
    public async Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", tour.TourName);
            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;
            return await _tourRepository.AddAsync(tour, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", tour.TourName);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing tour.
    /// </summary>
    public async Task<Tour> UpdateTourAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID {TourId}", tour.TourId);
            var existing = await _tourRepository.GetByIdAsync(tour.TourId, cancellationToken);
            if (existing == null)
                throw new NotFoundException(nameof(Tour), tour.TourId);

            tour.ModifiedDate = DateTime.UtcNow;
            return await _tourRepository.UpdateAsync(tour, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", tour.TourId);
            throw;
        }
    }

    /// <summary>
    /// Deletes a tour by its ID.
    /// </summary>
    public async Task DeleteTourAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID {TourId}", id);
            var existing = await _tourRepository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new NotFoundException(nameof(Tour), id);

            await _tourRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches tours by a search term.
    /// </summary>
    public async Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default)
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
