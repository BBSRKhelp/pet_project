using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using static System.String;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public record PetPhoto
{
    //ef core
    private PetPhoto()
    {
    }
    
    private PetPhoto(string path, bool isMainPhoto)
    {
        Path = path;
        IsMainPhoto = isMainPhoto;
    }
    
    public string Path { get; } = null!;
    public bool? IsMainPhoto { get; }

    public static Result<PetPhoto, Error> Create(string path, bool isMainPhoto)
    {
        if(IsNullOrEmpty(path))
            return Errors.General.IsRequired(nameof(path));
        
        return new PetPhoto(path, isMainPhoto);
    }
}