namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

/// <summary>
/// Represents the combination of <see cref="Ecs.EntityRegistry"/> and <see cref="Ecs.Entity"/> for better usability.
/// </summary>
public readonly struct EntityReference
{
    public readonly EntityRegistry EntityRegistry;
    public readonly Entity Entity;

    /// <summary>
    /// Checks if this entity is alive.
    /// </summary>
    public bool IsAlive
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => EntityRegistry.IsAlive(Entity);
    }

    public EntityReference(EntityRegistry entityRegistry, Entity entity)
    {
        EntityRegistry = entityRegistry;
        Entity = entity;
    }

    /// <summary>
    /// Debug property to inspect components for this <see cref="EntityReference"/>.
    /// </summary>
    private object[] Components
    {
        get
        {
            var entity = Entity;
            return EntityRegistry.GetComponentManagers
                .Select(e => (e.TryGetComponentBoxed(entity, out var component), component))
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
        EntityRegistry.DestroyEntity(Entity);
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the this entity.
    /// Throws if this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<TComponent>(in TComponent component)
    {
        EntityRegistry.AddComponent(Entity, in component);
    }

    /// <summary>
    /// Checks if this entity has a component of <typeparamref name="TComponent"/>.
    /// Throws if this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<TComponent>()
    {
        return EntityRegistry.HasComponent<TComponent>(Entity);
    }

    /// <summary>
    /// Returns a reference to the component <typeparamref name="TComponent"/> of this entity.
    /// Throws if this entity is not found or no component is found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponent<TComponent>()
    {
        return ref EntityRegistry.GetComponent<TComponent>(Entity);
    }

    /// <summary>
    /// Returns a reference to the component <typeparamref name="TComponent"/> of this entity or a reference to an uninitialized <typeparamref name="TComponent"/>.
    /// Throws if this entity is not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetOrAddComponent<TComponent>()
    {
        return ref EntityRegistry.GetOrAddComponent<TComponent>(Entity);
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
    {
        return ref EntityRegistry.TryGetComponent<TComponent>(Entity, out success);
    }

    /// <summary>
    /// Removes the specified <typeparamref name="TComponent"/> from this entity.
    /// Throws if the this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<TComponent>()
    {
        EntityRegistry.RemoveComponent<TComponent>(Entity);
    }
}
