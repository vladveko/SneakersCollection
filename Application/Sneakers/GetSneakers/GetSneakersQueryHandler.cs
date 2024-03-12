using Application.Abstractions.Messaging;
using Application.Sneakers.Dtos;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Sneakers.GetSneakers;

public class GetSneakersQueryHandler : IQueryHandler<GetSneakersQuery, IQueryable<SneakerDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISneakerRepository _sneakerRepository;

    public GetSneakersQueryHandler(IUserRepository userRepository, ISneakerRepository sneakerRepository)
    {
        _userRepository = userRepository;
        _sneakerRepository = sneakerRepository;
    }

    public async Task<Result<IQueryable<SneakerDto>>> Handle(GetSneakersQuery query, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsAsync(query.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure<IQueryable<SneakerDto>>(
                new Error(ErrorCodes.NotFound, "User is not found."));
        }

        var sneakersQuery = _sneakerRepository
            .GetSneakersQuery(query.UserId)
            .Select(x => new SneakerDto
            {
                Id = x.Id,
                Name = x.Name,
                Brand = x.Brand,
                Price = new MoneyDto(x.Price.Currency, x.Price.Amount),
                Rate = new RateDto(x.Rate.Value),
                Size = new ShoeSizeDto(x.Size.Country, x.Size.Value)
            });
        
        return Result.Success(sneakersQuery);
    }
}