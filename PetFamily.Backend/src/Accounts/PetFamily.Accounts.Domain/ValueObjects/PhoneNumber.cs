using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Accounts.Domain.ValueObjects;

public class PhoneNumber : ComparableValueObject
{
    public const int MAX_LENGTH = 11;

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PhoneNumber, Error> Create(string value)
    {
        var phone = value.Trim();
        
        if (phone.Length != MAX_LENGTH || !Regex.IsMatch(phone, @"^[0-9]+$"))
            return Errors.General.IsInvalid(nameof(phone));

        return new PhoneNumber(phone);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}