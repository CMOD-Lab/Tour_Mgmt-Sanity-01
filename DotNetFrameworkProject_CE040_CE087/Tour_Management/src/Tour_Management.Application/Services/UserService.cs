using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Application.Services;

/// <summary>
/// Service implementation for User business operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    public async Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _userRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a user by email.
    /// </summary>
    public async Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<UserInfo> RegisterUserAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new user: {Email}", user.Email);
            var existing = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException($"User with email '{user.Email}' already exists.");

            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            return await _userRepository.AddAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", user.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    public async Task<UserInfo> UpdateUserAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user: {Email}", user.Email);
            var existing = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existing == null)
                throw new NotFoundException(nameof(UserInfo), user.Email);

            user.ModifiedDate = DateTime.UtcNow;
            return await _userRepository.UpdateAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {Email}", user.Email);
            throw;
        }
    }

    /// <summary>
    /// Deletes a user by email.
    /// </summary>
    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user: {Email}", email);
            var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existing == null)
                throw new NotFoundException(nameof(UserInfo), email);

            await _userRepository.DeleteAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    public async Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);
            return await _userRepository.AuthenticateAsync(email, password, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw;
        }
    }
}
