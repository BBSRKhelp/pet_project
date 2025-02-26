using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record Address
{
    private Address(
        string country,
        string city,
        string street,
        string? postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }

    public string Country { get; }
    public string City { get; }
    public string Street { get; }
    public string? PostalCode { get; }

    public static Result<Address, Error> Create(
        string country,
        string city,
        string street,
        string? postalcode)
    {
        if (IsNullOrWhiteSpace(country))
            return Errors.General.IsRequired(nameof(country));

        if (IsNullOrWhiteSpace(city))
            return Errors.General.IsRequired(nameof(city));

        if (IsNullOrWhiteSpace(street))
            return Errors.General.IsRequired(nameof(street));

        return new Address(country,
            city,
            street,
            postalcode);
    }
}