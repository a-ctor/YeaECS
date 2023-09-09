namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public class View<T1> : IView, IViewEnumerator
    where T1 : struct
{
    private readonly EntityRegistry _entityRegistry;

    private EntityId _current;
    private ComponentManager<T1>.Enumerator _enumerator;

    internal View(EntityRegistry entityRegistry, ComponentManager<T1>.Enumerator enumerator)
    {
        _entityRegistry = entityRegistry;
        _current = EntityId.Null;
        _enumerator = enumerator;
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1);
    }

    /// <inheritdoc />
    bool IView.Has<TComponent>()
    {
        return Supports<TComponent>();
    }

    /// <inheritdoc />
    public ref TComponent Get<TComponent>()
        where TComponent : struct
    {
        if (typeof(TComponent) == typeof(T1))
            return ref Unsafe.As<T1, TComponent>(ref _enumerator.Current);

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    public ViewEnumerator<View<T1>> GetEnumerator() => new(this);

    /// <inheritdoc />
    EntityReference IViewEnumerator.Current => new(_entityRegistry, _current);

    /// <inheritdoc />
    bool IViewEnumerator.MoveNext()
    {
        var result = _enumerator.MoveNext();
        _current = result
            ? _enumerator.CurrentEntityId
            : EntityId.Null;

        return result;
    }

    /// <inheritdoc />
    bool IViewEnumerator.MoveTo(EntityId entityId)
    {
        return _enumerator.MoveTo(entityId);
    }
}
