namespace Wildfire.Ecs.UnitTests.Chaos;

public class RemoveEntityChaosAction : IChaosAction
{
    private readonly Entity _entity;

    public RemoveEntityChaosAction(Entity entity)
    {
        _entity = entity;
    }
    
    public void Record(ChaosTracker tracker)
    {
        tracker.RemoveEntity(_entity);
        tracker.RemoveAllComponents(_entity);
    }

    public void Apply(EntityRegistry entityRegistry)
    {
        entityRegistry.DestroyEntity(_entity);
    }
}
