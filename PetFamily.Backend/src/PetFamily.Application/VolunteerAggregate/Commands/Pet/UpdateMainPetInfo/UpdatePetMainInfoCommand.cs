using PetFamily.Application.DTOs;
using PetFamily.Application.DTOs.Pet;
using PetFamily.Application.Interfaces.Abstractions;

namespace PetFamily.Application.VolunteerAggregate.Commands.Pet.UpdateMainPetInfo;

public record UpdatePetMainInfoCommand(
    Guid VolunteerId,
    Guid PetId,
    string? Name,
    string? Description,
    AppearanceDetailsDto AppearanceDetails,
    AddressDto Address,
    string PhoneNumber,
    DateOnly? Birthday,
    HealthDetailsDto HealthDetails,
    IEnumerable<RequisiteDto>? Requisites,
    BreedAndSpeciesIdDto BreedAndSpeciesId) : ICommand;