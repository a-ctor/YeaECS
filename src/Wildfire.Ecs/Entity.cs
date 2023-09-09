namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

/// <summary>
/// A reference to an entity in the ECS.
/// The references entity might not exists anymore.
/// </summary>
public readonly struct Entity : IEquatable<Entity>, IComparable<Entity>, IComparable
{
    // todo remove comparability after introducing the sparse set -> wrong and then not needed anymore

    public static readonly Entity Null = new();

    internal readonly uint Generation;
    internal readonly uint Id;

    public Entity(uint generation, uint id)
    {
        Generation = generation;
        Id = id;
    }

    internal long Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.As<Entity, long>(ref Unsafe.AsRef(in this));
    }

    /// <inheritdoc />
    public bool Equals(Entity other) => Value == other.Value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => $"<{Id}@{Generation}>";

    /// <inheritdoc />
    public int CompareTo(Entity other) => Id.CompareTo(other.Id);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;

        return obj is Entity other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Entity)}");
    }

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);

    public static bool operator <(Entity left, Entity right) => left.Id < right.Id;

    public static bool operator >(Entity left, Entity right) => left.Id > right.Id;

    public static bool operator <=(Entity left, Entity right) => left.Id <= right.Id;

    public static bool operator >=(Entity left, Entity right) => left.Id >= right.Id;
}
