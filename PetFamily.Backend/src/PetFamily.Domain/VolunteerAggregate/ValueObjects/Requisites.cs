using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record Requisite
{
    [JsonConstructor]
    private Requisite(){}
    
    private Requisite(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; }
    public string Description { get; }

    public static Result<Requisite, Error> Create(string title, string description)
    {
        if (IsNullOrWhiteSpace(title))
            return Errors.General.IsRequired(nameof(title));
        
        if (IsNullOrWhiteSpace(description))
            return Errors.General.IsRequired(nameof(description));
        
        return new Requisite(title, description);
    }
}