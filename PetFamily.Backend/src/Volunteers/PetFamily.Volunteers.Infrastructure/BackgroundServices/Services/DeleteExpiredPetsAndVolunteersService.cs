using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Core.Extensions;
using PetFamily.Volunteers.Domain;
using PetFamily.Volunteers.Infrastructure.Database;
using PetFamily.Volunteers.Infrastructure.Options;

namespace PetFamily.Volunteers.Infrastructure.BackgroundServices.Services;

public class DeleteExpiredPetsAndVolunteersService(
    WriteDbContext writeDbContext,
    IOptions<SoftDeleteOptions> options,
    ILogger<DeleteExpiredPetsAndVolunteersService> logger)
{
    private readonly SoftDeleteOptions _options = options.Value;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var volunteers = await GetVolunteersWithPetsAsync(cancellationToken);

        logger.LogInformation("Finding and deleting entities with expired lifetime after soft deletion");

        foreach (var volunteer in volunteers)
            volunteer.DeleteExpiredPets(_options.SoftDeleteRetentionDays);

        var expiredVolunteers = volunteers
            .Where(v => v.ShouldBeHardDeleted(_options.SoftDeleteRetentionDays))
            .ToList();

        if (expiredVolunteers.Count != 0)
        {
            writeDbContext.Volunteers.RemoveRange(expiredVolunteers);
            logger.LogInformation("Removing {ExpiredVolunteersCount} expired volunteers", expiredVolunteers.Count);
        }

        await writeDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Volunteer>> GetVolunteersWithPetsAsync(CancellationToken cancellationToken = default)
    {
        writeDbContext.ChangeTracker.Clear();
        return await writeDbContext.Volunteers.ToListAsync(cancellationToken);
    }
}