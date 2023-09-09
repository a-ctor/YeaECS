namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public class View<T1, T2> : IView, IViewEnumerator
    where T1 : struct
    where T2 : struct
{
    private readonly EntityRegistry _entityRegistry;
    private readonly int _preferredEnumerator;

    private EntityId _current;
    private ComponentManager<T1>.Enumerator _enumerator1;
    private ComponentManager<T2>.Enumerator _enumerator2;

    internal View(
        EntityRegistry entityRegistry,
        ComponentManager<T1>.Enumerator enumerator1,
        ComponentManager<T2>.Enumerator enumerator2)
    {
        _entityRegistry = entityRegistry;
        _current = EntityId.Null;
        _enumerator1 = enumerator1;
        _enumerator2 = enumerator2;
        
        _preferredEnumerator = _enumerator2.ComponentCount < _enumerator1.ComponentCount ? 1 : 0; 
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1) || typeof(TComponent) == typeof(T2);
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
            return ref Unsafe.As<T1, TComponent>(ref _enumerator1.Current);

        if (typeof(TComponent) == typeof(T2))
            return ref Unsafe.As<T2, TComponent>(ref _enumerator2.Current);

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    public ViewEnumerator<View<T1, T2>> GetEnumerator() => new(this);

    /// <inheritdoc />
    EntityReference IViewEnumerator.Current => new(_entityRegistry, _current);

    /// <inheritdoc />
    bool IViewEnumerator.MoveNext()
    {
        if (_preferredEnumerator == 0)
        {
            while (_enumerator1.MoveNext())
            {
                var entityId = _enumerator1.CurrentEntityId;
                if (!_enumerator2.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        else
        {
            while (_enumerator2.MoveNext())
            {
                var entityId = _enumerator2.CurrentEntityId;
                if (!_enumerator1.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        
        _current = EntityId.Null;
        return false;
    }

    /// <inheritdoc />
    bool IViewEnumerator.MoveTo(EntityId entityId)
    {
        return _enumerator1.MoveTo(entityId) & _enumerator2.MoveTo(entityId);
    }
}
