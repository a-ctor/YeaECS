namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

/// <summary>
/// Represents the combination of <see cref="Ecs.EntityRegistry"/> and <see cref="EntityId"/> for better usability.
/// </summary>
public readonly struct EntityReference
{
    public readonly EntityRegistry EntityRegistry;
    public readonly EntityId Id;

    /// <summary>
    /// Checks if this entity exists.
    /// </summary>
    public bool Exists
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => EntityRegistry.HasEntity(Id);
    }

    public EntityReference(EntityRegistry entityRegistry, EntityId id)
    {
        EntityRegistry = entityRegistry;
        Id = id;
    }

    /// <summary>
    /// Debug property to inspect components for this <see cref="EntityReference"/>.
    /// </summary>
    private object[] Components
    {
        get
        {
            var id = Id;
            return EntityRegistry.GetComponentManagers
                .Select(e => (e.TryGetComponentBoxed(id, out var component), component))
                .Where(e => e.Item1)
                .Select(e => e.component)
                .ToArray()!;
        }
    }

    /// <summary>
    /// Destroys this entity and all its components.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy()
    {
        EntityRegistry.DestroyEntity(Id);
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the this entity.
    /// Throws if this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<TComponent>(in TComponent component)
        where TComponent : struct
    {
        EntityRegistry.AddComponent(Id, in component);
    }

    /// <summary>
    /// Checks if this entity has a component of <typeparamref name="TComponent"/>.
    /// Throws if this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<TComponent>()
        where TComponent : struct
    {
        return EntityRegistry.HasComponent<TComponent>(Id);
    }

    /// <summary>
    /// Returns a reference to the component <typeparamref name="TComponent"/> of this entity.
    /// Throws if this entity is not found or no component is found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponent<TComponent>()
        where TComponent : struct
    {
        return ref EntityRegistry.GetComponent<TComponent>(Id);
    }

    /// <summary>
    /// Returns a reference to the component <typeparamref name="TComponent"/> of this entity or a reference to an uninitialized <typeparamref name="TComponent"/>.
    /// Throws if this entity is not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetOrAddComponent<TComponent>()
        where TComponent : struct
    {
        return ref EntityRegistry.GetOrAddComponent<TComponent>(Id);
    }

    /// <summary>
    /// Tries to get the component <typeparamref name="TComponent"/> of this entity.
    /// Throws if the this entity does not exist.
    /// </summary>
    /// <remarks>
    /// Always check <paramref name="success"/> before accessing the returned reference.
    /// A dummy reference is returned if <paramref name="success"/> is <c>false</c>.
    /// The data of this dummy reference is undefined.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent TryGetComponent<TComponent>(out bool success)
        where TComponent : struct
    {
        return ref EntityRegistry.TryGetComponent<TComponent>(Id, out success);
    }

    /// <summary>
    /// Removes the specified <typeparamref name="TComponent"/> from this entity.
    /// Throws if the this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<TComponent>()
        where TComponent : struct
    {
        EntityRegistry.RemoveComponent<TComponent>(Id);
    }
}
