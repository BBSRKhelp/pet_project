using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared.Models;

namespace PetFamily.Application.VolunteerAggregate.Queries.Pet.GetPetById;

public class GetPetByIdValidator : AbstractValidator<GetPetByIdQuery>
{
    public GetPetByIdValidator()
    {
        RuleFor(g => g.PetId).NotEmpty().WithError(Errors.General.IsRequired("PetId"));
    }
}