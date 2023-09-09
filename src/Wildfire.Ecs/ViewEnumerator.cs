namespace Wildfire.Ecs;

using System.Collections;

public unsafe struct ViewEnumerator<TFilterObj> : IEnumerator<EntityReference>
{
    private readonly EntityRegistry _entityRegistry;
    private readonly delegate*<TFilterObj, Entity, bool> _filter;
    private readonly TFilterObj _filterObj;

    private SparseSetEnumerator _enumerator;

    public ViewEnumerator(EntityRegistry entityRegistry, TFilterObj filterObj, delegate*<TFilterObj, Entity, bool> filter, SparseSetEnumerator enumerator)
    {
        _entityRegistry = entityRegistry;
        _filterObj = filterObj;
        _filter = filter;
        _enumerator = enumerator;
    }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public EntityReference Current => new(_entityRegistry, _enumerator.Current);

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        while (_enumerator.MoveNext())
        {
            if (_filter(_filterObj, _enumerator.Current))
                return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void Reset() => _enumerator.Reset();
}
