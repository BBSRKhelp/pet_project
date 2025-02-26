using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesAggregate.ValueObjects.Ids;
using PetFamily.Domain.VolunteerAggregate.Enums;
using PetFamily.Domain.VolunteerAggregate.ValueObjects;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Application.IntegrationTests.Pet;

public class PetTestsBase : IClassFixture<PetTestsWebFactory>, IAsyncLifetime
{
    protected readonly PetTestsWebFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly WriteDbContext WriteDbContext;

    protected PetTestsBase(PetTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        Fixture = new Fixture();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
    }

    protected async Task<Domain.VolunteerAggregate.Entities.Pet> SeedPetAsync(
        Domain.VolunteerAggregate.Volunteer volunteer,
        SpeciesId speciesId,
        Guid breedId)
    {
        var pet = new Domain.VolunteerAggregate.Entities.Pet(
            Name.Create("testname").Value,
            Description.Create("").Value,
            AppearanceDetails.Create(Colour.Black, 15, 15).Value,
            HealthDetails.Create("string.Empty", true, true).Value,
            Address.Create("string.Empty", "string.Empty", "string.Empty", "string.Empty").Value,
            PhoneNumber.Create("89166666666").Value,
            DateTime.Now,
            Status.FoundHome,
            [],
            BreedAndSpeciesId.Create(speciesId, breedId).Value);
        
        volunteer.AddPet(pet);
        await WriteDbContext.SaveChangesAsync();
        
        return pet;
    }
    
    protected async Task<Domain.VolunteerAggregate.Volunteer> SeedVolunteerAsync()
    {
        var volunteer = new Domain.VolunteerAggregate.Volunteer(
            FullName.Create("testname", "testlastname", "testpatronymic").Value,
            Email.Create("test@test.com").Value,
            Description.Create("").Value,
            WorkExperience.Create(46).Value,
            PhoneNumber.Create("89166666666").Value,
            [],
            []);
        
        await WriteDbContext.Volunteers.AddAsync(volunteer);
        await WriteDbContext.SaveChangesAsync();
        
        return volunteer;
    }

    protected async Task<Domain.SpeciesAggregate.Species> SeedSpeciesAsync()
    {
        var species = new Domain.SpeciesAggregate.Species(Name.Create("testname").Value);
        
        await WriteDbContext.Species.AddAsync(species);
        await WriteDbContext.SaveChangesAsync();
        
        return species;
    }

    protected async Task<Guid> SeedBreedAsync(Domain.SpeciesAggregate.Species species)
    {
        var breed = new Domain.SpeciesAggregate.Entities.Breed(Name.Create("testname").Value);
        
        species.AddBreed(breed);
        await WriteDbContext.SaveChangesAsync();
        
        return breed.Id.Value;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await Factory.ResetDatabaseAsync();
        Scope.Dispose();
    }
}