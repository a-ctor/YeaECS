namespace Wildfire.Ecs;

/// <summary>
/// Represent the id of an entity, which is used to manipulate it.
/// </summary>
/// <remarks>
/// There is no guarantee that the id points to a valid entity.
/// </remarks>
public readonly struct EntityId : IEquatable<EntityId>, IComparable<EntityId>, IComparable
{
    public static readonly EntityId Null = new();

    private readonly int _value;

    public EntityId(int value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public bool Equals(EntityId other) => _value == other._value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value;

    /// <inheritdoc />
    public override string ToString() => $"<{_value}>";

    /// <inheritdoc />
    public int CompareTo(EntityId other)
    {
        return _value.CompareTo(other._value);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;

        return obj is EntityId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(EntityId)}");
    }

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);

    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);

    public static bool operator <(EntityId left, EntityId right) => left._value < right._value;

    public static bool operator >(EntityId left, EntityId right) => left._value > right._value;

    public static bool operator <=(EntityId left, EntityId right) => left._value <= right._value;

    public static bool operator >=(EntityId left, EntityId right) => left._value >= right._value;
}
