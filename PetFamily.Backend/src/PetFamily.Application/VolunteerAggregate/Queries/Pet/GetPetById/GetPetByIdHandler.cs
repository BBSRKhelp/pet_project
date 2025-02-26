using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Application.DTOs.Read;
using PetFamily.Application.Extensions;
using PetFamily.Application.Interfaces.Abstractions;
using PetFamily.Application.Interfaces.Database;
using PetFamily.Domain.Shared.Models;

namespace PetFamily.Application.VolunteerAggregate.Queries.Pet.GetPetById;

public class GetPetByIdHandler : IQueryHandler<PetDto, GetPetByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetPetByIdQuery> _validator;
    private readonly ILogger<GetPetByIdHandler> _logger;

    public GetPetByIdHandler(
        IReadDbContext readDbContext,
        IValidator<GetPetByIdQuery> validator,
        ILogger<GetPetByIdHandler> logger)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetDto, ErrorList>> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting pet by id");

        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Failed to get pet by id");
            return validationResult.ToErrorList();
        }

        var pet = await _readDbContext
            .Pets
            .Where(p => p.IsDeleted == false)
            .FirstOrDefaultAsync(p => p.Id == query.PetId, cancellationToken);

        if (pet is null)
        {
            _logger.LogInformation("Pet with id = '{PetId}' does not exist", query.PetId);
            return (ErrorList)Errors.General.NotFound("Pet");
        }

        return pet;
    }
}