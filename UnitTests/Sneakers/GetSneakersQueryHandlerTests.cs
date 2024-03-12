using Application.Sneakers.GetSneakers;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Moq;
using Xunit;

namespace UnitTests.Sneakers;

public class GetSneakersQueryHandlerTests
{
    [Fact]
    public async Task Handle_UserExists_ShouldReturnSneakersQueryResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sneakerRepositoryMock = new Mock<ISneakerRepository>();
        sneakerRepositoryMock
            .Setup(repo => repo.GetSneakersQuery(It.IsAny<Guid>()))
            .Returns(new List<Sneaker>
            {
                Sneaker.Create(
                Guid.NewGuid(),
                "Sneaker",
                "Brand",
                new Money(Currency.USD, 150.00m),
                new ShoeSize(Country.US, 11),
                new Rate(4)),
            }.AsQueryable());

        var handler = new GetSneakersQueryHandler(
            userRepositoryMock.Object,
            sneakerRepositoryMock.Object);

        var query = new GetSneakersQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Count());
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sneakerRepositoryMock = new Mock<ISneakerRepository>();

        var handler = new GetSneakersQueryHandler(
            userRepositoryMock.Object,
            sneakerRepositoryMock.Object);

        var query = new GetSneakersQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCodes.NotFound, result.Error.Code);
    }
}