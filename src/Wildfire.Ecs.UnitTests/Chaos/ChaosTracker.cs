namespace Wildfire.Ecs.UnitTests.Chaos;

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit.Sdk;

public class ChaosTracker
{
    public class Ref<T>
    {
        public T Value;

        public Ref(T value)
        {
            Value = value;
        }
    }
    
    private readonly Dictionary<Type, object> _components = new();
    private readonly List<Entity> _entities = new();

    public IReadOnlyList<Entity> Entities => _entities;

    public void AddEntity(Entity entity)
    {
        if (_entities.Contains(entity))
            throw new InvalidOperationException($"The entity {entity} has already been added.");

        _entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }
    
    public void AddComponent<T>(Entity entity, T component)
    {
        GetComponentDict<T>().Add(entity, new Ref<T>(component));
    }

    public IEnumerable<Ref<T>> GetAllComponents<T>()
    {
        return GetComponentDict<T>().Values;
    }

    public Ref<T> GetComponent<T>(Entity entity)
    {
        if (!GetComponentDict<T>().TryGetValue(entity, out var result))
            throw new InvalidOperationException($"The specified entity {entity} does not have a component '{typeof(T)}'.");

        return result;
    }

    public void RemoveComponent<T>(Entity entity)
    {
        GetComponentDict<T>().Remove(entity);
    }

    public void RemoveAllComponents(Entity entity)
    {
        foreach (var value in _components.Values)
            ((IDictionary)value).Remove(entity);
    }

    private Dictionary<Entity, Ref<T>> GetComponentDict<T>()
    {
        if (_components.TryGetValue(typeof(T), out var obj))
            return (Dictionary<Entity, Ref<T>>)obj;

        var result = new Dictionary<Entity, Ref<T>>();
        _components.Add(typeof(T), result);

        return result;
    }
}
