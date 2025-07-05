using CSharpFunctionalExtensions;
using PetFamily.Core.Enums;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects.Ids;

namespace PetFamily.Volunteers.Domain;

public class Volunteer() : SoftDeletableEntity<VolunteerId>(VolunteerId.NewId())
{
    private readonly List<Pet> _pets = [];

    public IReadOnlyList<Pet> Pets => _pets.AsReadOnly();

    public int PetsFoundHome() => _pets.Count(p => p.Status == Status.FoundHome);
    public int PetsLookingForHome() => _pets.Count(p => p.Status == Status.LookingForHome);
    public int PetsUndergoingTreatment() => _pets.Count(p => p.Status == Status.UndergoingTreatment);

    public Result<Pet, Error> GetPetById(PetId id)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == id);
        if (pet is null)
            return Errors.General.NotFound(nameof(pet));

        return pet;
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        var positionResult = Position.Create(_pets.Count + 1);
        if (positionResult.IsFailure)
            return positionResult.Error;

        pet.SetPosition(positionResult.Value);

        _pets.Add(pet);
        return Result.Success<Error>();
    }

    public UnitResult<Error> SetMainPetPhoto(Pet pet, PhotoPath photoPath)
    {
        var result = pet.SetMainPhoto(photoPath);

        return result.IsFailure ? result.Error : Result.Success<Error>();
    }

    public void UpdateMainPetInfo(
        Pet pet,
        Name name,
        Description description,
        AppearanceDetails appearanceDetails,
        Address address,
        DateOnly? birthday,
        HealthDetails healthDetails,
        BreedAndSpeciesId breedAndSpeciesId)
    {
        pet.UpdateMainInfo(
            name,
            description,
            appearanceDetails,
            address,
            birthday,
            healthDetails,
            breedAndSpeciesId);
    }

    public void UpdatePetStatus(Pet pet, Status status) => pet.UpdateStatus(status);

    public void AddPetPhotos(Pet pet, IReadOnlyList<PetPhoto> photos) => pet.AddPhotos(photos);

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        var currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
            return UnitResult.Success<Error>();

        var adjustedPosition = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPosition.IsFailure)
            return adjustedPosition.Error;

        newPosition = adjustedPosition.Value;

        var moveResult = MovePetsBetweenPositions(newPosition, currentPosition);
        if (moveResult.IsFailure)
            return moveResult.Error;

        pet.Move(newPosition);

        return UnitResult.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition <= _pets.Count)
            return newPosition;

        var lastPosition = Position.Create(_pets.Count);
        if (lastPosition.IsFailure)
            return lastPosition.Error;

        return lastPosition.Value;
    }

    private UnitResult<Error> MovePetsBetweenPositions(Position newPosition, Position currentPosition)
    {
        if (newPosition < currentPosition)
        {
            var petsToMove = _pets
                .Where(p => p.Position >= newPosition && p.Position < currentPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();
                if (result.IsFailure)
                    return result.Error;
            }
        }

        else if (newPosition > currentPosition)
        {
            var petsToMove = _pets
                .Where(p => p.Position <= newPosition && p.Position > currentPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveBack();
                if (result.IsFailure)
                    return result.Error;
            }
        }

        return Result.Success<Error>();
    }

    private void RecalculatePositionOfOtherPets(Position position)
    {
        var pets = _pets.Where(p => p.Position > position).ToList();

        foreach (var pet in pets)
            pet.SetPosition(Position.Create(pet.Position.Value - 1).Value);
    }

    public void DeletePet(Pet pet)
    {
        RecalculatePositionOfOtherPets(pet.Position);

        _pets.Remove(pet);
    }

    public void SoftDeletePet(Pet pet)
    {
        RecalculatePositionOfOtherPets(pet.Position);

        pet.SoftDelete();
    }

    public UnitResult<Error> RestorePet(Pet pet)
    {
        pet.Restore();

        var moveResult = MovePet(pet, Position.Create(_pets.Count).Value);
        if (moveResult.IsFailure)
            return moveResult.Error;

        return UnitResult.Success<Error>();
    }

    public void DeleteExpiredPets(byte retentionDays) => _pets.RemoveAll(p => p.ShouldBeHardDeleted(retentionDays));

    public override void SoftDelete()
    {
        base.SoftDelete();

        foreach (var pet in _pets)
            pet.SoftDelete();
    }

    public override void Restore()
    {
        base.Restore();

        foreach (var pet in _pets)
            pet.Restore();
    }
}