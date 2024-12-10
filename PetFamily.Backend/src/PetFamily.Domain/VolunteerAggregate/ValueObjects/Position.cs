using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.Models;

namespace PetFamily.Domain.VolunteerAggregate.ValueObjects;

public class Position : ValueObject
{
    private Position(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Result<Position, Error> Create(int number)
    {
        if (number <= 0)
            return Errors.General.IsInvalid(nameof(Position));

        return new Position(number);
    }
   
    public Result<Position, Error> Forward() => Create(Value + 1);
    public Result<Position, Error> Back() => Create(Value - 1);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator int(Position position) => position.Value;
}