using Application.Sneakers.Dtos;

namespace Web.Requests;

public class CreateUpdateSneakerRequest
{
    public string Name { get; init; } = null!;

    public string Brand { get; init; } = null!;

    public MoneyDto Price { get; init; } = null!;

    public ShoeSizeDto Size { get; init; } = null!;

    public RateDto Rate { get; init; } = null!;
}