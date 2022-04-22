namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public static class CombinedView
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CombinedView<TView, TOptionalView> Create<TView, TOptionalView>(in TView view, in TOptionalView optionalView)
        where TView : IView, IViewEnumerator
        where TOptionalView : IView, IOptionalViewEnumerator
    {
        return new CombinedView<TView, TOptionalView>(
            in view,
            in optionalView);
    }
}
