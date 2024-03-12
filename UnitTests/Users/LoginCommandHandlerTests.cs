using Application.Abstractions;
using Application.Users.Login;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Moq;
using Xunit;

namespace UnitTests.Users;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidLoginCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = User.Create("test@example.com", "akjncjaknas", "sjndvkjsdnv");
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(authService => authService.VerifyPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(true);
        authServiceMock.Setup(authService => authService.Generate(It.IsAny<User>()))
            .Returns("mockedToken");

        var handler = new LoginCommandHandler(
            userRepositoryMock.Object,
            authServiceMock.Object);

        var command = new LoginCommand(user.Email, "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("mockedToken", result.Value);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ShouldReturnFailureResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var authServiceMock = new Mock<IAuthService>();

        var handler = new LoginCommandHandler(
            userRepositoryMock.Object,
            authServiceMock.Object);

        var command = new LoginCommand("nonexistent@example.com", "password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.BadRequest, result.Error.Code);
        // Add more specific assertions based on your custom error handling
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldReturnFailureResult()
    {
        // Arrange
        var user = User.Create("test@example.com", "akjncjaknas", "sjndvkjsdnv");
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(authService => authService.VerifyPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(false);

        var handler = new LoginCommandHandler(
            userRepositoryMock.Object,
            authServiceMock.Object);

        var command = new LoginCommand(user.Email, "invalidPassword");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.BadRequest, result.Error.Code);
    }
}