namespace PetFamily.Accounts.Infrastructure.Options;

public class AdminOptions
{
    public const string ADMIN = nameof(ADMIN);

    public string UserName { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Patronymic { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}