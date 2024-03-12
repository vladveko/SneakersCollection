using Application.Abstractions.Messaging;
using Application.Sneakers.Dtos;
using FluentValidation;

namespace Application.Sneakers.EditSneaker;

public record EditSneakerCommand(
    Guid UserId,
    Guid SneakerId,
    string Name,
    string Brand,
    MoneyDto Price,
    ShoeSizeDto Size,
    RateDto Rate) : ICommand<SneakerDto>;
    
public class EditSneakerCommandValidator: AbstractValidator<EditSneakerCommand>
{
    public EditSneakerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rate).SetValidator(new RateDtoValidator());
    }
}