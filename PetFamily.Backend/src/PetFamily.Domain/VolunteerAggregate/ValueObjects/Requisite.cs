using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record Requisite
{
    private Requisite(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; } = null!;
    public string Description { get; } = null!;

    public static Result<Requisite, Error> Create(string title, string description)
    {
        if (IsNullOrWhiteSpace(title))
            return Errors.General.IsRequired(nameof(title));
        
        if (title.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(title));
        
        if (IsNullOrWhiteSpace(description))
            return Errors.General.IsRequired(nameof(description));

        if (title.Length > Constants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(title));
        
        return new Requisite(title, description);
    }
}