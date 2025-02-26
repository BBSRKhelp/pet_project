using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Extensions;
using PetFamily.Application.Interfaces.Abstractions;
using PetFamily.Application.Interfaces.Database;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;

namespace PetFamily.Application.VolunteerAggregate.Commands.Pet.UpdateMainPetInfo;

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
        IUnitOfWork unitOfWork,
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

        var isExistBreedAndSpecies = await _speciesAndBreedValidator
            .IsExist(command.BreedAndSpeciesId, cancellationToken);
        if (isExistBreedAndSpecies.IsFailure)
        {
            _logger.LogWarning("Failed to update pet");
            return isExistBreedAndSpecies.Error;
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

        var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        var healthDetails = HealthDetails.Create(
            command.HealthDetails.HealthInformation,
            command.HealthDetails.IsCastrated,
            command.HealthDetails.IsVaccinated).Value;

        var requisites = command
            .Requisites
            ?.Select(x => Requisite.Create(x.Title, x.Description).Value).ToArray() ?? [];

        var breedAndSpeciesId = BreedAndSpeciesId.Create(
            command.BreedAndSpeciesId.SpeciesId,
            command.BreedAndSpeciesId.BreedId).Value;

        petResult.Value.UpdateMainInfo(
            name,
            description,
            appearanceDetails,
            address,
            phoneNumber,
            command.BirthDate,
            healthDetails,
            requisites,
            breedAndSpeciesId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet with Id = {PetId} has been update", command.PetId);

        return petResult.Value.Id.Value;
    }
}