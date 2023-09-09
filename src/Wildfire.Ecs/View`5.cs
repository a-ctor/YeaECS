namespace Wildfire.Ecs;

public class View<T1, T2, T3, T4, T5>
    where T1 : struct
    where T2 : struct
    where T3 : struct
    where T4 : struct
    where T5 : struct
{
    private static bool T12Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T13Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T14Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager4.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T15Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager5.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T21Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T23Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T24Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager4.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T25Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager5.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T31Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T32Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager4.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T34Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager4.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T35Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager5.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T41Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T42Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T43Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager5.HasComponent(entity);
    }

    private static bool T45Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager5.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity);
    }

    private static bool T51Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T52Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T53Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager4.HasComponent(entity);
    }

    private static bool T54Filter(View<T1, T2, T3, T4, T5> view, Entity entity)
    {
        return view._componentManager4.HasComponent(entity)
               && view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity);
    }

    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<T1> _componentManager1;
    private readonly ComponentManager<T2> _componentManager2;
    private readonly ComponentManager<T3> _componentManager3;
    private readonly ComponentManager<T4> _componentManager4;
    private readonly ComponentManager<T5> _componentManager5;

    internal View(
        EntityRegistry entityRegistry,
        ComponentManager<T1> componentManager1,
        ComponentManager<T2> componentManager2,
        ComponentManager<T3> componentManager3,
        ComponentManager<T4> componentManager4,
        ComponentManager<T5> componentManager5)
    {
        _entityRegistry = entityRegistry;
        _componentManager1 = componentManager1;
        _componentManager2 = componentManager2;
        _componentManager3 = componentManager3;
        _componentManager4 = componentManager4;
        _componentManager5 = componentManager5;
    }

    public unsafe ViewEnumerator<View<T1, T2, T3, T4, T5>> GetEnumerator()
    {
        var componentCounts = stackalloc int[5];
        componentCounts[0] = _componentManager1.ComponentCount;
        componentCounts[1] = _componentManager2.ComponentCount;
        componentCounts[2] = _componentManager3.ComponentCount;
        componentCounts[3] = _componentManager4.ComponentCount;
        componentCounts[4] = _componentManager5.ComponentCount;

        var (m1, m2) = MinimumUtil.GetTwoMinimaIndices(componentCounts, 5);
        return (m1 + 1, m2 + 1) switch
        {
            (1, 2) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T12Filter, _componentManager1.GetEnumerator()),
            (1, 3) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T13Filter, _componentManager1.GetEnumerator()),
            (1, 4) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T14Filter, _componentManager1.GetEnumerator()),
            (1, 5) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T15Filter, _componentManager1.GetEnumerator()),
            (2, 1) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T21Filter, _componentManager2.GetEnumerator()),
            (2, 3) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T23Filter, _componentManager2.GetEnumerator()),
            (2, 4) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T24Filter, _componentManager2.GetEnumerator()),
            (2, 5) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T25Filter, _componentManager2.GetEnumerator()),
            (3, 1) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T31Filter, _componentManager3.GetEnumerator()),
            (3, 2) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T32Filter, _componentManager3.GetEnumerator()),
            (3, 4) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T34Filter, _componentManager3.GetEnumerator()),
            (3, 5) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T35Filter, _componentManager3.GetEnumerator()),
            (4, 1) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T41Filter, _componentManager4.GetEnumerator()),
            (4, 2) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T42Filter, _componentManager4.GetEnumerator()),
            (4, 3) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T43Filter, _componentManager4.GetEnumerator()),
            (4, 5) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T45Filter, _componentManager4.GetEnumerator()),
            (5, 1) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T51Filter, _componentManager5.GetEnumerator()),
            (5, 2) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T52Filter, _componentManager5.GetEnumerator()),
            (5, 3) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T53Filter, _componentManager5.GetEnumerator()),
            (5, 4) => new ViewEnumerator<View<T1, T2, T3, T4, T5>>(_entityRegistry, this, &T54Filter, _componentManager5.GetEnumerator()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
