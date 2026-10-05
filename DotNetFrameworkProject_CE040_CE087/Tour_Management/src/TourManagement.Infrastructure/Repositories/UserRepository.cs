using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for UserInfo entity.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="UserRepository"/>.</summary>
    public UserRepository(TourManagementDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users from database");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user: {Email}", user.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", user.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user is null) return false;

            user.IsActive = false;
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive &&
                    (u.Email.ToLower().Contains(term) ||
                     u.FirstName.ToLower().Contains(term) ||
                     u.LastName.ToLower().Contains(term)))
                .OrderBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
