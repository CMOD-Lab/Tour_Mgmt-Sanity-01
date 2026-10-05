using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for Tour entity.
/// </summary>
public class TourRepository : ITourRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<TourRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="TourRepository"/>.</summary>
    public TourRepository(TourManagementDbContext context, ILogger<TourRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Tours
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.TourName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours from database");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Tours
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour> AddAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync(cancellationToken);
            return tour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tour: {TourName}", tour.TourName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);
            return tour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", tour.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _context.Tours.FindAsync(new object[] { id }, cancellationToken);
            if (tour is null) return false;

            tour.IsActive = false;
            tour.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Tours
                .AsNoTracking()
                .AnyAsync(t => t.Id == id && t.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Tours
                .AsNoTracking()
                .Where(t => t.IsActive &&
                    (t.TourName.ToLower().Contains(term) ||
                     t.Place.ToLower().Contains(term) ||
                     t.Locations.ToLower().Contains(term)))
                .OrderBy(t => t.TourName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
