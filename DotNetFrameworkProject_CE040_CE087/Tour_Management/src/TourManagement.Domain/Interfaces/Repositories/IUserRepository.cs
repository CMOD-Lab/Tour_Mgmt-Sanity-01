using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity CRUD operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>Gets all active users.</summary>
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their identifier.</summary>
    Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their email address.</summary>
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Adds a new user.</summary>
    Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user.</summary>
    Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by their identifier.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a user exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an email is already registered.</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email.</summary>
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
