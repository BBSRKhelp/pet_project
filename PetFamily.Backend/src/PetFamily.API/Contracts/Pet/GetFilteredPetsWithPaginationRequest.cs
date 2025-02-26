using PetFamily.Application.VolunteerAggregate.Queries.Pet.GetFilteredPetsWithPagination;
using PetFamily.Domain.VolunteerAggregate.Enums;

namespace PetFamily.API.Contracts.Pet;

public record GetFilteredPetsWithPaginationRequest(
    int PageNumber,
    int PageSize,
    string? Name,
    string? Coloration,
    float? Weight,
    float? Height,
    string? Country,
    string? City,
    string? Street,
    string? PostalCode,
    string? PhoneNumber,
    DateTime? BirthDate,
    string? Status,
    bool? IsCastrated,
    bool? IsVaccinated,
    int? Position,
    Guid? VolunteerId,
    Guid? SpeciesId,
    Guid? BreedId,
    string? SortBy,
    string? SortDirection)
{
    public GetFilteredPetsWithPaginationQuery ToQuery()
    {
        Colour? coloration = Enum.TryParse(Coloration, true, out Colour resultColour)
            ? resultColour
            : null;
        
        Status? status = Enum.TryParse(Status, true, out Status resultStatus)
            ? resultStatus
            : null;

        return new GetFilteredPetsWithPaginationQuery(
            PageNumber,
            PageSize,
            Name,
            coloration,
            Weight,
            Height,
            Country,
            City,
            Street,
            PostalCode,
            PhoneNumber,
            BirthDate,
            status,
            IsCastrated,
            IsVaccinated,
            Position,
            VolunteerId,
            BreedId,
            SpeciesId,
            SortBy ?? "id",
            SortDirection ?? "ASC");
    }
}