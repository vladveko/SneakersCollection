using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Sneakers.AddSneaker;

public class AddSneakerCommandHandler : ICommandHandler<AddSneakerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ISneakerRepository _sneakerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddSneakerCommand> _validator;

    public AddSneakerCommandHandler(
        IUserRepository userRepository,
        ISneakerRepository sneakerRepository, 
        IUnitOfWork unitOfWork, IValidator<AddSneakerCommand> validator)
    {
        _sneakerRepository = sneakerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddSneakerCommand command, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsAsync(command.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(
                new Error(ErrorCodes.NotFound, "User is not found."));
        }

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure(validationResult.ToDictionary());
        }

        var sneaker = Sneaker.Create(
            command.UserId,
            command.Name,
            command.Brand, 
            new Money(command.Price.Currency, command.Price.Amount),
            new ShoeSize(command.Size.Country, command.Size.Value),
            new Rate(command.Rate.Value));
        _sneakerRepository.Add(sneaker);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}