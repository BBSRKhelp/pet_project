using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Models;
using PetFamily.Species.Application.Features.Queries.GetBreedsByIdSpecies;
using PetFamily.Species.Contracts.DTOs;
using Xunit.Abstractions;

namespace PetFamily.Breed.Application.IntegrationTests.Tests;

public class GetBreedsByIdSpeciesTests : BreedTestsBase
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IQueryHandler<PagedList<BreedDto>, GetBreedsByIdSpeciesQuery> _sut;

    public GetBreedsByIdSpeciesTests(BreedTestsWebFactory factory, ITestOutputHelper testOutputHelper) : base(factory)
    {
        _testOutputHelper = testOutputHelper;
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<BreedDto>, GetBreedsByIdSpeciesQuery>>();
    }

    [Fact]
    public async Task GetBreedsByIdSpecies_ShouldReturnPagedListOfBreeds()
    {
        //Arrange
        var species = await SeedSpeciesAsync();
        foreach (var i in Enumerable.Range(0, 10))
            await SeedBreedAsync(species, $"breedName{i}");

        var query = Fixture.BuildGetBreedsByIdSpeciesQuery(species.Id.Value, pageNumber: 1, pageSize: 5);

        //Act
        var result = await _sut.HandleAsync(query);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(5);
        result.Value.TotalCount.Should().Be(10);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeTrue();
        
        var breeds = WriteDbContext
            .Species.FirstOrDefault()?
            .Breeds.ToList();
        
        breeds.Should().NotBeNull();
        breeds.Count.Should().Be(10);
    }
    
    [Fact]
    public async Task GetBreedsByIdSpecies_WhenPaginationIsInvalid_ShouldReturnFailedResults()
    {
        //Arrange
        var species = await SeedSpeciesAsync();
        foreach (var _ in Enumerable.Range(0, 10))
            await SeedBreedAsync(species, "breedName");

        var query = Fixture.BuildGetBreedsByIdSpeciesQuery(species.Id.Value, pageNumber: -1, pageSize: -5);

        //Act
        var result = await _sut.HandleAsync(query);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}