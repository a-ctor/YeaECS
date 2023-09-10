namespace YeaECS;

using System.Runtime.CompilerServices;

/// <summary>
/// A reference to an entity in the ECS.
/// The references entity might not exists anymore.
/// </summary>
public readonly struct Entity : IEquatable<Entity>
{
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

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}
