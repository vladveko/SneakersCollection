using Domain.Entities;

namespace Domain.Repositories;

public interface ISneakerRepository
{
    IQueryable<Sneaker> GetSneakersQuery(Guid userId);

    Task<Sneaker?> GetSneakerByIdAsync(Guid userId, Guid sneakerId, CancellationToken ct = default);

    void Add(Sneaker sneaker);

    void Update(Sneaker sneaker);

    void Delete(Sneaker sneaker);
}