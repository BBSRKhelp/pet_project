using PetFamily.Accounts.Contracts.DTOs;
using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Features.Commands.Register;

public record RegisterCommand(
    string UserName,
    FullNameDto FullName,
    string Email, 
    IEnumerable<SocialNetworkDto>? SocialNetworks,
    string Password) : ICommand;