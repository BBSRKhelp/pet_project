using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.VolunteerAggregate.Enums;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record AppearanceDetails
{
    //ef core
    private AppearanceDetails()
    {
    }

    private AppearanceDetails(
        Colour coloration,
        float weight,
        float height)
    {
        Coloration = coloration;
        Weight = weight;
        Height = height;
    }

    public Colour Coloration { get; }
    public float Weight { get; }
    public float Height { get; }

    public static Result<AppearanceDetails, Error> Create(
        Colour coloration,
        float weight,
        float height)

    {
        if (weight is <= 0 or > Constants.MAX_MEDIUM_LOW_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(weight));
        
        if (height is <= 0 or > Constants.MAX_MEDIUM_LOW_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(height));

        return new AppearanceDetails(
            coloration,
            weight,
            height);
    }
}