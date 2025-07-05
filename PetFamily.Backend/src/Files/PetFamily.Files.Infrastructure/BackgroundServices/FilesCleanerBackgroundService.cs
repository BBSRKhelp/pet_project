using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Files.Application;

namespace PetFamily.Files.Infrastructure.BackgroundServices;

public class FilesCleanerBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<FilesCleanerBackgroundService> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("FilesCleanerBackgroundService is starting");
        await using var scope = scopeFactory.CreateAsyncScope();

        var filesCleanerService = scope.ServiceProvider.GetRequiredService<IFilesCleanerService>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await filesCleanerService.ProcessAsync(cancellationToken);
        }

        await Task.CompletedTask;
        logger.LogInformation("FilesCleanerBackgroundService is stopping");
    }
}