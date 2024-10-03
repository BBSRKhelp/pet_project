using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record SocialNetwork
{
    [JsonConstructor]
    private SocialNetwork()
    {
    }
    private SocialNetwork(string title, string url)
    {
        Title = title;
        Url = url;
    }
    
    public string Title { get; } = null!;
    public string Url { get; } = null!;

    public static Result<SocialNetwork, Error> Create(string title, string url)
    {
        if (IsNullOrWhiteSpace(title))
            return Errors.General.IsRequired(nameof(title));
        
        if (IsNullOrWhiteSpace(url))
            return Errors.General.IsRequired(nameof(url));
        
        return new SocialNetwork(title, url);
    }
}