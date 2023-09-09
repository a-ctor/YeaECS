namespace Wildfire.Ecs;

public class CombinedView<TView, TOptionalView> : IView, IViewEnumerator
    where TView : IView, IViewEnumerator
    where TOptionalView : IView, IOptionalViewEnumerator
{
    private TView _view;
    private TOptionalView _optionalView;

    public CombinedView(in TView view, in TOptionalView optionalView)
    {
        _view = view;
        _optionalView = optionalView;
    }

    /// <inheritdoc />
    public bool Supports<TComponent>()
        where TComponent : struct
    {
        return _view.Supports<TComponent>() || _optionalView.Supports<TComponent>();
    }

    /// <inheritdoc />
    public bool Has<TComponent>()
        where TComponent : struct
    {
        return _view.Has<TComponent>() || _optionalView.Has<TComponent>();
    }

    /// <inheritdoc />
    public ref TComponent Get<TComponent>()
        where TComponent : struct
    {
        if (_view.Supports<TComponent>())
            return ref _view.Get<TComponent>();

        if (_optionalView.Supports<TComponent>())
            return ref _optionalView.Get<TComponent>();

        throw new InvalidOperationException("The specified component is not part of the view.");
    }

    public ViewEnumerator<CombinedView<TView, TOptionalView>> GetEnumerator() => new(this);

    /// <inheritdoc />
    EntityReference IViewEnumerator.Current => _view.Current;

    /// <inheritdoc />
    bool IViewEnumerator.MoveNext()
    {
        var result = _view.MoveNext();
        if (!result)
        {
            _optionalView.MoveTo(EntityId.Null);
            return false;
        }

        _optionalView.MoveTo(_view.Current.Id);
        return true;
    }

    /// <inheritdoc />
    bool IViewEnumerator.MoveTo(EntityId entityId)
    {
        var result = _view.MoveTo(entityId);
        _optionalView.MoveTo(entityId);

        return result;
    }
}
