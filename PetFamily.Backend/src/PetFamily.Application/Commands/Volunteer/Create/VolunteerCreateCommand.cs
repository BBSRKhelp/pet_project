using PetFamily.Application.Dto;

namespace PetFamily.Application.Commands.Volunteer.Create;

public record VolunteerCreateCommand(
    FullNameDto FullName,
    string Email,
    string? Description,
    byte WorkExperience,
    string PhoneNumber,
    IEnumerable<SocialNetworkDto>? SocialNetworks,
    IEnumerable<RequisiteDto>? Requisites);