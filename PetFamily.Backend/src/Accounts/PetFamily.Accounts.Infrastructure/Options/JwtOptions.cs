namespace PetFamily.Accounts.Infrastructure.Options;

public class JwtOptions
{
    public const string JWT = nameof(JWT);
    
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public int AccessTokenLifeTimeMinutes { get; init; }
}