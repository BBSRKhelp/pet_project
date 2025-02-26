using PetFamily.Application.DTOs;
using PetFamily.Application.DTOs.Pet;
using PetFamily.Application.Interfaces.Abstractions;

namespace PetFamily.Application.VolunteerAggregate.Commands.Pet.UpdateMainPetInfo;

public record UpdateMainPetInfoCommand(
    Guid VolunteerId,
    Guid PetId,
    string? Name,
    string? Description,
    AppearanceDetailsDto AppearanceDetails,
    AddressDto Address,
    string PhoneNumber,
    DateTime? BirthDate,
    HealthDetailsDto HealthDetails,
    IEnumerable<RequisiteDto>? Requisites,
    BreedAndSpeciesIdDto BreedAndSpeciesId) : ICommand;