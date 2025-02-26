using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Models;

namespace PetFamily.Application.Extensions;

public static class QueryExtensions
{
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string sortBy, string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return source;

        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, sortBy);
        var keySelector = Expression.Lambda(property, parameter);

        var methodName = sortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, [source, keySelector])!;
    }

    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static SqlBuilder AddCondition(this SqlBuilder builder, string condition, string? value)
    {
        if (!string.IsNullOrWhiteSpace(condition))
            builder.Where(condition);

        return builder;
    }

    public static SqlBuilder AddCondition<T>(this SqlBuilder builder, string condition, T? value) 
        where T : struct
    {
        if (value.HasValue)
            builder.Where(condition);

        return builder;
    }
}