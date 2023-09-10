namespace YeaECS.UnitTests.Chaos;

public class AddEntityChaosAction : IChaosAction<Entity>
{
    public AddEntityChaosAction()
    {
    }
    
    public void Record(ChaosTracker tracker, Entity entity)
    {
        tracker.AddEntity(entity);
    }

    public Entity Apply(EntityRegistry entityRegistry)
    {
        return entityRegistry.CreateEntity().Entity;
    }
}
