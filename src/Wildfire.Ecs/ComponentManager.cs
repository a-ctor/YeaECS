namespace Wildfire.Ecs;

/// <summary>
/// Manages a certain component <typeparamref name="TComponent"/> for a set of entities.
/// </summary>
internal class ComponentManager<TComponent> : IComponentManager
{
    private readonly SparseSet<TComponent> _sparseSet;
    
    public int Capacity { get; }

    public int ComponentCount => _sparseSet.Count;

    public ComponentManager(int capacity)
    {
        Capacity = capacity;
        _sparseSet = new SparseSet<TComponent>(Capacity, Capacity);
    }

    private ComponentManager(ComponentManager<TComponent> other)
    {
        _sparseSet = other._sparseSet.Clone();
        Capacity = other.Capacity;
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the entity with the specified <paramref name="entity"/>.
    /// </summary>
    public void AddComponent(Entity entity, in TComponent component) => _sparseSet.Add(entity, in component);

    internal ComponentManager<TComponent> Clone() => new(this);

    /// <summary>
    /// Checks if the entity with the specified <paramref name="entity"/> has a component of <typeparamref name="TComponent"/>.
    /// </summary>
    public bool HasComponent(Entity entity) => _sparseSet.Has(entity);

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="entity"/>.
    /// Throws if no component is found.
    /// </summary>
    public ref TComponent GetComponent(Entity entity) => ref _sparseSet.Get(entity);

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="entity"/>,
    /// or returns a null reference if no such component was found.
    /// </summary>
    public ref TComponent GetComponentOrNullRef(Entity entity) => ref _sparseSet.GetOrNullRef(entity);

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="entity"/>.
    /// Returns a reference to an uninitialized component if no component was found.
    /// </summary>
    public ref TComponent GetOrAddComponent(Entity entity) => ref _sparseSet.GetOrCreate(entity);

    /// <summary>
    /// Tries to get the component of the entity with the specified <paramref name="entity"/>.
    /// </summary>
    /// <remarks>
    /// Always check <paramref name="success"/> before accessing the returned reference.
    /// A dummy reference is returned if <paramref name="success"/> is <c>false</c>.
    /// The data of this dummy reference is undefined.
    /// </remarks>
    public ref TComponent TryGetComponent(Entity entity, out bool success) => ref _sparseSet.TryGet(entity, out success);

    /// <summary>
    /// Removes the specified <typeparamref name="TComponent"/> from the entity with the specified <paramref name="entity"/>.
    /// </summary>
    public void RemoveComponent(Entity entity)
    {
        _sparseSet.Remove(entity);
    }

    public SparseSetEnumerator GetEnumerator() => _sparseSet.GetEnumerator();

    /// <inheritdoc />
    Type IComponentManager.ComponentType => typeof(TComponent);

    /// <inheritdoc />
    object IComponentManager.GetComponentBoxed(Entity entity) => GetComponent(entity);

    /// <inheritdoc />
    bool IComponentManager.TryGetComponentBoxed(Entity entity, out object? result)
    {
        ref var resultUnboxed = ref TryGetComponent(entity, out var success);
        if (success)
        {
            result = resultUnboxed;
            return true;
        }

        result = null;
        return false;
    }
}
