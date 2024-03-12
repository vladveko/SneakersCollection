using Application.Abstractions.Messaging;
using Application.Sneakers.Dtos;

namespace Application.Sneakers.GetSneakers;

public record GetSneakersQuery(Guid UserId): IQuery<IQueryable<SneakerDto>>;