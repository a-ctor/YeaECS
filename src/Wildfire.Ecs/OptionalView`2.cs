namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public struct OptionalView<T1, T2> : IView, IOptionalViewEnumerator
    where T1 : struct
    where T2 : struct
{
    private bool _hasItem1;
    private ComponentManager<T1>.Enumerator _enumerator1;
    private bool _hasItem2;
    private ComponentManager<T2>.Enumerator _enumerator2;

    internal OptionalView(
        ComponentManager<T1>.Enumerator enumerator1,
        ComponentManager<T2>.Enumerator enumerator2)
    {
        _hasItem1 = false;
        _enumerator1 = enumerator1;
        _hasItem2 = false;
        _enumerator2 = enumerator2;
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1) || typeof(TComponent) == typeof(T2);
    }

    public bool Has<TComponent>()
        where TComponent : struct
    {
        if (typeof(TComponent) == typeof(T1))
            return _hasItem1;

        if (typeof(TComponent) == typeof(T2))
            return _hasItem2;

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

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    /// <inheritdoc />
    void IOptionalViewEnumerator.MoveTo(Entity entity)
    {
        _hasItem1 = _enumerator1.MoveTo(entity);
        _hasItem2 = _enumerator2.MoveTo(entity);
    }
}
