using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Contracts;

namespace PetFamily.Accounts.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsPresentation(this IServiceCollection services)
    {
        return services.AddScoped<IAccountsContract, AccountsContract>();
    }
}