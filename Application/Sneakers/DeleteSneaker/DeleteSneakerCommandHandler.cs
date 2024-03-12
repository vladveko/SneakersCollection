using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Sneakers.DeleteSneaker;

public class DeleteSneakerCommandHandler : ICommandHandler<DeleteSneakerCommand>
{
    private readonly ISneakerRepository _sneakerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSneakerCommandHandler(
        ISneakerRepository sneakerRepository, 
        IUnitOfWork unitOfWork)
    {
        _sneakerRepository = sneakerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSneakerCommand command, CancellationToken cancellationToken)
    {
        var sneaker =
            await _sneakerRepository.GetSneakerByIdAsync(command.UserId, command.SneakerId, cancellationToken);

        if (sneaker is null)
        {
            return Result.Failure(
                new Error(ErrorCodes.NotFound, "Sneaker is not found."));
        }
        
        _sneakerRepository.Delete(sneaker);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}