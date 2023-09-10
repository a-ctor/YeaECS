namespace YeaECS;

public class View<T1, T2, T3>
{
    private static bool T12Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager3.HasComponent(entity);
    }

    private static bool T13Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager2.HasComponent(entity);
    }

    private static bool T21Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager3.HasComponent(entity);
    }

    private static bool T23Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager3.HasComponent(entity)
               && view._componentManager1.HasComponent(entity);
    }

    private static bool T31Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity)
               && view._componentManager2.HasComponent(entity);
    }

    private static bool T32Filter(View<T1, T2, T3> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity)
               && view._componentManager1.HasComponent(entity);
    }

    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<T1> _componentManager1;
    private readonly ComponentManager<T2> _componentManager2;
    private readonly ComponentManager<T3> _componentManager3;

    internal View(
        EntityRegistry entityRegistry,
        ComponentManager<T1> componentManager1,
        ComponentManager<T2> componentManager2,
        ComponentManager<T3> componentManager3)
    {
        _entityRegistry = entityRegistry;
        _componentManager1 = componentManager1;
        _componentManager2 = componentManager2;
        _componentManager3 = componentManager3;
    }

    public unsafe ViewEnumerator<View<T1, T2, T3>> GetEnumerator()
    {
        var componentCount1 = _componentManager1.ComponentCount;
        var componentCount2 = _componentManager2.ComponentCount;
        var componentCount3 = _componentManager3.ComponentCount;

        var minComponentCount = Math.Min(Math.Min(componentCount1, componentCount2), componentCount3);
        if (componentCount1 == minComponentCount)
        {
            return componentCount2 < componentCount3
                ? new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T12Filter, _componentManager1.GetEnumerator())
                : new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T13Filter, _componentManager1.GetEnumerator());
        }

        if (componentCount2 == minComponentCount)
        {
            return componentCount1 < componentCount3
                ? new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T21Filter, _componentManager2.GetEnumerator())
                : new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T23Filter, _componentManager2.GetEnumerator());
        }

        return componentCount1 < componentCount2
            ? new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T31Filter, _componentManager3.GetEnumerator())
            : new ViewEnumerator<View<T1, T2, T3>>(_entityRegistry, this, &T32Filter, _componentManager3.GetEnumerator());
    }
}
