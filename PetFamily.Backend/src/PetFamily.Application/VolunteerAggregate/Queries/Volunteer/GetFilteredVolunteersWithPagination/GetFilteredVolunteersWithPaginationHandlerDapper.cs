using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.DTOs;
using PetFamily.Application.DTOs.Read;
using PetFamily.Application.DTOs.Volunteer;
using PetFamily.Application.Extensions;
using PetFamily.Application.Interfaces.Abstractions;
using PetFamily.Application.Interfaces.Database;
using PetFamily.Application.Models;
using PetFamily.Domain.Shared.Models;

namespace PetFamily.Application.VolunteerAggregate.Queries.Volunteer.GetFilteredVolunteersWithPagination;

public class GetFilteredVolunteersWithPaginationHandlerDapper :
    IQueryHandler<PagedList<VolunteerDto>, GetFilteredVolunteersWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IValidator<GetFilteredVolunteersWithPaginationQuery> _validator;
    private readonly ILogger<GetFilteredVolunteersWithPaginationHandlerDapper> _logger;

    public GetFilteredVolunteersWithPaginationHandlerDapper(
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<GetFilteredVolunteersWithPaginationQuery> validator,
        ILogger<GetFilteredVolunteersWithPaginationHandlerDapper> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PagedList<VolunteerDto>, ErrorList>> HandleAsync(
        GetFilteredVolunteersWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting volunteers with pagination");

        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Failed to get volunteers with pagination");
            return validationResult.ToErrorList();
        }

        using var connection = _sqlConnectionFactory.GetConnection();

        _logger.LogInformation("Connection with database established");

        var builder = new SqlBuilder();

        var countTemplate = builder.AddTemplate("SELECT COUNT(*) FROM volunteers /**where**/");

        var volunteersTemplate = builder.AddTemplate("""
                                                     SELECT id, 
                                                     first_name, 
                                                     last_name, 
                                                     patronymic, 
                                                     description, 
                                                     work_experience,
                                                     phone_number,
                                                     email,
                                                     requisites, 
                                                     social_networks
                                                     FROM volunteers
                                                     /**where**/ 
                                                     /**orderby**/
                                                     LIMIT @PageSize OFFSET @OffSet;
                                                     """);

        builder.Where("is_deleted = 'false'");

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            builder.Where("first_name ILIKE @FirstName");

        if (!string.IsNullOrWhiteSpace(query.LastName))
            builder.Where("last_name ILIKE @LastName");

        if (!string.IsNullOrWhiteSpace(query.Patronymic))
            builder.Where("patronymic ILIKE @Patronymic");

        if (query.WorkExperience.HasValue)
            builder.Where("work_experience = @WorkExperience");

        builder.OrderBy($"{query.SortBy} {query.SortDirection}");

        var param = new
        {
            PageSize = query.PageSize,
            OffSet = (query.PageNumber - 1) * query.PageSize,
            FirstName = '%' + query.FirstName + '%',
            LastName = '%' + query.LastName + '%',
            Patronymic = '%' + query.Patronymic + '%',
            WorkExperience = query.WorkExperience,
            SortDirection = query.SortDirection,
            SortBy = query.SortBy
        };

        var totalCount = await connection.ExecuteScalarAsync<long>(countTemplate.RawSql, param);

        var volunteers = await connection.QueryAsync<VolunteerDto, string, string, VolunteerDto>(
            volunteersTemplate.RawSql,
            (volunteer, jsonRequisites, jsonSocialNetwork) =>
            {
                volunteer.Requisites = JsonSerializer.Deserialize<RequisiteDto[]>(jsonRequisites)!;
                volunteer.SocialNetworks = JsonSerializer.Deserialize<SocialNetworkDto[]>(jsonSocialNetwork)!;

                return volunteer;
            },
            splitOn: "requisites,social_networks",
            param: param);

        _logger.LogInformation("Volunteers have been received");

        return new PagedList<VolunteerDto>
        {
            Items = volunteers.ToList(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}