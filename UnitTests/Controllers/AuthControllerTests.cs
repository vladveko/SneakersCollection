using Application.Users.Login;
using Application.Users.Register;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Web.Requests;
using Xunit;

namespace UnitTests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnOkResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("mockedToken"));

        var controller = new AuthController(senderMock.Object);

        var loginRequest = new LoginRequest("test@example.com", "password123");

        // Act
        var result = await controller.LoginAsync(loginRequest, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("mockedToken", okResult.Value);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ShouldReturnBadRequestResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<string>(new Error("BadRequest", "Invalid credentials")));

        var controller = new AuthController(senderMock.Object);

        var loginRequest = new LoginRequest("test@example.com", "invalidPassword");

        // Act
        var result = await controller.LoginAsync(loginRequest, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid credentials", badRequestResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var controller = new AuthController(senderMock.Object);

        var registerRequest = new RegisterRequest("test@example.com", "password123");

        // Act
        var result = await controller.RegisterAsync(registerRequest, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_InvalidRequest_ShouldReturnBadRequestResult()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>() { { "Email", [ "Invalid email" ]}};
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.ValidationFailure(errors));

        var controller = new AuthController(senderMock.Object);

        var registerRequest = new RegisterRequest("invalidemail", "password123");

        // Act
        var result = await controller.RegisterAsync(registerRequest, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errors, badRequestResult.Value);
    }
}