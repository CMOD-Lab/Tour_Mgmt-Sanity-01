using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for User business operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active users.
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new user with hashed password.
    /// </summary>
    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", dto.Email);

            if (await _userRepository.EmailExistsAsync(dto.Email, cancellationToken))
                throw new DuplicateEntityException(nameof(User), "Email", dto.Email);

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            var created = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created with ID {UserId}", created.Id);
            return _mapper.Map<UserDto>(created);
        }
        catch (DuplicateEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    public async Task<UserDto> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID {UserId}", id);
            var existing = await _userRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), id);

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.UtcNow;
            var updated = await _userRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("User with ID {UserId} updated successfully", id);
            return _mapper.Map<UserDto>(updated);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);
            if (!await _userRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(User), id);

            await _userRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("User with ID {UserId} deleted successfully", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches users by a search term.
    /// </summary>
    public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user by email and password.
    /// </summary>
    public async Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Authentication failed for email: {Email}", email);
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                _logger.LogWarning("Invalid password for email: {Email}", email);
                return null;
            }

            _logger.LogInformation("User {Email} authenticated successfully", email);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw;
        }
    }
}
