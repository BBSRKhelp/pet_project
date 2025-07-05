using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Domain.ValueObjects;
using PetFamily.Accounts.Infrastructure.Authorization.Managers;
using PetFamily.Accounts.Infrastructure.Options;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel;

namespace PetFamily.Accounts.Infrastructure.Authorization.Seeding;

public class AccountsSeederService(
    UserManager<User> userManager,
    RoleManager roleManager,
    PermissionManager permissionManager,
    RolePermissionManager rolePermissionManager,
    AdminAccountManager adminAccountManager,
    IOptions<AdminOptions> adminOptions,
    [FromKeyedServices(UnitOfWorkContext.Accounts)]
    IUnitOfWork unitOfWork,
    ILogger<AccountsSeederService> logger)
{
    private readonly AdminOptions _adminOptions = adminOptions.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding accounts...");

        var json = await File.ReadAllTextAsync(FilePaths.ACCOUNTS, cancellationToken);

        var seedData = JsonSerializer.Deserialize<RolePermissionJsonModel>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission configuration.");

        await SeedPermissionsAsync(seedData, cancellationToken);
        await SeedRolesAsync(seedData, cancellationToken);
        await SeedRolePermissionsAsync(seedData, cancellationToken);
        await SeedAdminAccountAsync(cancellationToken);

        logger.LogInformation("Everything seeded");
    }

    private async Task SeedPermissionsAsync(
        RolePermissionJsonModel seedData,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding Permissions");

        var permissions = seedData
            .Permissions
            .SelectMany(permissionGroup => permissionGroup.Value)
            .ToList();

        await permissionManager.SyncPermissionsAsync(permissions, cancellationToken);
    }

    private async Task SeedRolesAsync(
        RolePermissionJsonModel seedData,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding Roles");

        var roles = seedData.Roles
            .Select(r => r.Key)
            .ToList();

        await roleManager.SyncRolesAsync(roles, cancellationToken);
    }

    private async Task SeedRolePermissionsAsync(
        RolePermissionJsonModel seedData,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding Role Permissions");

        var rolesWithPermissions = seedData.Roles;

        await rolePermissionManager.SyncRolesAndPermissionsAsync(rolesWithPermissions, cancellationToken);
    }

    private async Task SeedAdminAccountAsync(CancellationToken cancellationToken = default)
    {
        const string OPERATION_NAME = "Admin account seeding";
        logger.LogInformation("{Operation} started for email: {Email}", OPERATION_NAME, _adminOptions.Email);

        using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var existsUser = await userManager.FindByEmailAsync(_adminOptions.Email);
            if (existsUser is not null)
            {
                logger.LogInformation("{Operation} aborted - admin account already exists (Email: {Email})",
                    OPERATION_NAME, _adminOptions.Email);
                transaction.Commit();
                return;
            }

            var adminRole = await roleManager.FindByNameAsync(AdminAccount.ADMIN);
            if (adminRole is null)
            {
                logger.LogError("{Operation} failed - admin role '{RoleName}' not found in database",
                    OPERATION_NAME, AdminAccount.ADMIN);
                transaction.Rollback();
                throw new InvalidOperationException(
                    $"Required admin role '{AdminAccount.ADMIN}' was not found in database");
            }

            var fullNameResult = FullName.Create(
                _adminOptions.FirstName,
                _adminOptions.LastName,
                _adminOptions.Patronymic);

            if (fullNameResult.IsFailure)
            {
                logger.LogError(
                    "{Operation} failed - invalid full name format " +
                    "(FirstName: {FirstName}, LastName: {LastName}, Patronymic: {Patronymic}). Error: {Error}",
                    OPERATION_NAME, _adminOptions.FirstName, _adminOptions.LastName, _adminOptions.Patronymic,
                    fullNameResult.Error);
                transaction.Rollback();
                throw new ArgumentException($"Invalid admin name format: {fullNameResult.Error}");
            }

            var adminUserResult = User.CreateAdmin(
                _adminOptions.UserName,
                fullNameResult.Value,
                _adminOptions.Email,
                null,
                [],
                adminRole);

            if (adminUserResult.IsFailure)
            {
                logger.LogError("{Operation} failed - cannot create user object. Error: {Error}",
                    OPERATION_NAME, adminUserResult.Error);
                transaction.Rollback();
                throw new InvalidOperationException($"Failed to create admin user object: {adminUserResult.Error}");
            }

            var createResult = await userManager.CreateAsync(adminUserResult.Value, _adminOptions.Password);
            if (!createResult.Succeeded)
            {
                logger.LogError("{Operation} failed - user creation failed", OPERATION_NAME);
                transaction.Rollback();
                throw new ApplicationException("Failed to create admin user");
            }

            var adminAccount = new AdminAccount(adminUserResult.Value);
            await adminAccountManager.CreateAdminAccountAsync(adminAccount, cancellationToken);

            transaction.Commit();
            logger.LogInformation("{Operation} completed successfully for email: {Email}",
                OPERATION_NAME, _adminOptions.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Operation} failed unexpectedly", OPERATION_NAME);
            transaction.Rollback();
            throw;
        }
    }
}