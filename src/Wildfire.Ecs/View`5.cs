namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public struct View<T1, T2, T3, T4, T5> : IView, IViewEnumerator
    where T1 : struct
    where T2 : struct
    where T3 : struct
    where T4 : struct
    where T5 : struct
{
    private readonly EntityRegistry _entityRegistry;
    private readonly int _preferredEnumerator;

    private EntityId _current;
    private ComponentManager<T1>.Enumerator _enumerator1;
    private ComponentManager<T2>.Enumerator _enumerator2;
    private ComponentManager<T3>.Enumerator _enumerator3;
    private ComponentManager<T4>.Enumerator _enumerator4;
    private ComponentManager<T5>.Enumerator _enumerator5;

    internal View(
        EntityRegistry entityRegistry,
        ComponentManager<T1>.Enumerator enumerator1,
        ComponentManager<T2>.Enumerator enumerator2,
        ComponentManager<T3>.Enumerator enumerator3,
        ComponentManager<T4>.Enumerator enumerator4,
        ComponentManager<T5>.Enumerator enumerator5)
    {
        _entityRegistry = entityRegistry;
        _current = EntityId.Null;
        _enumerator1 = enumerator1;
        _enumerator2 = enumerator2;
        _enumerator3 = enumerator3;
        _enumerator4 = enumerator4;
        _enumerator5 = enumerator5;

        _preferredEnumerator = DeterminedPreferredEnumerator(
            enumerator1.ComponentCount,
            enumerator2.ComponentCount,
            enumerator3.ComponentCount,
            enumerator4.ComponentCount,
            enumerator5.ComponentCount);
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1) || typeof(TComponent) == typeof(T2) || typeof(TComponent) == typeof(T3) || typeof(TComponent) == typeof(T4) || typeof(TComponent) == typeof(T5);
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

        if (typeof(TComponent) == typeof(T3))
            return ref Unsafe.As<T3, TComponent>(ref _enumerator3.Current);

        if (typeof(TComponent) == typeof(T4))
            return ref Unsafe.As<T4, TComponent>(ref _enumerator4.Current);

        if (typeof(TComponent) == typeof(T5))
            return ref Unsafe.As<T5, TComponent>(ref _enumerator5.Current);

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    public ViewEnumerator<View<T1, T2, T3, T4, T5>> GetEnumerator()
    {
        return new ViewEnumerator<View<T1, T2, T3, T4, T5>>(ref this);
    }

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
                if (!_enumerator2.MoveTo(entityId) || !_enumerator3.MoveTo(entityId) || !_enumerator4.MoveTo(entityId) || !_enumerator5.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        else if (_preferredEnumerator == 1)
        {
            while (_enumerator2.MoveNext())
            {
                var entityId = _enumerator2.CurrentEntityId;
                if (!_enumerator1.MoveTo(entityId) || !_enumerator3.MoveTo(entityId) || !_enumerator4.MoveTo(entityId) || !_enumerator5.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        else if (_preferredEnumerator == 2)
        {
            while (_enumerator3.MoveNext())
            {
                var entityId = _enumerator3.CurrentEntityId;
                if (!_enumerator1.MoveTo(entityId) || !_enumerator2.MoveTo(entityId) || !_enumerator4.MoveTo(entityId) || !_enumerator5.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        else if (_preferredEnumerator == 3)
        {
            while (_enumerator4.MoveNext())
            {
                var entityId = _enumerator4.CurrentEntityId;
                if (!_enumerator1.MoveTo(entityId) || !_enumerator2.MoveTo(entityId) || !_enumerator3.MoveTo(entityId) || !_enumerator5.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }
        else
        {
            while (_enumerator5.MoveNext())
            {
                var entityId = _enumerator5.CurrentEntityId;
                if (!_enumerator1.MoveTo(entityId) || !_enumerator2.MoveTo(entityId) || !_enumerator3.MoveTo(entityId) || !_enumerator4.MoveTo(entityId))
                    continue;

                _current = entityId;
                return true;
            }
        }

        _current = EntityId.Null;
        return false;
    }

    private static int DeterminedPreferredEnumerator(int componentCount1, int componentCount2, int componentCount3, int componentCount4, int componentCount5)
    {
        var min = Math.Min(componentCount1, Math.Min(componentCount2, Math.Min(componentCount3, Math.Min(componentCount4, componentCount5))));
        if (min == componentCount1)
            return 0;

        if (min == componentCount2)
            return 1;

        if (min == componentCount3)
            return 2;

        return min == componentCount4
            ? 3
            : 4;
    }

    /// <inheritdoc />
    bool IViewEnumerator.MoveTo(EntityId entityId)
    {
        return _enumerator1.MoveTo(entityId) & _enumerator2.MoveTo(entityId) & _enumerator3.MoveTo(entityId) & _enumerator4.MoveTo(entityId) & _enumerator5.MoveTo(entityId);
    }
}
