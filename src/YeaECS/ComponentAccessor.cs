namespace YeaECS;

using System.Runtime.CompilerServices;

/// <summary>
/// Provides a read-only view of a <see cref="ComponentManager{T}"/>, which improves performance for bulk operations.
/// </summary>
public readonly ref struct ComponentAccessor<TComponent>
{
    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<TComponent> _componentManager;

    internal ComponentAccessor(EntityRegistry entityRegistry, ComponentManager<TComponent> componentManager)
    {
        _entityRegistry = entityRegistry;
        _componentManager = componentManager;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SparseSetEnumerator GetEnumerator() => _componentManager.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(EntityReference entity) => HasComponent(entity.Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent(Entity entity) => _componentManager.HasComponent(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponent(EntityReference entity) => ref GetComponent(entity.Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponent(Entity entity) => ref _componentManager.GetComponent(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponentOrNullRef(EntityReference entity) => ref _componentManager.GetComponentOrNullRef(entity.Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent GetComponentOrNullRef(Entity entity) => ref _componentManager.GetComponentOrNullRef(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent TryGetComponent(EntityReference entity, out bool success) => ref TryGetComponent(entity.Entity, out success);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TComponent TryGetComponent(Entity entity, out bool success) => ref _componentManager.TryGetComponent(entity, out success);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public View<TComponent> GetView() => new(_entityRegistry, _componentManager);
}
