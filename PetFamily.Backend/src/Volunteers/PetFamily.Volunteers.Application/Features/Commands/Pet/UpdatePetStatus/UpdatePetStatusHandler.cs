using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Pet.UpdatePetStatus;

public class UpdatePetStatusHandler : ICommandHandler<Guid, UpdatePetStatusCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<UpdatePetStatusCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePetStatusHandler> _logger;

    public UpdatePetStatusHandler(IVolunteersRepository volunteersRepository,
        IValidator<UpdatePetStatusCommand> validator,
        [FromKeyedServices(UnitOfWorkContext.Volunteers)]
        IUnitOfWork unitOfWork,
        ILogger<UpdatePetStatusHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdatePetStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating pet status with id = {PetId}", command.PetId);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Failed to update pet status");
            return validationResult.ToErrorList();
        }

        var volunteerResult = await _volunteersRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            _logger.LogWarning("Failed to update pet status");
            return (ErrorList)volunteerResult.Error;
        }

        var petResult = volunteerResult.Value.GetPetById(command.PetId);
        if (petResult.IsFailure)
        {
            _logger.LogWarning("Failed to update pet status");
            return (ErrorList)petResult.Error;
        }

        volunteerResult.Value.UpdatePetStatus(petResult.Value, command.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated pet status");

        return petResult.Value.Id.Value;
    }
}