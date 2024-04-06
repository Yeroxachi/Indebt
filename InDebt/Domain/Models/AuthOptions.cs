namespace Domain.Models;

public record AuthOptions 
{
    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int AccessTokenLifeTime { get; init; }
    public int RefreshTokenLifeTime { get; init; }
}