using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts.Breed;
using PetFamily.API.Extensions;
using PetFamily.Application.DTOs.Read;
using PetFamily.Application.Models;
using PetFamily.Application.SpeciesAggregate.Queries.GetBreedsByIdSpecies;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BreedsController : ControllerBase
{
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