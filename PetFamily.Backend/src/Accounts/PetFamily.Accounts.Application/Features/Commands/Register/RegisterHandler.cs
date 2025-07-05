using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces.Managers;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Domain.ValueObjects;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;

namespace PetFamily.Accounts.Application.Features.Commands.Register;

public class RegisterHandler : ICommandHandler<RegisterCommand>
{
    private const string BUCKET_NAME = "avatars";
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IParticipantAccountManager _participantAccountManager;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IParticipantAccountManager participantAccountManager,
        IValidator<RegisterCommand> validator,
        [FromKeyedServices(UnitOfWorkContext.Accounts)]
        IUnitOfWork unitOfWork,
        ILogger<RegisterHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _participantAccountManager = participantAccountManager;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Register user account");

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Validation failed for user registration");
            return validationResult.ToErrorList();
        }

        var fullName = FullName.Create(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.Patronymic).Value;

        var socialNetworks = command.SocialNetworks
            ?.Select(x => SocialNetwork.Create(x.Title, x.Url).Value)
            .ToArray() ?? [];

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var role = await _roleManager.FindByNameAsync(ParticipantAccount.PARTICIPANT);
            if (role is null)
            {
                _logger.LogError("Role not found");
                transaction.Rollback();
                return Errors.General.NotFound(nameof(role)).ToErrorList();
            }

            var participantUserResult = User.CreateParticipant(
                command.UserName,
                fullName,
                command.Email,
                null,
                socialNetworks,
                role);
            if (participantUserResult.IsFailure)
            {
                _logger.LogError("Create participant failed");
                transaction.Rollback();
                return Errors.General.IsInvalid("user").ToErrorList();
            }

            var createResult = await _userManager.CreateAsync(participantUserResult.Value, command.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogInformation("User: {UserName} could not be created", command.UserName);
                transaction.Rollback();

                var errors = createResult.Errors.Select(e => Error.Failure(e.Code, e.Description));
                return new ErrorList(errors);
            }

            var participantAccount = new ParticipantAccount(participantUserResult.Value);
            await _participantAccountManager.CreateParticipantAccountAsync(participantAccount, cancellationToken);

            transaction.Commit();
            _logger.LogInformation("User {UserName} registered successfully", command.UserName);

            return UnitResult.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user {UserName}", command.UserName);
            transaction.Rollback();
            throw;
        }
    }
}