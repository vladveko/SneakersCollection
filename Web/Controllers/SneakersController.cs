using Application.Sneakers.AddSneaker;
using Application.Sneakers.DeleteSneaker;
using Application.Sneakers.EditSneaker;
using Application.Sneakers.GetSneakers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Web.Requests;
using Web.Services;

namespace Web.Controllers;

public class SneakersController: ODataController
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public SneakersController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }
    
    [EnableQuery]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var query = new GetSneakersQuery(_currentUserService.UserId);
        
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error.Message);
    }
    
    [HttpPost("/api/sneakers")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateSneakerRequest request, CancellationToken cancellationToken)
    {
        var command = new AddSneakerCommand(
            _currentUserService.UserId,
            request.Name,
            request.Brand,
            request.Price,
            request.Size,
            request.Rate);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok() 
            : result.IsValidationFailure 
                ? BadRequest(result.ValidationErrors) 
                : NotFound(result.Error.Message);
    }
    
    [HttpPut("/api/sneakers/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateUpdateSneakerRequest request, CancellationToken cancellationToken)
    {
        var command = new EditSneakerCommand(
            _currentUserService.UserId,
            id,
            request.Name,
            request.Brand,
            request.Price,
            request.Size,
            request.Rate);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.IsValidationFailure 
                ? BadRequest(result.ValidationErrors) 
                : NotFound(result.Error.Message);
    }
    
    [HttpDelete("/api/sneakers/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSneakerCommand(
            _currentUserService.UserId,
            id);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error.Message);
    }
}