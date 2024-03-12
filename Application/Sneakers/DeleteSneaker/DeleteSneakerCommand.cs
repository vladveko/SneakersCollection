using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Sneakers.DeleteSneaker;

public record DeleteSneakerCommand(Guid UserId, Guid SneakerId): ICommand;