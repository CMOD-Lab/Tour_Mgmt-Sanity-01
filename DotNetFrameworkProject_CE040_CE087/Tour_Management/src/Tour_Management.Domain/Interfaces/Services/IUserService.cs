using Tour_Management.Domain.Entities;

namespace Tour_Management.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User business operations.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> RegisterUserAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateUserAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
