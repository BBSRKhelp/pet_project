using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Interfaces.Repositories;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteersRepository : IVolunteersRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VolunteersRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Volunteers.AddAsync(volunteer, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return volunteer.Id;
    }

    public async Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId volunteerId, CancellationToken cancellationToken = default)
    {
        var volunteer = await
            _dbContext
            .Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == volunteerId, cancellationToken);
        
        if (volunteer is null)
            return Errors.General.NotFound(nameof(volunteerId));

        return volunteer;
    }

    public async Task<Result<Volunteer, Error>> GetByPhoneAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        var volunteer = await 
            _dbContext
            .Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.PhoneNumber == phoneNumber, cancellationToken);
        
        if (volunteer is null)
            return Errors.General.NotFound(nameof(phoneNumber));
    
        return volunteer;
    }

    public async Task<Result<Volunteer, Error>> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var volunteer = await 
            _dbContext
            .Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Email == email, cancellationToken);
        
        if (volunteer is null)
            return Errors.General.NotFound(nameof(email));
    
        return volunteer;
    }
}