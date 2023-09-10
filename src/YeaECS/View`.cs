namespace YeaECS;

public class View<T1>
{
    private static bool TrueFilter(View<T1> view, Entity entity) => true;
    
    private readonly EntityRegistry _entityRegistry;
    private readonly ComponentManager<T1> _componentManager;

    internal View(EntityRegistry entityRegistry, ComponentManager<T1> componentManager)
    {
        _entityRegistry = entityRegistry;
        _componentManager = componentManager;
    }

    public unsafe ViewEnumerator<View<T1>> GetEnumerator() => new(_entityRegistry, this, &TrueFilter, _componentManager.GetEnumerator());
}
