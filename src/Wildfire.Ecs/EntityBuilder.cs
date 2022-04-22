namespace Wildfire.Ecs;

using System.Runtime.CompilerServices;

public ref struct EntityBuilder
{
    private EntityRegistry.BuilderToken _token;
    
    public EntityReference Reference => new(_token.EntityRegistry, _token.Id);

    internal EntityBuilder(EntityRegistry.BuilderToken token)
    {
        _token = token;
    }

    /// <summary>
    /// Debug property to inspect components for this <see cref="EntityReference"/>.
    /// </summary>
    private object[] Components
    {
        get
        {
            var id = _token.Id;
            return _token.EntityRegistry.GetComponentManagers
                .Select(e => (e.TryGetComponentBoxed(id, out var component), component))
                .Where(e => e.Item1)
                .Select(e => e.component)
                .ToArray()!;
        }
    }

    public void Dispose()
    {
        _token.Dispose();
    }

    /// <summary>
    /// Adds the specified <paramref name="component"/> to the this entity.
    /// Throws if this entity does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<TComponent>(in TComponent component)
        where TComponent : struct
    {
        _token.EntityRegistry.AddComponent(_token.Id, in component);
    }
}
