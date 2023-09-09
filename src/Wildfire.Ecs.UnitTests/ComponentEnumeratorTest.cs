namespace Wildfire.Ecs.UnitTests;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class ComponentEnumeratorTest
{
    private record struct EntityDescription(Entity Entity, bool HasComponent1, bool HasComponent2, bool HasComponent3, bool HasComponent4, bool HasComponent5);

    private record struct Component1();

    private record struct Component2();

    private record struct Component3();

    private record struct Component4();

    private record struct Component5();

    private const int c_entityCount = 10000;
    private const int c_randomSeed = 1337; // Was chosen randomly

    private readonly EntityRegistry _entityRegistry;

    private readonly List<EntityDescription> _entityDescriptions = new();

    public ComponentEnumeratorTest()
    {
        _entityRegistry = new EntityRegistry(c_entityCount + 10);
        var random = new Random(c_randomSeed);

        for (var i = 0; i < c_entityCount; i++)
        {
            var entity = _entityRegistry.CreateEntity();

            var hasComponent1 = random.Next(0, 2) == 0;
            if (hasComponent1)
                entity.AddComponent(new Component1());

            var hasComponent2 = random.Next(0, 2) == 0;
            if (hasComponent2)
                entity.AddComponent(new Component2());

            var hasComponent3 = random.Next(0, 2) == 0;
            if (hasComponent3)
                entity.AddComponent(new Component3());

            var hasComponent4 = random.Next(0, 2) == 0;
            if (hasComponent4)
                entity.AddComponent(new Component4());

            var hasComponent5 = random.Next(0, 2) == 0;
            if (hasComponent5)
                entity.AddComponent(new Component5());

            _entityDescriptions.Add(new EntityDescription(entity.Entity, hasComponent1, hasComponent2, hasComponent3, hasComponent4, hasComponent5));
        }
    }

    [Fact]
    public void EnumerateView_OneRequiredComponents()
    {
        var actualEntities = new List<Entity>();

        var view = _entityRegistry.ViewOf<Component1>();
        foreach (var entity in view)
        {
            Assert.Equal(entity.EntityRegistry, _entityRegistry);
            actualEntities.Add(entity.Entity);
        }

        var expectedEntityIds = _entityDescriptions
            .Where(e => e.HasComponent1)
            .Select(e => e.Entity)
            .ToArray();
        Assert.Equal(expectedEntityIds, actualEntities);
    }

    [Fact]
    public void EnumerateView_TwoRequiredComponents()
    {
        var actualEntities = new List<Entity>();

        var view = _entityRegistry.ViewOf<Component1, Component2>();
        foreach (var entity in view)
        {
            Assert.Equal(entity.EntityRegistry, _entityRegistry);
            actualEntities.Add(entity.Entity);
        }

        var expectedEntityIds = _entityDescriptions
            .Where(e => e.HasComponent1 && e.HasComponent2)
            .Select(e => e.Entity)
            .ToArray();
        Assert.Equal(expectedEntityIds, actualEntities);
    }

    [Fact]
    public void EnumerateView_ThreeRequiredComponents()
    {
        var actualEntities = new List<Entity>();

        var view = _entityRegistry.ViewOf<Component1, Component2, Component3>();
        foreach (var entity in view)
        {
            Assert.Equal(entity.EntityRegistry, _entityRegistry);
            actualEntities.Add(entity.Entity);
        }

        var expectedEntityIds = _entityDescriptions
            .Where(e => e.HasComponent1 && e.HasComponent2 && e.HasComponent3)
            .Select(e => e.Entity)
            .ToArray();
        Assert.Equal(expectedEntityIds, actualEntities);
    }

    [Fact]
    public void EnumerateView_FourRequiredComponents()
    {
        var actualEntities = new List<Entity>();

        var view = _entityRegistry.ViewOf<Component1, Component2, Component3, Component4>();
        foreach (var entity in view)
        {
            Assert.Equal(entity.EntityRegistry, _entityRegistry);
            actualEntities.Add(entity.Entity);
        }

        var expectedEntityIds = _entityDescriptions
            .Where(e => e.HasComponent1 && e.HasComponent2 && e.HasComponent3 && e.HasComponent4)
            .Select(e => e.Entity)
            .ToArray();
        Assert.Equal(expectedEntityIds, actualEntities);
    }

    [Fact]
    public void EnumerateView_FiveRequiredComponents()
    {
        var actualEntities = new List<Entity>();

        var view = _entityRegistry.ViewOf<Component1, Component2, Component3, Component4, Component5>();
        foreach (var entity in view)
        {
            Assert.Equal(entity.EntityRegistry, _entityRegistry);
            actualEntities.Add(entity.Entity);
        }

        var expectedEntityIds = _entityDescriptions
            .Where(e => e.HasComponent1 && e.HasComponent2 && e.HasComponent3 && e.HasComponent4 && e.HasComponent5)
            .Select(e => e.Entity)
            .ToArray();
        Assert.Equal(expectedEntityIds, actualEntities);
    }
}
