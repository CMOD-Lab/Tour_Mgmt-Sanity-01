using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email asynchronously.
    /// </summary>
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user asynchronously.
    /// </summary>
    Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user by email asynchronously.
    /// </summary>
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by email asynchronously.
    /// </summary>
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user credentials asynchronously.
    /// </summary>
    Task<UserInfo?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by name or email asynchronously.
    /// </summary>
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
