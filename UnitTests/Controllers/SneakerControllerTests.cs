using Application.Sneakers.AddSneaker;
using Application.Sneakers.DeleteSneaker;
using Application.Sneakers.Dtos;
using Application.Sneakers.EditSneaker;
using Application.Sneakers.GetSneakers;
using Domain.Enums;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Moq;
using Web.Controllers;
using Web.Requests;
using Web.Services;
using Xunit;

namespace UnitTests.Controllers;

public class SneakerControllerTests
{
    [Fact]
    public async Task Get_ValidUserId_ShouldReturnOkResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<GetSneakersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(Enumerable.Empty<SneakerDto>().AsQueryable()));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        // Act
        var result = await controller.Get(CancellationToken.None);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
    }
    
    [Fact]
    public async Task Get_NonExistingUserId_ShouldReturnNotFoundResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<GetSneakersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IQueryable<SneakerDto>>(new Error("NotFound", "User not found")));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        // Act
        var result = await controller.Get(CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundODataResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task Create_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<AddSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "SneakerName",
            Brand = "BrandName",
            Price = new MoneyDto(Currency.USD, 100.00m),
            Size = new ShoeSizeDto(Country.US, 10),
            Rate = new RateDto(4),
        };

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
    
    [Fact]
    public async Task Create_SneakerNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<AddSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error(ErrorCodes.NotFound, "Sneaker not found")));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "SneakerName",
            Brand = "BrandName",
            Price = new MoneyDto(Currency.USD, 100.00m),
            Size = new ShoeSizeDto(Country.US, 10),
            Rate = new RateDto(4),
        };

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundODataResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task Create_InvalidRequest_ShouldReturnBadRequestResult()
    {
        // Arrange
        var errors = new Dictionary<string, string[]> { { "Name", ["Name is required"] } };
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<AddSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.ValidationFailure(errors));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "SneakerName",
            Brand = "BrandName",
            Price = new MoneyDto(Currency.USD, 100.00m),
            Size = new ShoeSizeDto(Country.US, 10),
            Rate = new RateDto(4),
        };

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal(errors, badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var sneakerDto = new SneakerDto
        {
            Id = Guid.NewGuid(),
            Name = "Sneaker",
            Brand = "Brand",
            Price = new MoneyDto(Currency.USD, 150.00m),
            Size = new ShoeSizeDto(Country.US, 11),
            Rate = new RateDto(4)
        };
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<EditSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(sneakerDto));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "UpdatedSneakerName",
            Brand = "UpdatedBrandName",
            Price = new MoneyDto(Currency.USD, 150.00m),
            Size = new ShoeSizeDto(Country.US, 11),
            Rate = new RateDto(5),
        };

        // Act
        var result = await controller.Update(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equivalent(sneakerDto, okResult.Value);
    }
    
    [Fact]
    public async Task Update_InvalidRequest_ShouldReturnBadRequestResult()
    {
        // Arrange
        var errors = new Dictionary<string, string[]> { { "Brand", ["Brand is required"] } };
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<EditSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.ValidationFailure<SneakerDto>(errors));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid);

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "UpdatedSneakerName",
            Brand = "UpdatedBrandName",
            Price = new MoneyDto(Currency.USD, 150.00m),
            Size = new ShoeSizeDto(Country.US, 11),
            Rate = new RateDto(5),
        };

        // Act
        var result = await controller.Update(Guid.NewGuid(), request, CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal(errors, badRequestResult.Value);
    }

    [Fact]
    public async Task Update_SneakerNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<EditSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<SneakerDto>(new Error(ErrorCodes.NotFound, "Sneaker not found")));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        var request = new CreateUpdateSneakerRequest
        {
            Name = "UpdatedSneakerName",
            Brand = "UpdatedBrandName",
            Price = new MoneyDto(Currency.USD, 150.00m),
            Size = new ShoeSizeDto(Country.US, 11),
            Rate = new RateDto(5),
        };

        // Act
        var result = await controller.Update(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundODataResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ValidRequest_ShouldReturnNoContentResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<DeleteSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        // Act
        var result = await controller.Update(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ErrorResponse_ShouldReturnNotFoundResult()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock.Setup(sender => sender.Send(It.IsAny<DeleteSneakerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error(ErrorCodes.NotFound, "Sneaker not found")));

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.SetupGet(service => service.UserId).Returns(Guid.NewGuid());

        var controller = new SneakersController(senderMock.Object, currentUserServiceMock.Object);

        // Act
        var result = await controller.Update(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundODataResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}