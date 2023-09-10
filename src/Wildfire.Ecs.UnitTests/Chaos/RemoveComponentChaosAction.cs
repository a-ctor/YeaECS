namespace Wildfire.Ecs.UnitTests.Chaos;

public class RemoveComponentChaosAction<TComponent> : IChaosAction
{
    private readonly Entity _entity;

    public RemoveComponentChaosAction(Entity entity)
    {
        _entity = entity;
    }
    
    public void Record(ChaosTracker tracker)
    {
        tracker.RemoveComponent<TComponent>(_entity);
    }

    public void Apply(EntityRegistry entityRegistry)
    {
        entityRegistry.RemoveComponent<TComponent>(_entity);
    }
}
