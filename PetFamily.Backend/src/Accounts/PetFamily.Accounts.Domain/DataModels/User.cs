using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using PetFamily.Accounts.Domain.ValueObjects;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public sealed class User : IdentityUser<Guid>
{
    //ef core
    private User()
    {
    }
    
    private User(
        string userName,
        FullName fullName,
        string email,
        PhotoPath? photoPath,
        IReadOnlyList<SocialNetwork> socialNetworks,
        Role role)
    {
        UserName = userName;
        FullName = fullName;
        Email = email;
        PhotoPath = photoPath;
        SocialNetworks = socialNetworks;
        Roles = [role];
    }

    public FullName FullName { get; private set; } = null!;
    public PhotoPath? PhotoPath { get; private set; }
    public IReadOnlyList<SocialNetwork> SocialNetworks { get; private set; } = null!;
    public IReadOnlyList<Role> Roles { get; private set; } = null!;
    public ParticipantAccount? ParticipantAccount { get; init; }
    public VolunteerAccount? VolunteerAccount { get; init; }
    public AdminAccount? AdminAccount { get; init; }

    public static Result<User, Error> CreateParticipant(
        string userName,
        FullName fullName,
        string email,
        PhotoPath? photoPath,
        IReadOnlyList<SocialNetwork> socialNetworks,
        Role role)
    {
        if (role.NormalizedName is not ParticipantAccount.PARTICIPANT)
            return Errors.General.IsInvalid(nameof(role));

        return new User(
            userName,
            fullName,
            email,
            photoPath,
            socialNetworks,
            role);
    }

    public static Result<User, Error> CreateVolunteer(
        string userName,
        FullName fullName,
        string email,
        PhotoPath? photoPath,
        IReadOnlyList<SocialNetwork> socialNetworks,
        Role role)
    {
        if (role.NormalizedName is not VolunteerAccount.VOLUNTEER)
            return Errors.General.IsInvalid(nameof(role));

        return new User(
            userName,
            fullName,
            email,
            photoPath,
            socialNetworks,
            role);
    }

    public static Result<User, Error> CreateAdmin(
        string userName,
        FullName fullName,
        string email,
        PhotoPath? photoPath,
        IReadOnlyList<SocialNetwork> socialNetworks,
        Role role)
    {
        if (role.NormalizedName is not AdminAccount.ADMIN)
            return Errors.General.IsInvalid(nameof(role));

        return new User(
            userName,
            fullName,
            email,
            photoPath,
            socialNetworks,
            role);
    }

    public void ChangeRole(Role role)
        => Roles = [role];

    public void UpdateMainInfo(
        string userName,
        FullName fullName,
        PhotoPath? photoPath,
        IEnumerable<SocialNetwork> socialNetworks)
    {
        UserName = userName;
        FullName = fullName;
        PhotoPath = photoPath;
        SocialNetworks = socialNetworks.ToList();
    }
}