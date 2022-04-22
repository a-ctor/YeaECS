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

    public bool HasComponent(EntityReference entity) => HasComponent(entity.Id);

    public bool HasComponent(EntityId id) => _componentManager?.HasComponent(id) ?? false;

    public ref T GetComponent(EntityReference entity) => ref GetComponent(entity.Id);

    public ref T GetComponent(EntityId id)
    {
        if (_componentManager == null)
            throw new InvalidOperationException($"Could not find a component '{typeof(T)}' for entity with entity {id}.");

        return ref _componentManager.GetComponent(id);
    }

    public ref T TryGetComponent(EntityReference entity, out bool success) => ref TryGetComponent(entity.Id, out success);

    public ref T TryGetComponent(EntityId id, out bool success)
    {
        if (_componentManager != null)
            return ref _componentManager.TryGetComponent(id, out success);

        success = false;
        return ref RefDummy<T>.Value;
    }

    public View<T> GetView()
    {
        return _componentManager != null
            ? new View<T>(_entityRegistry, _componentManager.GetEnumerator())
            : default;
    }

    internal ComponentManager<T>.Enumerator GetInternalEnumerator()
    {
        return _componentManager != null
            ? _componentManager.GetEnumerator()
            : default;
    }
}
