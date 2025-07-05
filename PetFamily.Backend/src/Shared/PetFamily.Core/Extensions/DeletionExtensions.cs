using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Core.Extensions;

public static class DeletionExtensions
{
    public static bool ShouldBeHardDeleted<TId>(this SoftDeletableEntity<TId> entity, byte retentionDays)
        where TId : ComparableValueObject
    {
        return entity.DeletionDate != null 
               && DateTime.UtcNow >= entity.DeletionDate.Value.AddDays(retentionDays);
    }
}

