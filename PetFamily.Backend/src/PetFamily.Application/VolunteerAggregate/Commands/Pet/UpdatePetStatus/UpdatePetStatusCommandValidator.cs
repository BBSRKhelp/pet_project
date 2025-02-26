using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.VolunteerAggregate.Enums;

namespace PetFamily.Application.VolunteerAggregate.Commands.Pet.UpdatePetStatus;

public class UpdatePetStatusCommandValidator : AbstractValidator<UpdatePetStatusCommand>
{
    public UpdatePetStatusCommandValidator()
    {
        RuleFor(u => u.VolunteerId).NotEmpty().WithError(Errors.General.IsRequired("VolunteerId"));

        RuleFor(u => u.PetId).NotEmpty().WithError(Errors.General.IsRequired("PetId"));
        
        RuleFor(u => u.Status)
            .IsInEnum()
            .Must(x => x != Status.Unknown)
            .WithError(Errors.General.IsInvalid("Status"));
    }
}