using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    
    Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);

    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default);
    
    void Add(User user);
}