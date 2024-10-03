using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using static System.String;

namespace PetFamily.Domain.SpeciesAggregate.Entities;

public class Breed : Shared.Models.Entity<BreedId>
{
    //ef core
    private Breed() : base(BreedId.NewId())
    {
    }
    
    private Breed(string name) : base(BreedId.NewId())
    {
        Name = name;
    }

    public string Name { get; } = null!;

    public static Result<Breed, Error> Create(string name)
    {
        if (IsNullOrWhiteSpace(name))
            return Errors.General.IsRequired(nameof(name));
        
        return new Breed(name);
    }
}