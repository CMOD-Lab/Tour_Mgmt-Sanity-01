using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of UserService.
    /// </summary>
    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user with email {Email}", dto.Email);

            if (await _userRepository.ExistsAsync(dto.Email, cancellationToken))
            {
                throw new DuplicateEntityException($"A user with email '{dto.Email}' already exists.");
            }

            var user = _mapper.Map<UserInfo>(dto);
            // Hash password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("Successfully created user with email {Email}", dto.Email);
            return _mapper.Map<UserDto>(created);
        }
        catch (DuplicateEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", dto.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", email);

            var existing = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new NotFoundException(nameof(UserInfo), email);

            _mapper.Map(dto, existing);

            if (!string.IsNullOrEmpty(dto.Password))
            {
                existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            var updated = await _userRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Successfully updated user with email {Email}", email);
            return _mapper.Map<UserDto>(updated);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);

            if (!await _userRepository.ExistsAsync(email, cancellationToken))
            {
                throw new NotFoundException(nameof(UserInfo), email);
            }

            await _userRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("Successfully deleted user with email {Email}", email);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", email);
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValid)
            {
                _logger.LogWarning("Login failed: invalid password for email {Email}", email);
                return null;
            }

            _logger.LogInformation("Login successful for email {Email}", email);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term {SearchTerm}", searchTerm);
            var users = await _userRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
