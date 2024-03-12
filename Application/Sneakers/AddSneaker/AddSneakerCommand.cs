

using Application.Abstractions.Messaging;
using Application.Sneakers.Dtos;
using FluentValidation;

namespace Application.Sneakers.AddSneaker;

public record AddSneakerCommand(
    Guid UserId,
    string Name,
    string Brand,
    MoneyDto Price,
    ShoeSizeDto Size,
    RateDto Rate) : ICommand;
    
public class AddSneakerCommandValidator: AbstractValidator<AddSneakerCommand>
{
    public AddSneakerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rate).SetValidator(new RateDtoValidator());
    }
}