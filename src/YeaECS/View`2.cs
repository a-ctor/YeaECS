namespace YeaECS;

public class View<T1, T2>
{
    private static bool T21Filter(View<T1, T2> view, Entity entity)
    {
        return view._componentManager1.HasComponent(entity);
    }

    private static bool T12Filter(View<T1, T2> view, Entity entity)
    {
        return view._componentManager2.HasComponent(entity);
    }

    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<T1> _componentManager1;
    private readonly ComponentManager<T2> _componentManager2;

    internal View(
        EntityRegistry entityRegistry,
        ComponentManager<T1> componentManager1,
        ComponentManager<T2> componentManager2)
    {
        _entityRegistry = entityRegistry;
        _componentManager1 = componentManager1;
        _componentManager2 = componentManager2;
    }

    public unsafe ViewEnumerator<View<T1, T2>> GetEnumerator()
    {
        return _componentManager1.ComponentCount < _componentManager2.ComponentCount
            ? new ViewEnumerator<View<T1, T2>>(_entityRegistry, this, &T12Filter, _componentManager1.GetEnumerator())
            : new ViewEnumerator<View<T1, T2>>(_entityRegistry, this, &T21Filter, _componentManager2.GetEnumerator());
    }
}
