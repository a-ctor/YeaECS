namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public readonly unsafe ref struct ViewEnumerator<TView>
    where TView : struct, IViewEnumerator
{
    private readonly void* _view;

    public ViewEnumerator(ref TView view)
    {
        // This operation is only safe for stack values so we assume that people use views correctly
        _view = Unsafe.AsPointer(ref view);
    }

    public EntityReference Current => _view != null
        ? Unsafe.AsRef<TView>(_view).Current
        : default;

    public bool MoveNext()
    {
        return _view != null && Unsafe.AsRef<TView>(_view).MoveNext();
    }

    public bool MoveTo(EntityId entityId)
    {
        return _view != null && Unsafe.AsRef<TView>(_view).MoveTo(entityId);
    }
}
