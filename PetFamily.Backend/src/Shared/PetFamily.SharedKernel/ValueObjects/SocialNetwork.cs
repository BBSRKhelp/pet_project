using CSharpFunctionalExtensions;
using static System.String;

namespace PetFamily.SharedKernel.ValueObjects;

public record SocialNetwork
{
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

        if (title.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(title));

        if (IsNullOrWhiteSpace(url))
            return Errors.General.IsRequired(nameof(url));

        if (url.Length > Constants.MAX_MEDIUM_LOW_TEXT_LENGTH)
            return Errors.General.MaxLengthExceeded(nameof(url));

        return new SocialNetwork(title, url);
    }
}