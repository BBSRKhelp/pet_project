using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Application.Requests.Volunteer.Create;

namespace PetFamily.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<VolunteerCreateHandler>();
        
        return services;
    }
}