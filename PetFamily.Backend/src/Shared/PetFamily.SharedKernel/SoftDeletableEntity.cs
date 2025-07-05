using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel;

public abstract class SoftDeletableEntity<TId>(TId id) : Entity<TId>(id)
    where TId : ComparableValueObject
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletionDate { get; private set; }

    public virtual void SoftDelete()
    {
        IsDeleted = true;
        DeletionDate = DateTime.UtcNow;
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        DeletionDate = null;
    }
}