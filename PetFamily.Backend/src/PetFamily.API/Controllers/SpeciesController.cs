using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts.Breed;
using PetFamily.API.Contracts.Species;
using PetFamily.API.Extensions;
using PetFamily.Application.DTOs.Read;
using PetFamily.Application.Models;
using PetFamily.Application.SpeciesAggregate.Commands.AddBreed;
using PetFamily.Application.SpeciesAggregate.Commands.Create;
using PetFamily.Application.SpeciesAggregate.Commands.Delete;
using PetFamily.Application.SpeciesAggregate.Commands.DeleteBreed;
using PetFamily.Application.SpeciesAggregate.Queries.GetBreedsByIdSpecies;
using PetFamily.Application.SpeciesAggregate.Queries.GetFilteredSpeciesWithPagination;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SpeciesController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync(
       [FromServices] CreateSpeciesHandler handler,
       [FromBody] CreateSpeciesRequest request,
       CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.ToResponse();
    }

    [HttpPost("{id:guid}/breeds")]
    public async Task<ActionResult<Guid>> AddBreedAsync(
        [FromServices] AddBreedHandler handler,
        [FromBody] CreateBreedRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand(id);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.ToResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Boolean>> DeleteAsync(
        [FromServices] DeleteSpeciesHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSpeciesCommand(id);
        
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.ToResponse();
    }

    [HttpDelete("{speciesId:guid}/breeds/{breedId:guid}")]
    public async Task<ActionResult<Boolean>> DeleteBreedAsync(
        [FromServices] DeleteBreedHandler handler,
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);
        
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.ToResponse();
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<SpeciesDto>>> GetAsync(
        [FromServices] GetFilteredSpeciesWithPaginationHandler handler,
        [FromQuery] GetFilteredSpeciesWithPaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = request.ToQuery();
        
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.ToResponse();
    }

    [HttpGet("{speciesId:guid}/breeds")]
    public async Task<ActionResult<PagedList<BreedDto>>> GetBreedAsync(
        [FromServices] GetBreedsByIdSpeciesHandler speciesHandler,
        [FromQuery] GetBreedsByIdSpeciesRequest request,
        [FromRoute] Guid speciesId,
        CancellationToken cancellationToken = default)
    {
        var query = request.ToQuery(speciesId);
        
        var result = await speciesHandler.HandleAsync(query, cancellationToken);
        
        return result.ToResponse();
    }
}