using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record BreedAndSpeciesId
{
    private BreedAndSpeciesId(SpeciesId speciesId, Guid breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }
    
    public SpeciesId SpeciesId { get; } = null!;
    public Guid BreedId { get; }

    public static Result<BreedAndSpeciesId, Error> Create(
        SpeciesId speciesId, 
        Guid breedId)
    {
        if (speciesId == SpeciesId.Empty())
            return Errors.General.IsRequired(nameof(speciesId));
        
        if (breedId == Guid.Empty)
            return Errors.General.IsRequired(nameof(breedId));
        
        return new BreedAndSpeciesId(speciesId, breedId);
    }
}