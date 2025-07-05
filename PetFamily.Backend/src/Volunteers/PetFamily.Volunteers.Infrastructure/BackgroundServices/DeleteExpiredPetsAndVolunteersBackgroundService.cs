using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Volunteers.Infrastructure.BackgroundServices.Services;

namespace PetFamily.Volunteers.Infrastructure.BackgroundServices;

public class DeleteExpiredPetsAndVolunteersBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<DeleteExpiredPetsAndVolunteersBackgroundService> logger)
    : BackgroundService
{
    private const int DELETE_PETS_AND_VOLUNTEERS_DELAY_HOURS = 24;
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteExpiredPetsAndVolunteersBackgroundService is starting");
        await using var scope = scopeFactory.CreateAsyncScope();

        var deleteExpiredPetsAndVolunteersService = scope.ServiceProvider
            .GetRequiredService<DeleteExpiredPetsAndVolunteersService>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await deleteExpiredPetsAndVolunteersService.ProcessAsync(cancellationToken);
            logger.LogInformation("DeleteExpiredEntitiesService is waiting for the next {Hours} hours", 
                DELETE_PETS_AND_VOLUNTEERS_DELAY_HOURS);
            
             await Task.Delay(TimeSpan.FromHours(DELETE_PETS_AND_VOLUNTEERS_DELAY_HOURS),
                 cancellationToken);
        }
    }
}