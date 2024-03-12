using Application.Abstractions;
using Application.Sneakers.Dtos;
using Application.Sneakers.EditSneaker;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace UnitTests.Sneakers;

public class EditSneakerCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessResultWithUpdatedSneaker()
    {
        // Arrange
        var sneaker = Sneaker.Create(
            Guid.NewGuid(),
            "Sneaker",
            "Brand",
            new Money(Currency.USD, 150.00m),
            new ShoeSize(Country.US, 11),
            new Rate(4));
        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        sneakerRepositoryMock
            .Setup(repo => repo.GetSneakerByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sneaker);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<EditSneakerCommand>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<EditSneakerCommand>()))
            .Returns(new ValidationResult());

        var handler = new EditSneakerCommandHandler(
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new EditSneakerCommand
        (
            sneaker.UserId,
            sneaker.Id,
            "Updated Sneaker",
            "Updated Brand",
            new MoneyDto(Currency.USD, 200.00m),
            new ShoeSizeDto(Country.US, 12),
            new RateDto(5)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(command.SneakerId, result.Value.Id);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Brand, result.Value.Brand);
        Assert.Equivalent(command.Price, result.Value.Price);
        Assert.Equivalent(command.Size, result.Value.Size);
        Assert.Equivalent(command.Rate, result.Value.Rate);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldReturnValidationFailureResult()
    {
        // Arrange
        var sneaker = Sneaker.Create(
            Guid.NewGuid(),
            "Sneaker",
            "Brand",
            new Money(Currency.USD, 150.00m),
            new ShoeSize(Country.US, 11),
            new Rate(4));
        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        sneakerRepositoryMock
            .Setup(repo => repo.GetSneakerByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sneaker);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<EditSneakerCommand>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<EditSneakerCommand>()))
            .Returns(new ValidationResult
            {
                Errors = { new ValidationFailure("PropertyName", "Error Message") }
            });

        var handler = new EditSneakerCommandHandler(
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new EditSneakerCommand
        (
            sneaker.UserId,
            sneaker.Id,
            "Updated Sneaker",
            "Updated Brand",
            new MoneyDto(Currency.USD, 200.00m),
            new ShoeSizeDto(Country.US, 12),
            new RateDto(5)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValidationFailure);
    }
    
    [Fact]
    public async Task Handle_SneakerNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        sneakerRepositoryMock.Setup(repo => repo.GetSneakerByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sneaker)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var validatorMock = new Mock<IValidator<EditSneakerCommand>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<EditSneakerCommand>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var handler = new EditSneakerCommandHandler(
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object,
            validatorMock.Object);

        var command = new EditSneakerCommand
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Updated Sneaker",
            "Updated Brand",
            new MoneyDto(Currency.USD, 200.00m),
            new ShoeSizeDto(Country.US, 12),
            new RateDto(5)
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
    }
}