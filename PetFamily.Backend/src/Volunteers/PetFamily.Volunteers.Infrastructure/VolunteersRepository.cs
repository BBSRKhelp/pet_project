using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel;
using PetFamily.Volunteers.Application.Interfaces;
using PetFamily.Volunteers.Domain.ValueObjects.Ids;
using PetFamily.Volunteers.Domain;
using PetFamily.Volunteers.Infrastructure.Database;

namespace PetFamily.Volunteers.Infrastructure;

public class VolunteersRepository : IVolunteersRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ILogger<VolunteersRepository> _logger;

    public VolunteersRepository(WriteDbContext writeDbContext, ILogger<VolunteersRepository> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Volunteers.AddAsync(volunteer, cancellationToken);

        return volunteer.Id.Value;
    }

    public Guid Delete(Volunteer volunteer)
    {
        _writeDbContext.Volunteers.Remove(volunteer);

        return volunteer.Id.Value;
    }

    public async Task<Result<Volunteer, Error>> GetByIdAsync(
        VolunteerId volunteerId,
        CancellationToken cancellationToken = default)
    {
        var volunteer = await _writeDbContext
            .Volunteers
            .FirstOrDefaultAsync(v => v.Id == volunteerId, cancellationToken);

        if (volunteer is null)
        {
            _logger.LogInformation("A volunteer with id = {volunteerId} was not found", volunteerId.Value);

            return Errors.General.NotFound(nameof(volunteerId));
        }

        _logger.LogInformation("A volunteer with id = {volunteerId} has been found", volunteerId.Value);

        return volunteer;
    }
}