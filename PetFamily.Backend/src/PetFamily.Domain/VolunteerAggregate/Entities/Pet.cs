using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Shell;

namespace PetFamily.Domain.VolunteerAggregate.Entities;

public class Pet : Shared.Models.Entity<PetId>
{
    private bool _isDeleted = false;

    //ef core
    [JsonConstructor]
    private Pet() : base(PetId.NewId())
    {
    }

    public Pet(
        Name? name,
        Description? description,
        AppearanceDetails appearanceDetails,
        HealthDetails healthDetails,
        Address address,
        PhoneNumber phoneNumber,
        DateOnly? birthday,
        StatusForHelp status,
        PetPhotosShell petPhotos,
        RequisitesShell requisites,
        BreedAndSpeciesId breedAndSpeciesId)
        : base(PetId.NewId())
    {
        Name = name;
        Description = description;
        AppearanceDetails = appearanceDetails;
        HealthDetails = healthDetails;
        Address = address;
        PhoneNumber = phoneNumber;
        Birthday = birthday;
        Status = status;
        PetPhotos = petPhotos;
        Requisites = requisites;
        BreedAndSpeciesId = breedAndSpeciesId;
    }

    public Name? Name { get; private set; }
    public Description? Description { get; private set; }
    public AppearanceDetails AppearanceDetails { get; private set; } = null!;
    public HealthDetails HealthDetails { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public DateOnly? Birthday { get; private set; }
    public StatusForHelp Status { get; private set; }
    public PetPhotosShell PetPhotos { get; private set; } = null!;
    public RequisitesShell Requisites { get; private set; } = null!;
    public BreedAndSpeciesId BreedAndSpeciesId { get; private set; } = null!;
    public Position Position { get; private set; } = null!;
    public static DateTime CreatedAt => DateTime.Now;

    public void UpdatePhotos(PetPhotosShell photos) =>
        PetPhotos = photos;
    
    public void SetSerialNumber(Position position) =>
        Position = position;
    
    public void Move(Position newPosition) =>
        Position = newPosition;

    public UnitResult<Error> MoveForward()
    {
        var newPosition = Position.Forward();
        if (newPosition.IsFailure)
            return newPosition.Error;
        
        Position = newPosition.Value;
        
        return Result.Success<Error>();
    }
    
    public UnitResult<Error> MoveBack()
    {
        var newPosition = Position.Back();
        if (newPosition.IsFailure)
            return newPosition.Error;
        
        Position = newPosition.Value;
        
        return Result.Success<Error>();
    }
    
    public void IsDeactivate() => _isDeleted = true;
    public void IsActivate() => _isDeleted = false;
}