using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Users.Login;

public class LoginCommandHandler: ICommandHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<string>(
                new Error(ErrorCodes.BadRequest, "Incorrect email."));
        }

        var isOk = _authService.VerifyPassword(user, command.Password);

        if (!isOk)
        {
            return Result.Failure<string>(
                new Error(ErrorCodes.BadRequest, "Invalid password."));
        }

        var token = _authService.Generate(user);

        return Result.Success(token);
    }
}