using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Sneakers.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Sneakers.EditSneaker;

public class EditSneakerCommandHandler: ICommandHandler<EditSneakerCommand, SneakerDto>
{
    private readonly ISneakerRepository _sneakerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<EditSneakerCommand> _validator;

    public EditSneakerCommandHandler(ISneakerRepository sneakerRepository, IUnitOfWork unitOfWork, IValidator<EditSneakerCommand> validator)
    {
        _sneakerRepository = sneakerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<SneakerDto>> Handle(EditSneakerCommand command, CancellationToken cancellationToken)
    {
        var sneaker =
            await _sneakerRepository.GetSneakerByIdAsync(command.UserId, command.SneakerId, cancellationToken);

        if (sneaker is null)
        {
            return Result.Failure<SneakerDto>(
                new Error(ErrorCodes.NotFound, "Sneaker is not found."));
        }
        
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure<SneakerDto>(validationResult.ToDictionary());
        }
        
        sneaker.Update(
            command.Name,
            command.Brand, 
            new Money(command.Price.Currency, command.Price.Amount),
            new ShoeSize(command.Size.Country, command.Size.Value),
            new Rate(command.Rate.Value));
        _sneakerRepository.Update(sneaker);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new SneakerDto
        {
            Id = sneaker.Id,
            Name = sneaker.Name,
            Brand = sneaker.Brand,
            Price = new MoneyDto(sneaker.Price.Currency, sneaker.Price.Amount),
            Rate = new RateDto(sneaker.Rate.Value),
            Size = new ShoeSizeDto(sneaker.Size.Country, sneaker.Size.Value)
        });
    }
}    