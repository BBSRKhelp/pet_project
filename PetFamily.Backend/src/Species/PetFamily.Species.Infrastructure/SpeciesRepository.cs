using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Species.Application.Interfaces;
using PetFamily.Species.Infrastructure.Database;

namespace PetFamily.Species.Infrastructure;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ILogger<SpeciesRepository> _logger;

    public SpeciesRepository(WriteDbContext writeDbContext, ILogger<SpeciesRepository> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Domain.Species species, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Species.AddAsync(species, cancellationToken);

        return species.Id.Value;
    }

    public Guid Delete(Domain.Species species)
    {
        _writeDbContext.Species.Remove(species);

        return species.Id.Value;
    }

    public async Task<Result<Domain.Species, Error>> GetByIdAsync(SpeciesId speciesId,
        CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext
            .Species
            .FirstOrDefaultAsync(s => s.Id == speciesId, cancellationToken);

        if (species is null)
        {
            _logger.LogInformation("A species with Id = {speciesId} was not found", speciesId.Value);

            return Errors.General.NotFound(nameof(species));
        }

        _logger.LogInformation("A species with Id = {speciesId} has been found", speciesId.Value);

        return species;
    }

    public async Task<Result<Domain.Species, Error>> GetByNameAsync(Name name,
        CancellationToken cancellationToken = default)
    {
        var species = await _writeDbContext
            .Species
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

        if (species is null)
        {
            _logger.LogInformation("A species with name = {name} was not found", name.Value);

            return Errors.General.NotFound(nameof(species));
        }

        _logger.LogInformation("A species with name = {name} has been found", name.Value);

        return species;
    }
}