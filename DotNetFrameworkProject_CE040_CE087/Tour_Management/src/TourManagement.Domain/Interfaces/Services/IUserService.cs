using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User business operations.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
