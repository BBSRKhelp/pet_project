using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Interfaces.Abstractions;
using PetFamily.Application.VolunteerAggregate.Commands.Pet.ChangePetsPosition;

namespace PetFamily.Application.IntegrationTests.Pet.ChangePetsPositionTests;

public class ChangePetsPositionTests : PetTestsBase
{
    private readonly ICommandHandler<ChangePetsPositionCommand> _sut;

    public ChangePetsPositionTests(PetTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<ChangePetsPositionCommand>>();
    }

    [Fact]
    public async Task ChangePetsPosition_ShouldNotReturnAnError()
    {
        //Arrange
        var volunteer = await SeedVolunteerAsync();
        var species = await SeedSpeciesAsync();
        var breedId = await SeedBreedAsync(species);

        List<Domain.VolunteerAggregate.Entities.Pet> pets = [];
        foreach (var _ in Enumerable.Range(0, 5))
        {
            var pet = await SeedPetAsync(volunteer, species.Id, breedId);
            pets.Add(pet);
        }

        var petWithInitialPosition = pets.FirstOrDefault(p => p.Position.Value == 2);
        const int NEW_POSITION = 4;

        var command = Fixture
            .BuildChangePetsPositionCommand(volunteer.Id.Value, petWithInitialPosition!.Id.Value, NEW_POSITION);

        //Act
        var result = await _sut.HandleAsync(command);

        //Assert
        result.IsSuccess.Should().BeTrue();

        var petFromDb = WriteDbContext
            .Volunteers.FirstOrDefault()
            ?.Pets.FirstOrDefault(p => p.Position.Value == 4);

        petFromDb?.Id.Should().Be(petWithInitialPosition.Id);
    }
}