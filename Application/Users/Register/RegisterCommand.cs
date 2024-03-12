using Application.Abstractions.Messaging;
using Domain.Repositories;
using FluentValidation;

namespace Application.Users.Register;

public record RegisterCommand(string Email, string Password): ICommand;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .MustAsync(async (email, ct) => await userRepository.IsEmailUniqueAsync(email, ct))
            .WithMessage("Email is not unique.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}