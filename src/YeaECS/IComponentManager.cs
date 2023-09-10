namespace YeaECS;

internal interface IComponentManager
{
    Type ComponentType { get; }

    object GetComponentBoxed(Entity entity);

    bool TryGetComponentBoxed(Entity entity, out object? component);

    bool HasComponent(Entity entity);

    void RemoveComponent(Entity entity);
}
