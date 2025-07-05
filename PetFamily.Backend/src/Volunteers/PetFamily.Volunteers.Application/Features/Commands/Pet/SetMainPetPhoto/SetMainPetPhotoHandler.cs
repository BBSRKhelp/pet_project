using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Application.Interfaces;

namespace PetFamily.Volunteers.Application.Features.Commands.Pet.SetMainPetPhoto;

public class SetMainPetPhotoHandler : ICommandHandler<Guid, SetMainPetPhotoCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<SetMainPetPhotoCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SetMainPetPhotoHandler> _logger;

    public SetMainPetPhotoHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<SetMainPetPhotoCommand> validator,
        [FromKeyedServices(UnitOfWorkContext.Volunteers)]
        IUnitOfWork unitOfWork,
        ILogger<SetMainPetPhotoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SetMainPetPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting main pet photo with id = {PetId}", command.PetId);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Set main pet photo failed");
            return validationResult.ToErrorList();
        }

        var volunteerResult = await _volunteersRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            _logger.LogWarning("Set main pet photo failed");
            return volunteerResult.Error.ToErrorList();
        }

        var petResult = volunteerResult.Value.GetPetById(command.PetId);
        if (petResult.IsFailure)
        {
            _logger.LogWarning("Set main pet photo failed");
            return petResult.Error.ToErrorList();
        }

        var photoPath = PhotoPath.Create(command.PhotoPath).Value;

        var result = volunteerResult.Value.SetMainPetPhoto(petResult.Value, photoPath);
        if (result.IsFailure)
        {
            _logger.LogWarning("Set main pet photo failed");
            return result.Error.ToErrorList();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Set main pet photo success with id = {PetId}", command.PetId);

        return petResult.Value.Id.Value;
    }
}