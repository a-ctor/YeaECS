namespace Wildfire.Ecs;

internal interface IComponentManager
{
    Type ComponentType { get; }

    object GetComponentBoxed(EntityId id);

    bool TryGetComponentBoxed(EntityId id, out object? component);

    bool HasComponent(EntityId id);

    void RemoveComponent(EntityId id);
}
