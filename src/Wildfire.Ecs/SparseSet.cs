namespace Wildfire.Ecs;

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

internal struct DenseEntry
{
    public uint Generation;
    public uint IdOrIndex;

    public Entity ToEntity()
    {
        return Generation != 0
            ? new Entity(Generation, IdOrIndex)
            : throw new InvalidOperationException();
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return Generation == 0
            ? $"empty -> {IdOrIndex}"
            : $"<{IdOrIndex}@{Generation}>";
    }
}

internal struct SparseEntry
{
    public uint Generation;
    public uint Index;

    /// <inheritdoc />
    public override string ToString()
    {
        return Generation == 0
            ? "empty"
            : $"<{Index}@{Generation}>";
    }
}

/// <summary>
/// Provides an implementation of a sparse set that also provides efficient enumeration
/// </summary>
internal class SparseSet<T> : IEnumerable<Entity>
{
    public struct Enumerator : IEnumerator<Entity>
    {
        private readonly SparseSet<T> _sparseSet;
        private int _index = -1;

        public Enumerator(SparseSet<T> sparseSet)
        {
            _sparseSet = sparseSet;
        }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public Entity Current => _sparseSet._denseEntries[_index].ToEntity();

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (_index < _sparseSet._denseEntries.Length - 1)
            {
                _index++;
                var index = _index;
                if (_sparseSet._denseEntries[index].Generation != 0)
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset() => _index = -1;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    // There are two dense arrays that have the same size and shared indices
    // One stores the value while the other stores metadata
    // Not all indices contain valid entries and the metadata also acts as a free linked list in such a case
    // Marker for empty entries is entity.Generation == 0
    // For empty entries, entity.Id represents the index of the next free entry
    private T[] _denseValues;
    private DenseEntry[] _denseEntries;

    // The count of elements in the dense array that need to be search to find all valid entries
    private int _denseCount;

    // Linked list details for the free slots linked list in _denseEntities
    // Allows efficient & dense adding/removing of elements
    private int _denseFreeHead;

    // Lookup table for sparse[entity.Id] -> (Version, Index)
    // with the Index only being valid if Version == entity.Version
    private SparseEntry[] _sparseEntities;

    private int _count;

    public SparseSet(int denseCapacity, int sparseCapacity)
    {
        _denseValues = new T[denseCapacity];
        _denseEntries = new DenseEntry[denseCapacity];
        _denseCount = 0;
        _denseFreeHead = 0;
        _sparseEntities = new SparseEntry[sparseCapacity];

        // Initialize the link
        for (var i = 0; i < _denseEntries.Length; i++)
            _denseEntries[i].IdOrIndex = (uint)(i + 1);
    }

    public int Count => _count;

    /// <summary>
    /// Adds <paramref name="value"/> to the sparse set for <paramref name="entity"/>.
    /// Throws an exception if <paramref name="entity"/> already has a value.
    /// </summary>
    public ref T Add(Entity entity, in T value)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var valueRef = ref GetOrCreate(entity, out var created);
        if (!created)
            throw new InvalidOperationException($"A value for entity {entity} already exists in the sparse set.");

        valueRef = value;
        return ref valueRef;
    }

    /// <summary>
    /// Clears the sparse set, removing all values.
    /// </summary>
    public void Clear()
    {
        for (var i = 0; i < _denseValues.Length; i++)
        {
            _denseValues[i] = default!;
            _denseEntries[i].Generation = 0;
            _denseEntries[i].IdOrIndex = (uint)(i + 1);
        }

        _denseCount = 0;
        _denseFreeHead = 0;

        for (var i = 0; i < _sparseEntities.Length; i++)
            _sparseEntities[i] = new SparseEntry();

        _count = 0;
    }

    /// <summary>
    /// Returns a reference to the value in the sparse set for <paramref name="entity"/>.
    /// Throws an exception if no such value exists.
    /// </summary>
    public ref T Get(Entity entity)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var value = ref TryGet(entity, out var found);
        if (!found)
            throw new InvalidOperationException($"Cannot find a value for the specified entity {entity} in the sparse set.");

