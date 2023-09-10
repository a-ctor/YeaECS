namespace YeaECS.UnitTests.Chaos;

public interface IChaosAction<TResult>
{
    void Record(ChaosTracker tracker, TResult result);

    TResult Apply(EntityRegistry entityRegistry);
}
