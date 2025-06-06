using CSharpFunctionalExtensions;
using PetFamily.Core.Enums;
using PetFamily.SharedKernel;

namespace PetFamily.Volunteers.Domain.ValueObjects;

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