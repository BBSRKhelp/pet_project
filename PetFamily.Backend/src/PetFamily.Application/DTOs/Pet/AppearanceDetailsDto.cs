using PetFamily.Domain.VolunteerAggregate.Enums;

namespace PetFamily.Application.DTOs.Pet;

public record AppearanceDetailsDto(Colour Coloration, float Weight, float Height);