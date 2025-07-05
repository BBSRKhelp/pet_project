namespace PetFamily.Accounts.Infrastructure.Options;

public class RefreshOptions
{
    public const string REFRESH = nameof(REFRESH);

    public byte RefreshTokenLifeTimeDays { get; init; }
}