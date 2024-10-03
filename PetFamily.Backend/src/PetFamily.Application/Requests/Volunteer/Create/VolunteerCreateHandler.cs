using CSharpFunctionalExtensions;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;

namespace PetFamily.Application.Requests.Volunteer.Create;

public class VolunteerCreateHandler(IVolunteersRepository volunteersRepository)
{
    public async Task<Result<Guid, Error>> HandleAsync(
        VolunteerCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var fullnameResult = Fullname.Create(
            request.FirstName,
            request.LastName, 
            request.Patronymic);
        if (fullnameResult.IsFailure) 
            return fullnameResult.Error;
        
        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult.Error;
        
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;
        
        var volunteerPhone= await volunteersRepository.
            GetByPhoneAsync(phoneNumberResult.Value, cancellationToken);
        if (volunteerPhone.IsSuccess)
            return Errors.General.IsExisted(nameof(volunteerPhone));
        
        var volunteerEmail = await volunteersRepository.
            GetByEmailAsync(emailResult.Value, cancellationToken);
        if (volunteerEmail.IsSuccess)
            return Errors.General.IsExisted(nameof(volunteerEmail));
        
        var volunteerDetails = new VolunteerDetails(request.SocialNetworks, request.Requisites);
        
        //Deconstruction (isSuccess, isFailure, volunteer, error)
        var (_, isFailure, volunteer, error) = 
            Domain.
            VolunteerAggregate.
            Volunteer.Create(
                fullnameResult.Value,
                emailResult.Value,
                request.Description,
                request.WorkExperience,
                phoneNumberResult.Value,
                volunteerDetails,
                request.Pets);
        if (isFailure)
            return error; ;

        await volunteersRepository.AddAsync(volunteer, cancellationToken);
        
        return (Guid)volunteer.Id;
    }
}