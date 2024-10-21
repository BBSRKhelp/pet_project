using FluentValidation;
using PetFamily.Application.Validators;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;

namespace PetFamily.Application.Commands.Volunteer.Create;

public class VolunteerCreateCommandValidator : AbstractValidator<VolunteerCreateCommand>
{
    public VolunteerCreateCommandValidator()
    {
        RuleFor(c => c.FullName)
            .MustBeValueObject(fnd => FullName.Create(fnd.FirstName, fnd.LastName, fnd.Patronymic));
        
        RuleFor(c => c.Email).MustBeValueObject(Email.Create);
        
        RuleFor(c => c.Description).MustBeValueObject(Description.Create);
        
        RuleFor(c => c.WorkExperience).MustBeValueObject(WorkExperience.Create);
        
        RuleFor(c => c.PhoneNumber).MustBeValueObject(PhoneNumber.Create);
        
        RuleForEach(c => c.SocialNetworks)
            .MustBeValueObject(snd => SocialNetwork.Create(snd.Title, snd.Url));
        
        RuleForEach(c => c.Requisites)
            .MustBeValueObject(rd => Requisite.Create(rd.Title, rd.Description));
    }
}