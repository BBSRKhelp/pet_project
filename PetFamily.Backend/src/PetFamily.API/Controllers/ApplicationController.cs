using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Contracts.Shared;
using PetFamily.API.Extensions;
using PetFamily.Application.Requests.Volunteer.Create;

namespace PetFamily.API.Controllers;


[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase
{
   public override OkObjectResult Ok(object? value)
   {
      var envelope = Envelope.Ok(value);
      
      return new OkObjectResult(envelope);
   }
}