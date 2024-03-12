using Application.Abstractions;
using Application.Users.Register;
using Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace UnitTests.Users;

public class RegisterCommandHandlerTests
{
        [Fact]
    public async Task Handle_ValidRegistration_ShouldReturnSuccessResult()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(authService => authService.CalculatePasswordHash(It.IsAny<string>()))
            .Returns(("hashedPassword", "saltValue"));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var validatorMock = new Mock<IValidator<RegisterCommand>>();
        validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var userRepositoryMock = new Mock<IUserRepository>();

        var handler = new RegisterCommandHandler(
            authServiceMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object,
            userRepositoryMock.Object);

        var command = new RegisterCommand("test@example.com", "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidRegistration_ShouldReturnValidationFailureResult()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var validatorMock = new Mock<IValidator<RegisterCommand>>();
        validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("PropertyName", "Error Message") }
            });

        var userRepositoryMock = new Mock<IUserRepository>();

        var handler = new RegisterCommandHandler(
            authServiceMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object,
            userRepositoryMock.Object);

        var command = new RegisterCommand("invalidemail", "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValidationFailure);
    }
}