using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Shell;

namespace PetFamily.Application.Commands.Volunteer.Create;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<CreateVolunteerCommand> _validator;
    private readonly ILogger<CreateVolunteerHandler> _logger;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<CreateVolunteerCommand> validator,
        ILogger<CreateVolunteerHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Volunteer");
        
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var fullname = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.Patronymic).Value;

        var email = Email.Create(command.Email).Value;

        var description = Description.Create(command.Description).Value;

        var workExperience = WorkExperience.Create(command.WorkExperience).Value;

        var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        var socialNetwork = command.SocialNetworks
            ?.Select(x => SocialNetwork.Create(x.Title, x.Url).Value) ?? [];
        var socialNetworks = new SocialNetworksShell(socialNetwork);

        var requisite = command.Requisites
            ?.Select(x => Requisite.Create(x.Title, x.Description).Value) ?? [];
        var requisites = new RequisitesShell(requisite);

        var volunteerEmailResult = await _volunteersRepository.GetByEmailAsync(email, cancellationToken);
        if (volunteerEmailResult.IsSuccess)
        {
            _logger.LogWarning("Volunteer creation failed");
            return (ErrorList)Errors.General.IsExisted(nameof(email));
        }

        var volunteerPhoneResult = await _volunteersRepository.GetByPhoneAsync(phoneNumber, cancellationToken);
        if (volunteerPhoneResult.IsSuccess)
        {
            _logger.LogWarning("Volunteer creation failed");
            return (ErrorList)Errors.General.IsExisted(nameof(phoneNumber));
        }

        var volunteer = new Domain
            .VolunteerAggregate
            .Volunteer(
                fullname,
                email,
                description,
                workExperience,
                phoneNumber,
                socialNetworks,
                requisites);

        var result = await _volunteersRepository.AddAsync(volunteer, cancellationToken);

        _logger.LogInformation("The volunteer was created with the ID: {volunteerId}", result);

        return result;
    }
}