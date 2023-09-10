namespace YeaECS.UnitTests.Chaos;

public class ChaosTester
{
    public EntityRegistry Registry { get; }

    public ChaosTracker Tracker { get; } = new();

    public ChaosTester(int capacity)
    {
        Registry = new EntityRegistry(capacity);
    }

    public Entity AddEntity()
    {
        return ExecuteAction(new AddEntityChaosAction());
    }

    public void DestroyEntity(Entity entity)
    {
        ExecuteAction(new RemoveEntityChaosAction(entity));
    }
    
    public void AddComponent<TComponent>(Entity entity, TComponent component)
    {
        ExecuteAction(new AddComponentChaosAction<TComponent>(entity, component));
    }

    public void RemoveComponent<TComponent>(Entity entity)
    {
        ExecuteAction(new RemoveComponentChaosAction<TComponent>(entity));
    }

    private void ExecuteAction(IChaosAction action)
    {
        action.Apply(Registry);
        action.Record(Tracker);
    }

    private T ExecuteAction<T>(IChaosAction<T> action)
    {
        var result = action.Apply(Registry);
        action.Record(Tracker, result);
        return result;
    }
}
