using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo business operations.
/// </summary>
public interface IUserService
{
    /// <summary>Gets all active users.</summary>
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their identifier.</summary>
    Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a user by their email address.</summary>
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new user.</summary>
    Task<UserInfo> CreateAsync(UserInfo user, string plainPassword, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user.</summary>
    Task<UserInfo?> UpdateAsync(int id, UserInfo user, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by their identifier.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Validates user credentials and returns the user if valid.</summary>
    Task<UserInfo?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email.</summary>
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
