namespace YeaECS;

/// <summary>
/// Provides an API for creating and manipulating entities.
/// </summary>
internal class EntityManager
{
    // todo add id reuse via generations and improve tracking of entities
    private readonly uint[] _entityGenerations;

    private uint _nextId = 0;

    public int Capacity { get; }

    public int EntityCount { get; private set; }

    public EntityManager(int capacity)
    {
        Capacity = capacity;
        _entityGenerations = new uint[capacity];
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

        var id = _nextId++;
        var generation = ++_entityGenerations[id];
        var entityId = new Entity(generation, id);

        EntityCount++;

        return entityId;
    }

    /// <summary>
    /// Checks if an entity with the specified <paramref name="entity"/> exists.
    /// </summary>
    public bool HasEntity(Entity entity)
    {
        var index = entity.Id;
        return index < _entityGenerations.Length && _entityGenerations[index] == entity.Generation;
    }

    /// <summary>
    /// Destroys an entity with the specified <paramref name="entity"/> and all its components.
    /// </summary>
    public void DestroyEntity(Entity entity)
    {
        var index = entity.Id;
        if (index < _entityGenerations.Length && _entityGenerations[index] == entity.Generation)
        {
            _entityGenerations[index]++;
            EntityCount--;
        }
    }
}
