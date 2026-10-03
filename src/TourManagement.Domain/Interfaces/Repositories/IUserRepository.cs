using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity CRUD operations.
/// </summary>
public interface IUserRepository
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
