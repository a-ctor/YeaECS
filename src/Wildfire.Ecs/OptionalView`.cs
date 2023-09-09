namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public struct OptionalView<T1> : IView, IOptionalViewEnumerator
    where T1 : struct
{
    private bool _hasItem1;
    private ComponentManager<T1>.Enumerator _enumerator;

    internal OptionalView(ComponentManager<T1>.Enumerator enumerator)
    {
        _hasItem1 = false;
        _enumerator = enumerator;
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1);
    }

    /// <inheritdoc />
    public bool Has<TComponent>()
        where TComponent : struct
    {
        return typeof(TComponent) == typeof(T1) && _hasItem1;
    }

    /// <inheritdoc />
    public ref TComponent Get<TComponent>()
        where TComponent : struct
    {
        if (typeof(T1) == typeof(TComponent))
        {
            return ref _hasItem1
                ? ref Unsafe.As<T1, TComponent>(ref _enumerator.Current)
                : ref RefDummy<TComponent>.Value;
        }

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    /// <inheritdoc />
    void IOptionalViewEnumerator.MoveTo(Entity entity)
    {
        _hasItem1 = _enumerator.MoveTo(entity);
    }
}
