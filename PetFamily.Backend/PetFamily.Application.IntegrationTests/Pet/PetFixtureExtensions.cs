using AutoFixture;
using PetFamily.Application.DTOs.Pet;
using PetFamily.Application.VolunteerAggregate.Commands.Pet.AddPet;
using PetFamily.Application.VolunteerAggregate.Commands.Pet.ChangePetsPosition;
using PetFamily.Application.VolunteerAggregate.Commands.Pet.UpdateMainPetInfo;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate.Enums;

namespace PetFamily.Application.IntegrationTests.Pet;

public static class PetFixtureExtensions
{
    public static AddPetCommand BuildAddPetCommand(
        this Fixture fixture,
        Guid volunteerId,
        SpeciesId speciesId,
        Guid breedId)
    {
        return fixture.Build<AddPetCommand>()
            .With(p => p.BirthDate, DateTime.Now)
            .With(p => p.VolunteerId, volunteerId)
            .With(p => p.Name, (string?)null)
            .With(p => p.Description, (string?)null)
            .With(p => p.AppearanceDetails, new AppearanceDetailsDto(Colour.Black, 12, 12))
            .With(p => p.HealthDetails, new HealthDetailsDto("test", true, true))
            .With(p => p.Address, new AddressDto("co", "ci", "st", null))
            .With(p => p.PhoneNumber, "79169166666")
            .With(p => p.BreedAndSpeciesId, new BreedAndSpeciesIdDto(speciesId, breedId))
            .With(p => p.Status, Status.FoundHome)
            .Create();
    }

    public static ChangePetsPositionCommand BuildChangePetsPositionCommand(
        this Fixture fixture,
        Guid volunteerId,
        Guid petId,
        int newPosition)
    {
        return fixture.Build<ChangePetsPositionCommand>()
            .With(p => p.VolunteerId, volunteerId)
            .With(p => p.PetId, petId)
            .With(p => p.NewPosition, newPosition)
            .Create();
    }

    public static UpdateMainPetInfoCommand BuildUpdateMainPetInfoCommand(
        this Fixture fixture,
        Guid volunteerId,
        Guid petId,
        SpeciesId speciesId,
        Guid breedId,
        string phoneNumber)
    {
        return fixture.Build<UpdateMainPetInfoCommand>()
            .With(p => p.VolunteerId, volunteerId)
            .With(p => p.PetId, petId)
            .With(p => p.AppearanceDetails, new AppearanceDetailsDto(Colour.Black, 12, 12))
            .With(p => p.Address, new AddressDto("co", "ci", "st", null))
            .With(p => p.PhoneNumber, phoneNumber)
            .With(p => p.BirthDate, DateTime.Now)
            .With(p => p.HealthDetails, new HealthDetailsDto("test", true, true))
            .With(p => p.BreedAndSpeciesId, new BreedAndSpeciesIdDto(speciesId, breedId))
            .Create();
    }
}