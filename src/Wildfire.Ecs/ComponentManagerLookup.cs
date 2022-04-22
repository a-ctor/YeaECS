namespace Wildfire.Ecs;

internal readonly struct ComponentManagerLookup
{
    private readonly int _capacity;
    private readonly Dictionary<Type, IComponentManager> _componentManagers = new();

    public IEnumerable<IComponentManager> Values => _componentManagers.Values;
    
    public ComponentManagerLookup(int capacity)
    {
        _capacity = capacity;
    }

    public ComponentManager<T> GetOrAdd<T>()
        where T : struct
    {
        if (_componentManagers.TryGetValue(typeof(T), out var result))
            return (ComponentManager<T>)result;

        var componentManager = new ComponentManager<T>(_capacity);
        _componentManagers[typeof(T)] = componentManager;
        return componentManager;
    }

    public bool TryGet<T>(out ComponentManager<T> componentManager)
        where T : struct
    {
        var success = _componentManagers.TryGetValue(typeof(T), out var result);
        componentManager = (ComponentManager<T>)result!;
        return success;
    }
}
