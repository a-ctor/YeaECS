namespace Wildfire.Ecs;

public readonly ref struct ViewEnumerator<TView>
    where TView : class, IViewEnumerator
{
    private readonly TView _view;

    public ViewEnumerator(TView view)
    {
        if (view == null)
            throw new ArgumentNullException(nameof(view));

        _view = view;
    }

    public EntityReference Current => _view.Current;

    public bool MoveNext()
    {
        return _view.MoveNext();
    }

    public bool MoveTo(Entity entity)
    {
        return _view.MoveTo(entity);
    }
}
