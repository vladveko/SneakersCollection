using Application.Abstractions;
using Application.Sneakers.DeleteSneaker;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Moq;
using Xunit;

namespace UnitTests.Sneakers;

public class DeleteSneakerCommandHandlerTests
{
    [Fact]
    public async Task Handle_SneakerFound_ShouldReturnSuccessResult()
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

        var handler = new DeleteSneakerCommandHandler(
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new DeleteSneakerCommand(sneaker.UserId, sneaker.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_SneakerNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        sneakerRepositoryMock
            .Setup(repo => repo.GetSneakerByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sneaker)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new DeleteSneakerCommandHandler(
            sneakerRepositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new DeleteSneakerCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
    }
}
