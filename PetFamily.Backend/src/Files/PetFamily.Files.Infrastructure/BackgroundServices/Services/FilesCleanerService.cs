using Microsoft.Extensions.Logging;
using PetFamily.Core.Messaging;
using PetFamily.Core.Providers;
using PetFamily.Files.Application;

namespace PetFamily.Files.Infrastructure.BackgroundServices.Services;

public class FilesCleanerService(
    IFileProvider fileProvider,
    IMessageQueue<IEnumerable<FileIdentifier>> messageQueue,
    ILogger<FilesCleanerService> logger)
    : IFilesCleanerService
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Search for files to clean up");
        var fileIdentifiers = await messageQueue.ReadAsync(cancellationToken);

        logger.LogInformation("Cleaning up files");
        foreach (var fileIdentifier in fileIdentifiers)
        {
            await fileProvider.RemoveFileAsync(fileIdentifier, cancellationToken);
        }

        logger.LogInformation("Files have been cleared");
    }
}