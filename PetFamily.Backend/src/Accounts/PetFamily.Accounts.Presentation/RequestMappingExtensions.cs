using PetFamily.Accounts.Application.Features.Commands.Login;
using PetFamily.Accounts.Application.Features.Commands.RefreshToken;
using PetFamily.Accounts.Application.Features.Commands.Register;
using PetFamily.Accounts.Contracts.DTOs;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Presentation;

public static class RequestMappingExtensions
{
    public static RegisterCommand ToCommand(this RegisterUserRequest request)
    {
        var fullName = new FullNameDto(request.FirstName, request.LastName,  request.Patronymic);
        
        return new RegisterCommand(
            request.UserName, 
            fullName,
            request.Email, 
            request.SocialNetworks, 
            request.Password);
    }

    public static LoginCommand ToCommand(this LoginRequest request)
    {
        return new LoginCommand(request.Email, request.Password);
    }

    public static RefreshTokensCommand ToCommand(this RefreshTokensRequest request)
    {
        return new RefreshTokensCommand(request.AccessToken,  request.RefreshToken);
    }
}