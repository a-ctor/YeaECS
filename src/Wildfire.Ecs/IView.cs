namespace Wildfire.Ecs;

public interface IView
{
    /// <summary>
    /// Checks if the view supports the specified <typeparamref name="TComponent"/>.
    /// If it supports a component it can be retrieved using <see cref="Get{TComponent}"/>.
    /// </summary>
    bool Supports<TComponent>()
        where TComponent : struct;

    /// <summary>
    /// Checks if the current entity in the view has the specified <typeparamref name="TComponent"/>.
    /// </summary>
    bool Has<TComponent>()
        where TComponent : struct;

    /// <summary>
    /// Returns the specified <typeparamref name="TComponent"/> for the current entity in the view.
    /// Throws if the specified <typeparamref name="TComponent"/> is not supported by this view.
    /// </summary>
    ref TComponent Get<TComponent>()
        where TComponent : struct;
}
