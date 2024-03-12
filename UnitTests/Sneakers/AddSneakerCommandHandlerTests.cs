using Application.Abstractions;
using Application.Sneakers.AddSneaker;
using Application.Sneakers.Dtos;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace UnitTests.Sneakers;

public class AddSneakerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<AddSneakerCommand>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<AddSneakerCommand>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var handler = new AddSneakerCommandHandler(
            userRepositoryMock.Object,
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new AddSneakerCommand
        (
            Guid.NewGuid(),
            "Test Sneaker",
            "Test Brand",
            new MoneyDto(Currency.USD, 100.00m),
            new ShoeSizeDto(Country.US, 10),
            new RateDto(4)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ShouldReturnFailureResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<AddSneakerCommand>>();

        var handler = new AddSneakerCommandHandler(
            userRepositoryMock.Object,
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new AddSneakerCommand
        (
            Guid.NewGuid(),
            "Test Sneaker",
            "Test Brand",
            new MoneyDto(Currency.USD, 100.00m),
            new ShoeSizeDto(Country.US, 10),
            new RateDto(4)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
    }
    
    [Fact]
    public async Task Handle_ValidationFails_ShouldReturnValidationFailureResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<AddSneakerCommand>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<AddSneakerCommand>()))
            .Returns(new ValidationResult
            {
                Errors = { new ValidationFailure("PropertyName", "Error Message") }
            });

        var handler = new AddSneakerCommandHandler(
            userRepositoryMock.Object,
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new AddSneakerCommand
        (
            Guid.NewGuid(),
            "Test Sneaker",
            "Test Brand",
            new MoneyDto(Currency.USD, 100.00m),
            new ShoeSizeDto(Country.US, 10),
            new RateDto(4)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValidationFailure);
    }
}