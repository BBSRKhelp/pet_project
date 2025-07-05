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
using PetFamily.Volunteers.Application.Validation;

namespace PetFamily.Volunteers.Application.Features.Commands.Pet.UpdateMainPetInfo;

public class UpdateMainPetInfoHandler : ICommandHandler<Guid, UpdateMainPetInfoCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IValidator<UpdateMainPetInfoCommand> _validator;
    private readonly SpeciesAndBreedValidator _speciesAndBreedValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMainPetInfoHandler> _logger;

    public UpdateMainPetInfoHandler(
        IVolunteersRepository volunteersRepository,
        IValidator<UpdateMainPetInfoCommand> validator,
        SpeciesAndBreedValidator speciesAndBreedValidator,
        [FromKeyedServices(UnitOfWorkContext.Volunteers)]IUnitOfWork unitOfWork,
        ILogger<UpdateMainPetInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _validator = validator;
        _speciesAndBreedValidator = speciesAndBreedValidator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainPetInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating pet main info with id = {PetId}", command.PetId);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Failed to update pet main info");
            return validationResult.ToErrorList();
        }

        var volunteerResult = await _volunteersRepository.GetByIdAsync(command.VolunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
        {
            _logger.LogWarning("Failed to update pet");
            return (ErrorList)volunteerResult.Error;
        }

        var petResult = volunteerResult.Value.GetPetById(command.PetId);
        if (petResult.IsFailure)
        {
            _logger.LogWarning("Failed to update pet");
            return (ErrorList)petResult.Error;
        }

        var speciesAndBreedExists = await _speciesAndBreedValidator
            .IsExist(command.BreedAndSpeciesId, cancellationToken);
        if (speciesAndBreedExists.IsFailure)
        {
            _logger.LogWarning("Failed to update pet");
            return speciesAndBreedExists.Error;
        }

        var name = Name.Create(command.Name).Value;

        var description = Description.Create(command.Description).Value;

        var appearanceDetails = AppearanceDetails.Create(
            command.AppearanceDetails.Coloration,
            command.AppearanceDetails.Weight,
            command.AppearanceDetails.Height).Value;

        var address = Address.Create(
            command.Address.Country,
            command.Address.City,
            command.Address.Street,
            command.Address.PostalCode).Value;

        var healthDetails = HealthDetails.Create(
            command.HealthDetails.HealthInformation,
            command.HealthDetails.IsCastrated,
            command.HealthDetails.IsVaccinated).Value;

        var breedAndSpeciesId = BreedAndSpeciesId.Create(
            command.BreedAndSpeciesId.SpeciesId,
            command.BreedAndSpeciesId.BreedId).Value;

        volunteerResult.Value.UpdateMainPetInfo(
            petResult.Value,
            name,
            description,
            appearanceDetails,
            address,
            command.BirthDate,
            healthDetails,
            breedAndSpeciesId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet with Id = {PetId} has been update", command.PetId);

        return petResult.Value.Id.Value;
    }
}