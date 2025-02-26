using PetFamily.Application.Interfaces.Abstractions;

namespace PetFamily.Application.VolunteerAggregate.Queries.Pet.GetPetById;

public record GetPetByIdQuery(Guid PetId) : IQuery;