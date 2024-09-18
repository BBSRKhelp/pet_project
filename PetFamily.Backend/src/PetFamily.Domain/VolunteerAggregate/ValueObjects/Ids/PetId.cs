namespace PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;

public record PetId
{
    private PetId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    public static PetId NewId() => new(Guid.NewGuid());
    public static PetId Empty() => new(Guid.Empty);
    public static PetId Create(Guid id) => new(id);
}