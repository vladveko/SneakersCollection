using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using FluentValidation;

namespace Application.Users.Register;

public class RegisterCommandHandler: ICommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterCommandHandler(IAuthService authService, IUnitOfWork unitOfWork, IValidator<RegisterCommand> validator, IUserRepository userRepository)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure<string>(
                validationResult.ToDictionary());
        }

        var (hash, salt) = _authService.CalculatePasswordHash(command.Password);

        var user = User.Create(command.Email, hash, salt);
        
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}