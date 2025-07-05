using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using static System.String;

namespace PetFamily.Accounts.Domain.ValueObjects;

public record FullName
{
    private FullName(string firstName, string lastName, string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string? Patronymic { get; }

    public static Result<FullName, Error> Create(
        string firstName,
        string lastName,
        string? patronymic = null)
    {
        if (IsNullOrWhiteSpace(firstName))
            return Errors.General.IsRequired(nameof(firstName));
        
        if (firstName.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(firstName));

        if (IsNullOrWhiteSpace(lastName))
            return Errors.General.IsRequired(nameof(lastName));
        
        if (lastName.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(lastName));
        
        if (patronymic?.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(patronymic));

        return new FullName(firstName, lastName, patronymic);
    }
}