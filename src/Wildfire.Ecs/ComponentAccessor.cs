namespace Wildfire.Ecs;

/// <summary>
/// Provides a read-only view of a <see cref="ComponentManager{T}"/>, which improves performance for bulk operations.
/// </summary>
public readonly ref struct ComponentAccessor<T>
    where T : struct
{
    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<T>? _componentManager;

    public bool IsDefault => _componentManager == null;

    internal ComponentAccessor(EntityRegistry entityRegistry, ComponentManager<T> componentManager)
    {
        _entityRegistry = entityRegistry;
        _componentManager = componentManager;
    }

    public bool HasComponent(EntityReference entity) => HasComponent(entity.Entity);

    public bool HasComponent(Entity entity) => _componentManager?.HasComponent(entity) ?? false;

    public ref T GetComponent(EntityReference entity) => ref GetComponent(entity.Entity);

    public ref T GetComponent(Entity entity)
    {
        if (_componentManager == null)
            throw new InvalidOperationException($"Could not find a component '{typeof(T)}' for entity {entity}.");

        return ref _componentManager.GetComponent(entity);
    }

    public ref T TryGetComponent(EntityReference entity, out bool success) => ref TryGetComponent(entity.Entity, out success);

    public ref T TryGetComponent(Entity entity, out bool success)
    {
        if (_componentManager != null)
            return ref _componentManager.TryGetComponent(entity, out success);

        success = false;
        return ref RefDummy<T>.Value;
    }

    public View<T> GetView()
    {
        return _componentManager != null
            ? new View<T>(_entityRegistry, _componentManager.GetEnumerator())
            : new View<T>(_entityRegistry, default);
    }

    internal ComponentManager<T>.Enumerator GetInternalEnumerator()
    {
        return _componentManager != null
            ? _componentManager.GetEnumerator()
            : default;
    }
}
