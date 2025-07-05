namespace PetFamily.Volunteers.Infrastructure.Options;

public class SoftDeleteOptions
{
    public const string SOFT_DELETE = "SoftDelete";
    
    public byte SoftDeleteRetentionDays { get; init; }
}