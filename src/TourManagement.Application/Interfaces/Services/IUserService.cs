using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email asynchronously.
    /// </summary>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    Task<UserDto> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user by email asynchronously.
    /// </summary>
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user login credentials asynchronously.
    /// </summary>
    Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by name or email asynchronously.
    /// </summary>
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
