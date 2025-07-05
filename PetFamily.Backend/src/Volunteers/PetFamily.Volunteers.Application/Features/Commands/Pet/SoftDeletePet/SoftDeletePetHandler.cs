using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Pet.SoftDeletePet;

public class SoftDeletePetHandler(
    IVolunteersRepository volunteersRepository,
    IValidator<SoftDeletePetCommand> validator,
    [FromKeyedServices(UnitOfWorkContext.Volunteers)]
    IUnitOfWork unitOfWork,
    ILogger<SoftDeletePetHandler> logger)
    : ICommandHandler<Guid, SoftDeletePetCommand>
{
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SoftDeletePetCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting soft-deletion pet with id = {PetId}", command.PetId);
        
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Failed soft-deletion pet");
            return validationResult.ToErrorList();
        }
        
        var volunteerResult = await volunteersRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            logger.LogWarning("Failed soft-deletion pet");
            return (ErrorList)volunteerResult.Error;
        }
        
        var petResult = volunteerResult.Value.GetPetById(command.PetId);
        if (petResult.IsFailure)
        {
            logger.LogWarning("Failed soft-deletion pet");
            return (ErrorList)petResult.Error;
        }
        
        volunteerResult.Value.SoftDeletePet(petResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully soft-deletion pet with id = {PetId}", command.PetId);
        
        return petResult.Value.Id.Value;
    }
}