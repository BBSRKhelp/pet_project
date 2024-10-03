using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts.Shared;
using PetFamily.API.Extensions;
using PetFamily.Application.Requests.Volunteer.Create;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] VolunteerCreateHandler handler,
        [FromBody] VolunteerCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(request, cancellationToken);

        return result.ToResponse();
    }
}