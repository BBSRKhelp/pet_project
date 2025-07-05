using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.Volunteers.Application.Features.Commands.Volunteer.Create;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Volunteer.Delete;

public class DeleteVolunteerHandler(
    IVolunteersRepository volunteersRepository,
    IValidator<DeleteVolunteerCommand> validator,
    [FromKeyedServices(UnitOfWorkContext.Volunteers)] IUnitOfWork unitOfWork,
    ILogger<CreateVolunteerHandler> logger)
    : ICommandHandler<Guid, DeleteVolunteerCommand>
{
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting Volunteer");

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var volunteerResult = await volunteersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            logger.LogWarning("Volunteer delete failed");
            return (ErrorList)volunteerResult.Error;
        }

        volunteerResult.Value.SoftDelete();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("The volunteer with id = {VolunteerId} has been deleted", command.Id);

        return command.Id;
    }
}