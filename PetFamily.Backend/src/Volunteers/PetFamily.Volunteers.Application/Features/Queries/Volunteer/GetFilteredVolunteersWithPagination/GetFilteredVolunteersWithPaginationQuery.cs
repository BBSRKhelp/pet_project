using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Features.Queries.Volunteer.GetFilteredVolunteersWithPagination;

public record GetFilteredVolunteersWithPaginationQuery(
    int PageNumber,
    int PageSize,
    string? FirstName,
    string? LastName,
    string? Patronymic,
    byte? WorkExperience, 
    string SortBy,
    string SortDirection) : IQuery;
