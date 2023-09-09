namespace Wildfire.Ecs;

/// <summary>
/// Provides an API for creating and manipulating entities.
/// </summary>
internal class EntityManager
{
    private readonly EntityId[] _entities;

    private uint _nextId = 1;

    public int Capacity { get; }

    public int EntityCount { get; private set; }

    public EntityManager(int capacity)
    {
        Capacity = capacity;
        _entities = new EntityId[capacity];
        EntityCount = 0;
    }

    /// <summary>
    /// Creates a new entity, returning its id.
    /// </summary>
    /// <remarks>
    /// Keep the returned entity id if you want to refer to the entity at a later point.
    /// </remarks>
    public EntityId CreateEntity()
    {
        if (EntityCount == Capacity)
            throw new InvalidOperationException("Cannot create entity as the entity capacity has been reached.");

        var entityId = new EntityId(_nextId);
        _nextId++;

        _entities[EntityCount] = entityId;
        EntityCount++;

        return entityId;
    }

    /// <summary>
    /// Checks if an entity with the specified <paramref name="id"/> exists.
    /// </summary>
    public bool HasEntity(EntityId id)
    {
        var index = Array.BinarySearch(_entities, 0, EntityCount, id);
        return index >= 0;
    }

    /// <summary>
    /// Destroys an entity with the specified <paramref name="id"/> and all its components.
    /// </summary>
    public void DestroyEntity(EntityId id)
    {
        var index = Array.BinarySearch(_entities, 0, EntityCount, id);
        if (index < 0)
            return;

        ArrayUtility.RemoveAt(_entities, EntityCount, index);
        EntityCount--;
    }
}
