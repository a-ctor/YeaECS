namespace Wildfire.Ecs;

public interface ISystem
{
    void Update(EntityRegistry entityRegistry);
}
