namespace Wildfire.Ecs;

/// <summary>
/// Provides an API for creating and manipulating entities.
/// </summary>
internal class EntityManager
{
    private readonly Entity[] _entities;

    private uint _nextId = 1;

    public int Capacity { get; }

    public int EntityCount { get; private set; }

    public EntityManager(int capacity)
    {
        Capacity = capacity;
        _entities = new Entity[capacity];
        EntityCount = 0;
    }

    /// <summary>
    /// Creates a new entity, returning a reference that can be used to interact with the entity.
    /// </summary>
    /// <remarks>
    /// Keep the returned entity if you want to refer to the entity at a later point.
    /// </remarks>
    public Entity CreateEntity()
    {
        if (EntityCount == Capacity)
            throw new InvalidOperationException("Cannot create entity as the entity capacity has been reached.");

        var entityId = new Entity(_nextId);
        _nextId++;

        _entities[EntityCount] = entityId;
        EntityCount++;

        return entityId;
    }

    /// <summary>
    /// Checks if an entity with the specified <paramref name="entity"/> exists.
    /// </summary>
    public bool HasEntity(Entity entity)
    {
        var index = Array.BinarySearch(_entities, 0, EntityCount, entity);
        return index >= 0;
    }

    /// <summary>
    /// Destroys an entity with the specified <paramref name="entity"/> and all its components.
    /// </summary>
    public void DestroyEntity(Entity entity)
    {
        var index = Array.BinarySearch(_entities, 0, EntityCount, entity);
        if (index < 0)
            return;

        ArrayUtility.RemoveAt(_entities, EntityCount, index);
        EntityCount--;
    }
}
