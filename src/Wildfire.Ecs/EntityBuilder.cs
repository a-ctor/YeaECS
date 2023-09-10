namespace Wildfire.Ecs;

using System.Diagnostics;
using System.Runtime.CompilerServices;

public ref struct EntityBuilder
{
    private EntityRegistry.BuilderToken _token;
    
    public EntityReference Reference => new(_token.EntityRegistry, _token.Entity);

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
            var entity = _token.Entity;
            return _token.EntityRegistry.GetComponentManagers()
                .Select(e => (e.TryGetComponentBoxed(entity, out var component), component))
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
    {
        _token.EntityRegistry.AddComponent(_token.Entity, in component);
    }
}
