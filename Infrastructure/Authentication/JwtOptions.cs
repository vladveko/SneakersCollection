namespace Infrastructure.Authentication;

public record JwtOptions
{
    public const string Jwt = "Jwt";
    
    public string Key { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;
}