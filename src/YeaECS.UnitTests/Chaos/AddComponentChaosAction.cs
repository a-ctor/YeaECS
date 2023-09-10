namespace YeaECS.UnitTests.Chaos;

public class AddComponentChaosAction<TComponent> : IChaosAction
{
    private readonly Entity _entity;
    private readonly TComponent _component;

    public AddComponentChaosAction(Entity entity, TComponent component)
    {
        _entity = entity;
        _component = component;
    }
    
    public void Record(ChaosTracker tracker)
    {
        tracker.AddComponent(_entity, _component);
    }

    public void Apply(EntityRegistry entityRegistry)
    {
        entityRegistry.AddComponent(_entity, _component);
    }
}
