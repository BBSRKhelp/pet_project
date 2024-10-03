using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record Fullname
{
    private Fullname(string firstName, string lastName, string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string? Patronymic { get; }

    public static Result<Fullname, Error> Create(
        string firstName,
        string lastName,
        string? patronymic = null)
    {
        if (IsNullOrWhiteSpace(firstName))
            return Errors.General.IsRequired(nameof(firstName));

        if (IsNullOrWhiteSpace(lastName))
            return Errors.General.IsRequired(nameof(lastName));
        
        return new Fullname(firstName, lastName, patronymic);
    }
}