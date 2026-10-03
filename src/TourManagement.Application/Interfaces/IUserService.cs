using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for User business operations.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
