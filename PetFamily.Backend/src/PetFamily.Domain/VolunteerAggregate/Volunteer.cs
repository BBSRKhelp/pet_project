using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.Entities;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate;

public class Volunteer : Shared.Models.Entity<VolunteerId>
{
    private const byte MAX_NUMBER = 100;
    private readonly List<Pet> _pets = [];
    
    //ef core
    private Volunteer() : base(VolunteerId.NewId())
    {
    }

    private Volunteer(
        Fullname fullname, 
        Email email, 
        string? description,
        byte workExperience, 
        PhoneNumber phoneNumber,
        VolunteerDetails volunteerDetails,
        IEnumerable<Pet>? pets)
        : base(VolunteerId.NewId())
    {
        Fullname = fullname;
        Email = email;
        Description = description;
        WorkExperience = workExperience;
        PhoneNumber = phoneNumber;
        Details = volunteerDetails;
        if (pets != null) _pets.AddRange(pets);
    }

    public Fullname Fullname { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string? Description { get; private set; }
    public byte WorkExperience { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public VolunteerDetails? Details { get; private set; }
    public IReadOnlyList<Pet>? Pets => _pets.AsReadOnly();

    public int PetsFoundHome() => Pets.Count(p => p.Status == StatusForHelp.FoundHome);
    public int PetsLookingForHome() => Pets.Count(p => p.Status == StatusForHelp.LookingForHome);
    public int PetsNeedHelp() => Pets.Count(p => p.Status == StatusForHelp.NeedsHelp);

    public static Result<Volunteer, Error> Create(
        Fullname fullname, 
        Email email, 
        string? description, 
        byte workExperience,
        PhoneNumber phoneNumber,
        VolunteerDetails volunteerDetails,
        IEnumerable<Pet>? pets)
    {
        if (description?.Length > Constants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(description));
        
        if (workExperience > MAX_NUMBER)
            return Errors.General.MaxLengthExceeded(nameof(workExperience));
        
        return new Volunteer(
            fullname, 
            email, 
            description,
            workExperience,
            phoneNumber, 
            volunteerDetails,
            pets);
    }
}