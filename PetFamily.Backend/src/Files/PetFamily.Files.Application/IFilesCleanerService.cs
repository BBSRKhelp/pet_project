namespace PetFamily.Files.Application;

public interface IFilesCleanerService
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}