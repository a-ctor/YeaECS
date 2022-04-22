namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public class EntityRegistry
{
    internal ref struct BuilderToken
    {
        public readonly EntityRegistry EntityRegistry;
        public readonly EntityId Id;

        private bool _disposed;

        public BuilderToken(EntityRegistry entityRegistry, EntityId id)
        {
            EntityRegistry = entityRegistry;
            Id = id;
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            EntityRegistry.OnEntityCreated?.Invoke(new EntityReference(EntityRegistry, Id));
        }
    }

    private readonly EntityManager _entityManager;
    private readonly ComponentManagerLookup _componentManagers;

    public int Capacity { get; }

    public int EntityCount => _entityManager.EntityCount;

    public event EntityCreatingEventHandler? OnEntityCreating;

    public event EntityCreatedEventHandler? OnEntityCreated;

    public event EntityDeletingEventHandler? OnEntityDeleting;

    public event EntityDeletedEventHandler? OnEntityDeleted;

    public EntityRegistry(int capacity)
    {
        Capacity = capacity;
        _entityManager = new EntityManager(capacity);
        _componentManagers = new ComponentManagerLookup(capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EntityReference CreateReference(EntityId entityId) => new(this, entityId);

    /// <summary>
    /// Creates a new entity, returning its id.
    /// </summary>
    /// <remarks>
    /// Keep the returned entity id if you want to refer to the entity at a later point.
    /// </remarks>
    public EntityReference CreateEntity()
    {
        var entityId = _entityManager.CreateEntity();
        var entity = new EntityReference(this, entityId);
        OnEntityCreating?.Invoke(entity);
        OnEntityCreated?.Invoke(entity);

        return new EntityReference(this, entityId);
    }

    public EntityBuilder BuildEntity()
    {
        var entityId = _entityManager.CreateEntity();
        var entity = new EntityReference(this, entityId);
        OnEntityCreating?.Invoke(entity);

        return new EntityBuilder(new BuilderToken(this, entityId));
    }

    /// <summary>
    /// Checks if an entity with the specified <paramref name="id"/> exists.
    /// </summary>
    public bool HasEntity(EntityId id)
    {
        return _entityManager.HasEntity(id);
    }

    /// <summary>
    /// Destroys an entity with the specified <paramref name="id"/> and all its components.
    /// </summary>
    public void DestroyEntity(EntityId id)
    {
        OnEntityDeleting?.Invoke(new EntityReference(this, id));

        _entityManager.DestroyEntity(id);
        foreach (var componentManager in _componentManagers.Values)
            componentManager.RemoveComponent(id);

        OnEntityDeleted?.Invoke(id);
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the entity with the specified <paramref name="id"/>.
    /// Throws if the specified entity does not exist.
    /// </summary>
    public void AddComponent<TComponent>(EntityId id, in TComponent component)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");

        var componentManager = _componentManagers.GetOrAdd<TComponent>();
        componentManager.AddComponent(id, in component);
    }

    /// <summary>
    /// Checks if the entity with the specified <paramref name="id"/> has a component of <typeparamref name="TComponent"/>.
    /// Throws if the specified entity does not exist.
    /// </summary>
    public bool HasComponent<TComponent>(EntityId id)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");

        return _componentManagers.TryGet<TComponent>(out var componentManager) && componentManager.HasComponent(id);
    }

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="id"/>.
    /// Throws if the specified entity is not found or no component is found.
    /// </summary>
    public ref TComponent GetComponent<TComponent>(EntityId id)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");
        if (!_componentManagers.TryGet<TComponent>(out var componentManager))
            throw new InvalidOperationException($"Could not find a component '{typeof(TComponent)}' for entity with id {id}.");

        return ref componentManager.GetComponent(id);
    }

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="id"/> or a reference to an uninitialized <typeparamref name="TComponent"/>.
    /// Throws if the specified entity is not found
    /// </summary>
    public ref TComponent GetOrAddComponent<TComponent>(EntityId id)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");

        var componentManager = _componentManagers.GetOrAdd<TComponent>();
        return ref componentManager.GetOrAddComponent(id);
    }

    /// <summary>
    /// Tries to get the component of the entity with the specified <paramref name="id"/>.
    /// Throws if the specified entity does not exist.
    /// </summary>
    /// <remarks>
    /// Always check <paramref name="success"/> before accessing the returned reference.
    /// A dummy reference is returned if <paramref name="success"/> is <c>false</c>.
    /// The data of this dummy reference is undefined.
    /// </remarks>
    public ref TComponent TryGetComponent<TComponent>(EntityId id, out bool success)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");
        if (_componentManagers.TryGet<TComponent>(out var componentManager))
            return ref componentManager.TryGetComponent(id, out success);

        success = false;
        return ref RefDummy<TComponent>.Value;
    }

    /// <summary>
    /// Removes the specified <typeparamref name="TComponent"/> from the entity with the specified <paramref name="id"/>.
    /// Throws if the specified entity does not exist.
    /// </summary>
    public void RemoveComponent<TComponent>(EntityId id)
        where TComponent : struct
    {
        if (!HasEntity(id))
            throw new InvalidOperationException("The specified entity does not exist.");
        if (!_componentManagers.TryGet<TComponent>(out var componentManager))
            return;

        componentManager.RemoveComponent(id);
    }

    /// <summary>
    /// Returns a <see cref="ComponentAccessor{T}"/> for the specified <typeparamref name="TComponent"/>.
    /// Provides better performance for bulk operations.
    /// </summary>
    public ComponentAccessor<TComponent> GetComponentAccessor<TComponent>()
        where TComponent : struct
    {
        return _componentManagers.TryGet<TComponent>(out var componentManager)
            ? new ComponentAccessor<TComponent>(this, componentManager)
            : default;
    }


    /// <summary>
    /// Returns an optional view for the specified component <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public OptionalView<T> OptionalViewOf<T>()
        where T : struct
    {
        return new OptionalView<T>(GetComponentAccessor<T>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns an optional view for the specified components <typeparamref name="T1"/> and <typeparamref name="T2"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public OptionalView<T1, T2> OptionalViewOf<T1, T2>()
        where T1 : struct
        where T2 : struct
    {
        return new OptionalView<T1, T2>(GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns an optional view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public OptionalView<T1, T2, T3> OptionalViewOf<T1, T2, T3>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        return new OptionalView<T1, T2, T3>(GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns an optional view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, and <typeparamref name="T4"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public OptionalView<T1, T2, T3, T4> OptionalViewOf<T1, T2, T3, T4>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        return new OptionalView<T1, T2, T3, T4>(GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator(),
            GetComponentAccessor<T4>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns an optional view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, and <typeparamref name="T5"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public OptionalView<T1, T2, T3, T4, T5> OptionalViewOf<T1, T2, T3, T4, T5>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        return new OptionalView<T1, T2, T3, T4, T5>(GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator(),
            GetComponentAccessor<T4>().GetInternalEnumerator(),
            GetComponentAccessor<T5>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns a view for the specified component <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public View<T> ViewOf<T>()
        where T : struct
    {
        return new View<T>(this, GetComponentAccessor<T>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns a view for the specified components <typeparamref name="T1"/> and <typeparamref name="T2"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public View<T1, T2> ViewOf<T1, T2>()
        where T1 : struct
        where T2 : struct
    {
        return new View<T1, T2>(
            this,
            GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns a view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public View<T1, T2, T3> ViewOf<T1, T2, T3>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        return new View<T1, T2, T3>(
            this,
            GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns a view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, and <typeparamref name="T4"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public View<T1, T2, T3, T4> ViewOf<T1, T2, T3, T4>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        return new View<T1, T2, T3, T4>(
            this,
            GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator(),
            GetComponentAccessor<T4>().GetInternalEnumerator());
    }

    /// <summary>
    /// Returns a view for the specified components <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, and <typeparamref name="T5"/>.
    /// </summary>
    /// <remarks>
    /// The view instance must be on the stack when iterating, otherwise bad things might happen.
    /// It is best to not store view instances as they are cheap to create.
    /// </remarks>
    public View<T1, T2, T3, T4, T5> ViewOf<T1, T2, T3, T4, T5>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        return new View<T1, T2, T3, T4, T5>(
            this,
            GetComponentAccessor<T1>().GetInternalEnumerator(),
            GetComponentAccessor<T2>().GetInternalEnumerator(),
            GetComponentAccessor<T3>().GetInternalEnumerator(),
            GetComponentAccessor<T4>().GetInternalEnumerator(),
            GetComponentAccessor<T5>().GetInternalEnumerator());
    }

    internal IEnumerable<IComponentManager> GetComponentManagers => _componentManagers.Values;
}
