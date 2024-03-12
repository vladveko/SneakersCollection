namespace Web.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}