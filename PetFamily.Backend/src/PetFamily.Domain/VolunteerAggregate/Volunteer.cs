using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Shell;

namespace PetFamily.Domain.VolunteerAggregate;

public class Volunteer : Shared.Models.Entity<VolunteerId>
{
    private bool _isDeleted = false;

    private readonly List<Pet> _pets = [];

    //ef core
    private Volunteer() : base(VolunteerId.NewId())
    {
    }

    public Volunteer(
        FullName fullName,
        Email email,
        Description? description,
        WorkExperience workExperience,
        PhoneNumber phoneNumber,
        SocialNetworksShell? socialNetwork,
        RequisitesShell? requisites)
        : base(VolunteerId.NewId())
    {
        FullName = fullName;
        Email = email;
        Description = description;
        WorkExperience = workExperience;
        PhoneNumber = phoneNumber;
        SocialNetworks = socialNetwork;
        Requisites = requisites;
    }

    public FullName FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Description? Description { get; private set; }
    public WorkExperience WorkExperience { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public SocialNetworksShell? SocialNetworks { get; private set; }
    public RequisitesShell? Requisites { get; private set; }
    public IReadOnlyList<Pet> Pets => _pets.AsReadOnly();

    public int PetsFoundHome() => _pets.Count(p => p.Status == StatusForHelp.FoundHome);
    public int PetsLookingForHome() => _pets.Count(p => p.Status == StatusForHelp.LookingForHome);
    public int PetsNeedHelp() => _pets.Count(p => p.Status == StatusForHelp.NeedsHelp);

    public void UpdateMainInfo(
        FullName fullName,
        Email email,
        Description? description,
        WorkExperience workExperience,
        PhoneNumber phoneNumber)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        WorkExperience = workExperience;
        PhoneNumber = phoneNumber;
    }

    public void UpdateRequisite(RequisitesShell requisite)
    {
        Requisites = requisite;
    }

    public void UpdateSocialNetwork(SocialNetworksShell socialNetworks)
    {
        SocialNetworks = socialNetworks;
    }

    public void IsActivate()
    {
        _isDeleted = false;

        foreach (var pet in _pets)
        {
            pet.IsActivate();
        }
    }

    public void IsDeactivate()
    {
        _isDeleted = true;

        foreach (var pet in _pets)
            pet.IsDeactivate();
    }

    public Result<Pet, Error> GetPetById(PetId id)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == id);
        if (pet is null)
            return Errors.General.NotFound(nameof(pet));

        return pet;
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        var serialNumberResult = Position.Create(_pets.Count + 1);
        if (serialNumberResult.IsFailure)
            return serialNumberResult.Error;

        pet.SetSerialNumber(serialNumberResult.Value);

        _pets.Add(pet);
        return Result.Success<Error>();
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        var currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
            return Result.Success<Error>();

        var adjustedPosition = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPosition.IsFailure)
            return adjustedPosition.Error;

        newPosition = adjustedPosition.Value;

        var moveResult = MovePetsBetweenPositions(newPosition, currentPosition);
        if (moveResult.IsFailure)
            return moveResult.Error;
        
        pet.Move(newPosition);
        
        return Result.Success<Error>();
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
            var petsToMove = _pets.Where(p => p.Position >= newPosition
                                              && p.Position < currentPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();
                if (result.IsFailure)
                    return result.Error;
            }
        }

        else if (newPosition > currentPosition)
        {
            var petsToMove = _pets.Where(p => p.Position <= newPosition
                                              && p.Position > currentPosition);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveBack();
                if (result.IsFailure)
                    return result.Error;
            }
        }
        
        return Result.Success<Error>();
    }
}