using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace PetFamily.Species.Application.Features.Queries.GetFilteredSpeciesWithPagination;

public class GetFilteredSpeciesWithPaginationQueryValidator : AbstractValidator<GetFilteredSpeciesWithPaginationQuery>
{
    private readonly IReadOnlyList<string> _allowedFilters = ["id", "name"];

    public GetFilteredSpeciesWithPaginationQueryValidator()
    {
        RuleFor(g => g.PageSize)
            .Must(i => i >= 1).WithError(Errors.General.MinLengthLowered("PageSize"));

        RuleFor(g => g.PageNumber)
            .Must(i => i >= 1).WithError(Errors.General.MinLengthLowered("PageNumber"));

        RuleFor(g => g.SortBy).Must(sortBy => _allowedFilters.Contains(sortBy))
            .WithError(Errors.General.IsInvalid("SortBy"));

        RuleFor(g => g.SortDirection)
            .Must(sd => sd.ToLower() is null or "asc" or "desc").WithError(Errors.General.IsInvalid("SortDirection"));

        RuleFor(g => g.Name)
            .MaximumLength(50).WithError(Errors.General.MaxLengthExceeded("Name"));
    }
}