using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregate;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Domain.VolunteerAggregate.ValueObjects.Ids;

namespace PetFamily.Application.Interfaces.Repositories;

public interface IVolunteersRepository
{
    Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default);
    Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId volunteerId, CancellationToken cancellationToken = default);
    Task<Result<Volunteer, Error>> GetByPhoneAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<Result<Volunteer, Error>> GetByEmailAsync(Email requestEmail, CancellationToken cancellationToken);
}