using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.SpeciesAggregate;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SpeciesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default)
    {
        await _dbContext.Species.AddAsync(species, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return species.Id;
    }

    public async Task<Result<Species, Error>> GetByIdAsync(SpeciesId speciesId,
        CancellationToken cancellationToken = default)
    {
        var species = await 
            _dbContext.
            Species.
            Include(s => s.Breeds).
            FirstOrDefaultAsync(s => s.Id == speciesId, cancellationToken);
        
        if (species is null)
            return Errors.General.NotFound(speciesId);

        return species;

    }
}