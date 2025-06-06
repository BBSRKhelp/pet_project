using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Volunteer.UpdateMainInfo;

public class UpdateMainVolunteerInfoHandler : ICommandHandler<Guid, UpdateMainVolunteerInfoCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<UpdateMainVolunteerInfoCommand> _validator;
    private readonly ILogger<UpdateMainVolunteerInfoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMainVolunteerInfoHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<UpdateMainVolunteerInfoCommand> validator,
        [FromKeyedServices(UnitOfWorkContext.Volunteer)]IUnitOfWork unitOfWork,
        ILogger<UpdateMainVolunteerInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainVolunteerInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating main info volunteer with id = {VolunteerId}", command.Id);
        
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var volunteerResult = await _volunteersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            _logger.LogWarning("Volunteer update failed");
            return (ErrorList)volunteerResult.Error;
        }

        var fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.Patronymic).Value;

        var email = Email.Create(command.Email).Value;
        var description = Description.Create(command.Description).Value;
        var workExperience = WorkExperience.Create(command.WorkExperience).Value;
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        var volunteerResultByEmail = await _volunteersRepository.GetByEmailAsync(email, cancellationToken);
        if (volunteerResult.Value.Email != email && volunteerResultByEmail.IsSuccess)
        {
            _logger.LogWarning("Volunteer update failed");
            return (ErrorList)Errors.General.IsExisted(nameof(email));
        }

        var volunteerResultByPhone = await _volunteersRepository.GetByPhoneAsync(phoneNumber, cancellationToken);
        if (volunteerResult.Value.PhoneNumber != phoneNumber && volunteerResultByPhone.IsSuccess)
        {
            _logger.LogWarning("Volunteer update failed");
            return (ErrorList)Errors.General.IsExisted(nameof(phoneNumber));
        }

        volunteerResult.Value.UpdateMainInfo(
            fullName,
            email,
            description,
            workExperience,
            phoneNumber);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer with Id = {VolunteerId} has been update", command.Id);

        return volunteerResult.Value.Id.Value;
    }
}