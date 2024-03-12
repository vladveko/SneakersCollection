using Domain.Entities;

namespace Application.Abstractions;

public interface IAuthService
{
    string Generate(User user);

    bool VerifyPassword(User user, string password);

    (string passwordHash, string salt) CalculatePasswordHash(string password);
}