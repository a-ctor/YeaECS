namespace Wildfire.Ecs;

/// <summary>
/// A reference to an entity in the ECS.
/// The references entity might not exists anymore.
/// </summary>
public readonly struct Entity : IEquatable<Entity>, IComparable<Entity>, IComparable
{
    public static readonly Entity Null = new();

    private readonly uint _value;

    public Entity(uint value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public bool Equals(Entity other) => _value == other._value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => unchecked((int)_value);

    /// <inheritdoc />
    public override string ToString() => $"<{_value}>";

    /// <inheritdoc />
    public int CompareTo(Entity other)
    {
        return _value.CompareTo(other._value);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;

        return obj is Entity other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Entity)}");
    }

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);

    public static bool operator <(Entity left, Entity right) => left._value < right._value;

    public static bool operator >(Entity left, Entity right) => left._value > right._value;

    public static bool operator <=(Entity left, Entity right) => left._value <= right._value;

    public static bool operator >=(Entity left, Entity right) => left._value >= right._value;
}
