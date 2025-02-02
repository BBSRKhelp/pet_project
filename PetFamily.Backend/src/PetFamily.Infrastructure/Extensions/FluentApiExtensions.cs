using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetFamily.Infrastructure.Extensions;

public static class FluentApiExtensions
{
    public static PropertyBuilder<IReadOnlyList<TValueObject>> ValueObjectsCollectionJsonConversion<TValueObject, TDto>(
        this PropertyBuilder<IReadOnlyList<TValueObject>> builder,
        Func<TValueObject, TDto> toDtoSelector,
        Func<TDto, TValueObject> toValueObjectSelector)
    {
        return builder.HasConversion(
                valueObjects => SerializeValueObjectsCollection(valueObjects, toDtoSelector),
                json => DeserializeDtoCollection(json, toValueObjectSelector),
                ValueComparer<TValueObject>())
            .HasColumnType("jsonb");
    }

    private static string SerializeValueObjectsCollection<TValueObject, TDto>(
        IReadOnlyList<TValueObject> valueObjects,
        Func<TValueObject, TDto> selector)
    {
        var dtos = valueObjects.Select(selector);
        return JsonSerializer.Serialize(dtos, JsonSerializerOptions.Default);
    }

    private static IReadOnlyList<TValueObject> DeserializeDtoCollection<TValueObject, TDto>(
        string json,
        Func<TDto, TValueObject> selector)
    {
        var dtos = JsonSerializer.Deserialize<IEnumerable<TDto>>(json, JsonSerializerOptions.Default) ?? [];
        return dtos.Select(selector).ToList();
    }
    
    private static ValueComparer ValueComparer<TValueObject>()
    {
        return new ValueComparer<IReadOnlyList<TValueObject>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c.ToList());
    }
}