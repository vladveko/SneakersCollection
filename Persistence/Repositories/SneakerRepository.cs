using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SneakerRepository: ISneakerRepository
{
    private readonly ApplicationDbContext _context;

    public SneakerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Sneaker> GetSneakersQuery(Guid userId)
    {
        return _context.Sneakers.Where(x => x.UserId == userId).AsNoTracking();
    }

    public Task<Sneaker?> GetSneakerByIdAsync(Guid userId, Guid sneakerId, CancellationToken ct = default)
    {
        return _context.Sneakers.SingleOrDefaultAsync(x => x.UserId == userId && x.Id == sneakerId, ct);
    }

    public void Add(Sneaker sneaker)
    {
        _context.Sneakers.Add(sneaker);
    }

    public void Update(Sneaker sneaker)
    {
        _context.Sneakers.Update(sneaker);
    }

    public void Delete(Sneaker sneaker)
    {
        _context.Sneakers.Remove(sneaker);
    }
}