using FluentValidation;

namespace Application.Sneakers.Dtos;

public record RateDto(int Value);

public class RateDtoValidator : AbstractValidator<RateDto>
{
    public RateDtoValidator()
    {
        RuleFor(x => x.Value).GreaterThan(0).LessThanOrEqualTo(5);
    }
}