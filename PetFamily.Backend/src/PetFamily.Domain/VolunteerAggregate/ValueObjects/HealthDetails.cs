using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record HealthDetails
{
    private HealthDetails(
        string healthInformation,
        bool isCastrated,
        bool isVaccinated)
    {
        HealthInformation = healthInformation;
        IsCastrated = isCastrated;
        IsVaccinated = isVaccinated;
    }

    public string HealthInformation { get; }
    public bool IsCastrated { get; }
    public bool IsVaccinated { get; }

    public static Result<HealthDetails, Error> Create(
        string healthInformation,
        bool isCastrated,
        bool isVaccinated)
    {
        if (IsNullOrEmpty(healthInformation) || healthInformation.Length > Constants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(healthInformation));
        
        return new HealthDetails(
            healthInformation,
            isCastrated,
            isVaccinated);
    }
}