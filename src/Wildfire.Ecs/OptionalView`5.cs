namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public struct OptionalView<T1, T2, T3, T4, T5> : IView, IOptionalViewEnumerator
    where T1 : struct
    where T2 : struct
    where T3 : struct
    where T4 : struct
    where T5 : struct
{
    private bool _hasItem1;
    private ComponentManager<T1>.Enumerator _enumerator1;
    private bool _hasItem2;
    private ComponentManager<T2>.Enumerator _enumerator2;
    private bool _hasItem3;
    private ComponentManager<T3>.Enumerator _enumerator3;
    private bool _hasItem4;
    private ComponentManager<T4>.Enumerator _enumerator4;
    private bool _hasItem5;
    private ComponentManager<T5>.Enumerator _enumerator5;

    internal OptionalView(
        ComponentManager<T1>.Enumerator enumerator1,
        ComponentManager<T2>.Enumerator enumerator2,
        ComponentManager<T3>.Enumerator enumerator3,
        ComponentManager<T4>.Enumerator enumerator4,
        ComponentManager<T5>.Enumerator enumerator5)
    {
        _hasItem1 = false;
        _enumerator1 = enumerator1;
        _hasItem2 = false;
        _enumerator2 = enumerator2;
        _hasItem3 = false;
        _enumerator3 = enumerator3;
        _hasItem4 = false;
        _enumerator4 = enumerator4;
        _hasItem5 = false;
        _enumerator5 = enumerator5;
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1) || typeof(TComponent) == typeof(T2) || typeof(TComponent) == typeof(T3) || typeof(TComponent) == typeof(T4) || typeof(TComponent) == typeof(T5);
    }

    public bool Has<TComponent>()
        where TComponent : struct
    {
        if (typeof(TComponent) == typeof(T1))
            return _hasItem1;

        if (typeof(TComponent) == typeof(T2))
            return _hasItem2;

        if (typeof(TComponent) == typeof(T3))
            return _hasItem3;

        if (typeof(TComponent) == typeof(T4))
            return _hasItem4;

        if (typeof(TComponent) == typeof(T5))
            return _hasItem5;

        return false;
    }

    /// <inheritdoc /> 
    public ref TComponent Get<TComponent>()
        where TComponent : struct
    {
        if (typeof(T1) == typeof(TComponent))
        {
            return ref _hasItem1
                ? ref Unsafe.As<T1, TComponent>(ref _enumerator1.Current)
                : ref RefDummy<TComponent>.Value;
        }

        if (typeof(T2) == typeof(TComponent))
        {
            return ref _hasItem2
                ? ref Unsafe.As<T2, TComponent>(ref _enumerator2.Current)
                : ref RefDummy<TComponent>.Value;
        }

        if (typeof(T3) == typeof(TComponent))
        {
            return ref _hasItem3
                ? ref Unsafe.As<T3, TComponent>(ref _enumerator3.Current)
                : ref RefDummy<TComponent>.Value;
        }

        if (typeof(T4) == typeof(TComponent))
        {
            return ref _hasItem4
                ? ref Unsafe.As<T4, TComponent>(ref _enumerator4.Current)
                : ref RefDummy<TComponent>.Value;
        }

        if (typeof(T5) == typeof(TComponent))
        {
            return ref _hasItem5
                ? ref Unsafe.As<T5, TComponent>(ref _enumerator5.Current)
                : ref RefDummy<TComponent>.Value;
        }

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    /// <inheritdoc />
    void IOptionalViewEnumerator.MoveTo(Entity entity)
    {
        _hasItem1 = _enumerator1.MoveTo(entity);
        _hasItem2 = _enumerator2.MoveTo(entity);
        _hasItem3 = _enumerator3.MoveTo(entity);
        _hasItem4 = _enumerator4.MoveTo(entity);
        _hasItem5 = _enumerator5.MoveTo(entity);
    }
}
