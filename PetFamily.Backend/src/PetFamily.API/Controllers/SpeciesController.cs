using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts.Breed;
using PetFamily.API.Contracts.Species;
using PetFamily.API.Extensions;
using PetFamily.Application.Commands.Species.AddBreed;
using PetFamily.Application.Commands.Species.Create;

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
        [FromServices] CreateBreedHandler handler,
        [FromBody] CreateBreedRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand(id);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.ToResponse();
    }
}