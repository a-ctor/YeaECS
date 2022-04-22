namespace Wildfire.Ecs;

/// <summary>
/// Manages a certain component <typeparamref name="TComponent"/> for a set of entities.
/// </summary>
internal class ComponentManager<TComponent> : IComponentManager
    where TComponent : struct
{
    public struct Enumerator
    {
        private readonly ComponentManager<TComponent>? _componentManager;
        private int _index = -1;

        public bool IsDefault => _componentManager == null;

        public int ComponentCount => _componentManager?.ComponentCount ?? 0;

        public Enumerator(ComponentManager<TComponent> componentManager)
        {
            _componentManager = componentManager;
        }

        public ref TComponent Current => ref _componentManager!._data[_index];

        public EntityId CurrentEntityId => _componentManager!._entityIds[_index];

        public bool MoveNext()
        {
            if (_componentManager == null)
                return false;

            var newIndex = _index + 1;
            if (newIndex < 0 || newIndex >= _componentManager.ComponentCount)
                return false;

            _index = newIndex;
            return true;
        }

        /// <summary>
        /// Moves the enumerator forward until the entity with the specified <paramref name="id"/> is found or,
        /// to the last position where the entity id is smaller than the specified <paramref name="id"/>.
        /// </summary>
        /// <remarks>
        /// Similarly to <see cref="MoveNext"/>, accessing <see cref="Current"/> or <see cref="CurrentEntityId"/> is only valid if
        /// this method returns <c>true</c>.
        /// This API provides an efficient way to iterate over multiple components as one enumerator is moved using
        /// <see cref="MoveNext"/> and the other(s) are moved to the corresponding entity id.
        /// </remarks>
        public bool MoveTo(EntityId id)
        {
            if (_componentManager == null)
                return false;

            var newIndex = _index;
            var count = _componentManager.ComponentCount;
            do
            {
                newIndex++;
            } while (newIndex < count && _componentManager._entityIds[newIndex] < id);

            if (newIndex >= count)
            {
                _index = newIndex;
                return false;
            }

            // If we hit the id we act as if we moved to it 
            if (_componentManager._entityIds[newIndex] == id)
            {
                _index = newIndex;
                return true;
            }

            // otherwise we position directly in front -> another move is necessary to access
            _index = newIndex - 1;
            return false;
        }
    }

    private readonly TComponent[] _data;
    private readonly EntityId[] _entityIds;

    public int Capacity { get; }

    public int ComponentCount { get; private set; }

    public ComponentManager(int capacity)
    {
        Capacity = capacity;
        _data = new TComponent[capacity];
        _entityIds = new EntityId[capacity];
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the entity with the specified <paramref name="id"/>.
    /// </summary>
    public void AddComponent(EntityId id, in TComponent component)
    {
        var count = ComponentCount;
        if (count == Capacity)
            throw new InvalidOperationException("Cannot add component as the component capacity is exceeded.");

        var insertionIndex = ArrayUtility.FindEntityIdInsertionIndex(_entityIds, count, id);
        ArrayUtility.InsertAt(_data, count, insertionIndex, in component);
        ArrayUtility.InsertAt(_entityIds, count, insertionIndex, in id);
        ComponentCount++;
    }

    /// <summary>
    /// Checks if the entity with the specified <paramref name="id"/> has a component of <typeparamref name="TComponent"/>.
    /// </summary>
    public bool HasComponent(EntityId id) => FindIndexOf(id) >= 0;

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="id"/>.
    /// Throws if no component is found.
    /// </summary>
    public ref TComponent GetComponent(EntityId id)
    {
        var index = FindIndexOf(id);
        if (index < 0)
            throw new InvalidOperationException($"Could not find a component '{typeof(TComponent)}' for entity with id {id}.");

        return ref _data[index];
    }

    /// <summary>
    /// Returns a reference to the component of the entity with the specified <paramref name="id"/>.
    /// Returns a reference to an uninitialized component if no component was found.
    /// </summary>
    public ref TComponent GetOrAddComponent(EntityId id)
    {
        var index = FindIndexOf(id);
        if (index < 0)
        {
            AddComponent(id, default);
            index = FindIndexOf(id);
        }

        return ref _data[index];
    }

    /// <summary>
    /// Tries to get the component of the entity with the specified <paramref name="id"/>.
    /// </summary>
    /// <remarks>
    /// Always check <paramref name="success"/> before accessing the returned reference.
    /// A dummy reference is returned if <paramref name="success"/> is <c>false</c>.
    /// The data of this dummy reference is undefined.
    /// </remarks>
    public ref TComponent TryGetComponent(EntityId id, out bool success)
    {
        var index = FindIndexOf(id);
        if (index < 0)
        {
            success = false;
            return ref RefDummy<TComponent>.Value; // We have to return something, so we use a dummy value
        }

        success = true;
        return ref _data[index];
    }

    /// <summary>
    /// Removes the specified <typeparamref name="TComponent"/> from the entity with the specified <paramref name="id"/>.
    /// </summary>
    public void RemoveComponent(EntityId id)
    {
        var removalIndex = FindIndexOf(id);
        if (removalIndex < 0)
            return;

        var count = ComponentCount;
        ArrayUtility.RemoveAt(_data, count, removalIndex);
        ArrayUtility.RemoveAt(_entityIds, count, removalIndex);

        ComponentCount--;
    }

    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    Type IComponentManager.ComponentType => typeof(TComponent);

    /// <inheritdoc />
    object IComponentManager.GetComponentBoxed(EntityId id) => GetComponent(id);

    /// <inheritdoc />
    bool IComponentManager.TryGetComponentBoxed(EntityId id, out object? result)
    {
        ref var resultUnboxed = ref TryGetComponent(id, out var success);
        if (success)
        {
            result = resultUnboxed;
            return true;
        }

        result = null;
        return false;
    }

    internal void ResetTo(ComponentManager<TComponent> other)
    {
        if (Capacity != other.Capacity)
            throw new InvalidOperationException();
        
        Array.Copy(other._data, _data, Capacity);
        Array.Copy(other._entityIds, _entityIds, Capacity);
        ComponentCount = other.ComponentCount;
    }

    private int FindIndexOf(EntityId id) => Array.BinarySearch(_entityIds, 0, ComponentCount, id);
}
