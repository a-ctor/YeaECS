namespace Wildfire.Ecs;

public class RenderSystemCollection
{
    private readonly List<IRenderSystem> _systems = new();

    public RenderSystemCollection()
    {
    }

    public void Add(IRenderSystem system) => _systems.Add(system);

    public void Remove(IRenderSystem system) => _systems.Remove(system);

    public void Draw(EntityRegistry entityRegistry)
    {
        for (var i = 0; i < _systems.Count; i++)
            _systems[i].Draw(entityRegistry);
    }
}
