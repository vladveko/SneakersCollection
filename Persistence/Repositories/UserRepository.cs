using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return _context.Users.SingleOrDefaultAsync(x => x.Id == userId, ct);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        return _context.Users.SingleOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default)
    {
        return !await _context.Users.AnyAsync(x => x.Email == email, ct);
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default)
    {
        return _context.Users.AnyAsync(x => x.Id == userId, ct);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}