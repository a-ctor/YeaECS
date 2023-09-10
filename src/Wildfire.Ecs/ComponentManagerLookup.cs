namespace Wildfire.Ecs;

internal struct ComponentManagerLookup
{
    // ReSharper disable once UnusedTypeParameter
    private static class ComponentIndex<TComponent>
    {
        public static readonly int Index = Interlocked.Increment(ref s_lastIndex);
    }

    private static int s_lastIndex = -1;

    private readonly int _capacity;
    private readonly List<IComponentManager> _componentManagers = new();

    private IComponentManager?[] _componentManagerLookup = new IComponentManager[64];

    public IEnumerable<IComponentManager> Values => _componentManagers;

    public ComponentManagerLookup(int capacity)
    {
        _capacity = capacity;
    }

    public ComponentManager<T> GetOrAdd<T>()
    {
        var index = ComponentIndex<T>.Index;
        var componentManager = index < _componentManagerLookup.Length
            ? (ComponentManager<T>?)_componentManagerLookup[index]
            : null;

        if (componentManager != null)
            return componentManager;

        componentManager = new ComponentManager<T>(_capacity);

        if (index >= _componentManagerLookup.Length)
            ResizeToFit(index);

        _componentManagerLookup[index] = componentManager;
        _componentManagers.Add(componentManager);

        return componentManager;
    }

    public bool TryGet<T>(out ComponentManager<T> componentManager)
    {
        var index = ComponentIndex<T>.Index;
        componentManager = (index < _componentManagerLookup.Length
            ? (ComponentManager<T>?)_componentManagerLookup[index]
            : null)!;

        return componentManager != null;
    }

    private void ResizeToFit(int index)
    {
        var size = _componentManagerLookup.Length;
        while (size <= index)
            size *= 2;
        
        Array.Resize(ref _componentManagerLookup, size);
    }
}
