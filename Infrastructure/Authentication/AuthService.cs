using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class AuthService: IAuthService
{
    private readonly JwtOptions _jwtOptions;

    public AuthService(IOptionsSnapshot<JwtOptions> optionsSnapshot)
    {
        _jwtOptions = optionsSnapshot.Value;
    }

    public string Generate(User user)
    {
        var claims = new Claim[]
        {
            new(Claims.UserId, user.Id.ToString()),
            new(Claims.Email, user.Email)
        };

        var signingCreds = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(2),
            signingCreds);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }

    public bool VerifyPassword(User user, string password)
    {
        using var sha256 = SHA256.Create();
        var enteredPasswordHash = Convert.ToBase64String(sha256.ComputeHash(
            Encoding.UTF8.GetBytes(password + user.Salt)));
        return enteredPasswordHash.Equals(user.PasswordHash);
    }

    public (string passwordHash, string salt) CalculatePasswordHash(string password)
    {
        using var sha256 = SHA256.Create();
        var salt = GenerateSalt();
        var passwordWithSalt = password + salt;
        var passwordHash = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordWithSalt)));
        return (passwordHash, salt: salt);
    }
    
    private string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
}