using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Contracts;
using PetFamily.Core.Models;

namespace PetFamily.Framework.Authorization;

public class PermissionRequirementHandler(IServiceScopeFactory scopeFactory) : AuthorizationHandler<PermissionAttribute>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        using var scope = scopeFactory.CreateScope();

        var accountsContract = scope.ServiceProvider.GetRequiredService<IAccountsContract>();

        var userIdClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == CustomClaims.NAME_IDENTIFIER)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        var permissionCodes = await accountsContract.GetUserPermissionCodesAsync(userId);

        if (permissionCodes.Contains(permission.Code))
        {
            context.Succeed(permission);
            return;
        }

        context.Fail();
    }
}