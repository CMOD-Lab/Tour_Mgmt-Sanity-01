using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    /// <summary>Initializes a new instance of <see cref="UserService"/>.</summary>
    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
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

    /// <inheritdoc/>
    public async Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", id);
            return await _userRepository.GetByIdAsync(id, cancellationToken);
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
            _logger.LogInformation("Retrieving user with email {Email}", email);
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo> CreateAsync(UserInfo user, string plainPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", user.Email);

            var emailExists = await _userRepository.EmailExistsAsync(user.Email, cancellationToken);
            if (emailExists)
                throw new DuplicateEntityException("UserInfo", "Email", user.Email);

            user.PasswordHash = HashPassword(plainPassword);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            user.Role = "User";

            var created = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created with ID {UserId}", created.Id);
            return created;
        }
        catch (DuplicateEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> UpdateAsync(int id, UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID {UserId}", id);
            var existing = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return null;
            }

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Gender = user.Gender;
            existing.DateOfBirth = user.DateOfBirth;
            existing.Street = user.Street;
            existing.City = user.City;
            existing.State = user.State;
            existing.IsActive = user.IsActive;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = user.ModifiedBy;

            var updated = await _userRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("User with ID {UserId} updated successfully", id);
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);
            var result = await _userRepository.DeleteAsync(id, cancellationToken);
            if (result)
                _logger.LogInformation("User with ID {UserId} deleted successfully", id);
            else
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating credentials for email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null || !user.IsActive)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            return await _userRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>Hashes a plain-text password.</summary>
    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>Verifies a plain-text password against a stored hash.</summary>
    private static bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Legacy plain-text comparison for migrated data
            return password == hash;
        }
    }
}