        return ref value;
    }

    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Returns a reference to the value in the sparse set for <paramref name="entity"/>.
    /// Creates an entry in the sparse set if no existing value was found.
    /// </summary>
    public ref T GetOrCreate(Entity entity)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        return ref GetOrCreate(entity, out _);
    }

    /// <summary>
    /// Returns a reference to the value in the sparse set for <paramref name="entity"/>.
    /// Creates an entry in the sparse set if no existing value was found.
    /// </summary>
    public ref T GetOrCreate(Entity entity, out bool created)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var sparseSetEntry = ref GetOrCreateSparseSetEntry(entity);
        if (entity.Generation == sparseSetEntry.Generation)
        {
            created = false;
            return ref _denseValues[sparseSetEntry.Index];
        }

        if (sparseSetEntry.Generation != 0)
            throw new InvalidOperationException("There already exists a value from a different generation.");

        var freeIndex = _denseFreeHead;
        if (freeIndex >= _denseValues.Length)
            throw new InvalidOperationException("SparseSet is full and cannot store any more items.");

        sparseSetEntry.Generation = entity.Generation;
        sparseSetEntry.Index = unchecked((uint)freeIndex);
        
        ref var denseEntity = ref _denseEntries[freeIndex];
        _denseFreeHead = unchecked((int)denseEntity.IdOrIndex);
        denseEntity.Generation = entity.Generation;
        denseEntity.IdOrIndex = entity.Id;

        _denseCount = Math.Max(_denseCount, freeIndex + 1);
        _count++;

        created = true;
        return ref _denseValues[freeIndex];
    }

    /// <summary>
    /// Checks if a value for <paramref name="entity"/> exists in the sparse set.
    /// </summary>
    public bool Has(Entity entity)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var sparseSetEntry = ref GetSparseSetEntry(entity);
        return !Unsafe.IsNullRef(ref sparseSetEntry) && sparseSetEntry.Generation == entity.Generation;
    }

    /// <summary>
    /// Removes any value associated with <paramref name="entity"/> from the sparse set.
    /// Returns a boolean indicating if a value has been removed.
    /// </summary>
    public bool Remove(Entity entity)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var sparseSetEntry = ref GetSparseSetEntry(entity);
        if (entity.Generation != sparseSetEntry.Generation)
            return false;

        var indexToRemove = unchecked((int)sparseSetEntry.Index);
        sparseSetEntry.Generation = 0;
        sparseSetEntry.Index = 0;

        // Determine the index of the linked list item that we need to update or -1 if we need to update the head
        var targetEntryToUpdate = -1;
        var current = _denseFreeHead;
        while (true)
        {
            ref var denseEntry = ref _denseEntries[current];
            Debug.Assert(denseEntry.Generation == 0, "denseEntry.Generation == 0");
            current = unchecked((int)denseEntry.IdOrIndex);
            if (current >= indexToRemove)
                break;

            targetEntryToUpdate = current;
        }

        _denseValues[indexToRemove] = default!;
        _denseEntries[indexToRemove].Generation = 0;
        if (targetEntryToUpdate == -1)
        {
            _denseEntries[indexToRemove].IdOrIndex = unchecked((uint)_denseFreeHead);
            _denseFreeHead = indexToRemove;
        }
        else
        {
            Debug.Assert(_denseEntries[targetEntryToUpdate].Generation == 0, "_denseEntries[targetEntryToUpdate].Generation == 0");
            _denseEntries[indexToRemove].IdOrIndex = _denseEntries[targetEntryToUpdate].IdOrIndex;
            _denseEntries[targetEntryToUpdate].IdOrIndex = unchecked((uint)indexToRemove);
        }

        if (indexToRemove == _denseCount - 1)
        {
            var newDenseCount = indexToRemove;
            while (newDenseCount > 0 && _denseEntries[newDenseCount - 1].Generation == 0)
                newDenseCount--;
            _denseCount = newDenseCount;
        }

        _count--;

        return true;
    }

    /// <summary>
    /// Tries to get a reference to the value in the sparse set for <paramref name="entity"/>.
    /// Out parameter <paramref name="found"/> indicates if a value has been returned.
    /// </summary>
    public ref T TryGet(Entity entity, out bool found)
    {
        if (entity.Generation == 0)
            throw new ArgumentException("The specified entity is not valid.", nameof(entity));

        ref var sparseSetEntry = ref GetSparseSetEntry(entity);
        if (entity.Generation != sparseSetEntry.Generation)
        {
            found = false;
            return ref Unsafe.NullRef<T>();
        }

        found = true;
        return ref _denseValues[sparseSetEntry.Index];
    }

    private ref SparseEntry GetSparseSetEntry(Entity entity)
    {
        var id = entity.Id;
        var sparseSetEntries = _sparseEntities;
        return ref id >= sparseSetEntries.Length 
            ? ref Unsafe.NullRef<SparseEntry>() 
            : ref sparseSetEntries[id];
    }

    private ref SparseEntry GetOrCreateSparseSetEntry(Entity entity)
    {
        // todo extend the sparse set if necessary
        ref var result = ref GetSparseSetEntry(entity);
        if (Unsafe.IsNullRef(ref result))
            throw new InvalidOperationException("SparseSet is not big enough for the specified entity.");

        return ref result;
    }
}
