using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;

namespace PetFamily.Domain.Shared.ValueObjects;

public record PhoneNumber
{
    public const int MAX_LENGTH = 11;
    
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; } = null!;

    public static Result<PhoneNumber, Error> Create(string phone)
    {
        if (phone.Length != MAX_LENGTH || !Regex.IsMatch(phone, @"^[0-9]+$"))
            return Errors.General.IsInvalid(nameof(PhoneNumber));

        return new PhoneNumber(phone);
    }
}