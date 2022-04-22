namespace Wildfire.Ecs;

public class SystemCollection
{
    private readonly List<ISystem> _systems = new();

    public SystemCollection()
    {
    }

    public void Add(ISystem system) => _systems.Add(system);

    public void Remove(ISystem system) => _systems.Remove(system);

    public void Update(EntityRegistry entityRegistry)
    {
        for (var i = 0; i < _systems.Count; i++)
            _systems[i].Update(entityRegistry);
    }
}
