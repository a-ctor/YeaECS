namespace Wildfire.Ecs.UnitTests;

using System;
using System.Collections.Generic;
using Xunit;

public class ComponentManagerTest
{
    [Fact]
    public void Constructor()
    {
        var componentManager = new ComponentManager<TestComponent>(3);

        Assert.Equal(3, componentManager.Capacity);
        Assert.Equal(0, componentManager.ComponentCount);
    }

    [Fact]
    public void AddComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        Assert.Equal(1, componentManager.ComponentCount);
    }

    [Fact]
    public void AddComponent_WithExistingComponent_Throws()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        Assert.Throws<InvalidOperationException>(() => componentManager.AddComponent(new Entity(3), in value));
    }

    [Fact]
    public void AddComponent_WithFullCapacity_Throws()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(1), in value);
        componentManager.AddComponent(new Entity(2), in value);
        componentManager.AddComponent(new Entity(3), in value);

        Assert.Throws<InvalidOperationException>(() => componentManager.AddComponent(new Entity(4), in value));
    }

    [Fact]
    public void HasComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        Assert.False(componentManager.HasComponent(new Entity(3)));

        componentManager.AddComponent(new Entity(3), in value);
        Assert.True(componentManager.HasComponent(new Entity(3)));
    }

    [Fact]
    public void GetComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        var component = componentManager.GetComponent(new Entity(3));
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void GetComponent_WithNonExistingComponent_Throws()
    {
        var componentManager = new ComponentManager<TestComponent>(3);

        Assert.Throws<InvalidOperationException>(() => componentManager.GetComponent(new Entity(3)));
    }

    [Fact]
    public void GetOrAddComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        var component = componentManager.GetOrAddComponent(new Entity(3));
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void GetOrAddComponent_WithNonExistingComponent_AddsDefaultComponent()
    {
        var componentManager = new ComponentManager<TestComponent>(3);

        ref var component = ref componentManager.GetOrAddComponent(new Entity(3));
        Assert.Equal(0, component.Value);

        component.Value = 13;

        Assert.Equal(13, componentManager.GetComponent(new Entity(3)).Value);
    }

    [Fact]
    public void GetComponent_ReturnsReference()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        ref var componentRef = ref componentManager.GetComponent(new Entity(3));
        componentRef.Value = 5;

        var component = componentManager.GetComponent(new Entity(3));
        Assert.Equal(5, component.Value);
    }

    [Fact]
    public void TryGetComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);

        var component = componentManager.TryGetComponent(new Entity(3), out var success);
        Assert.True(success);
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void TryGetComponent_WithNonExistingComponent_ReturnsFalse()
    {
        var componentManager = new ComponentManager<TestComponent>(3);

        var component = componentManager.TryGetComponent(new Entity(3), out var success);
        Assert.False(success);
        Assert.Equal(0, component.Value);
    }

    [Fact]
    public void RemoveComponent()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);
        Assert.Equal(1, componentManager.ComponentCount);

        componentManager.RemoveComponent(new Entity(3));
        Assert.Equal(0, componentManager.ComponentCount);
    }

    [Fact]
    public void RemoveComponent_WithNonExistingComponent_DoesNothing()
    {
        var value = new TestComponent(3);
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(3), in value);
        Assert.Equal(1, componentManager.ComponentCount);

        componentManager.RemoveComponent(new Entity(2));
        Assert.Equal(1, componentManager.ComponentCount);
    }

    [Fact]
    public void GetEnumerator()
    {
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(1), new TestComponent(1));
        componentManager.AddComponent(new Entity(2), new TestComponent(2));
        componentManager.AddComponent(new Entity(3), new TestComponent(3));

        foreach (ref var c in componentManager)
            c.Value += 10;

        Assert.Equal(11, componentManager.GetComponent(new Entity(1)).Value);
        Assert.Equal(12, componentManager.GetComponent(new Entity(2)).Value);
        Assert.Equal(13, componentManager.GetComponent(new Entity(3)).Value);
    }

    [Fact]
    public void GetEnumerator_WithManualEnumeration_ProvidesAccessToEntities()
    {
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(1), new TestComponent(11));
        componentManager.AddComponent(new Entity(2), new TestComponent(12));
        componentManager.AddComponent(new Entity(3), new TestComponent(13));

        var enumerator = componentManager.GetEnumerator();
        for (var i = 1; i <= 3; i++)
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(enumerator.Current.Value, i + 10);
            Assert.Equal(enumerator.CurrentEntity, new Entity((uint)i));
        }

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void GetEnumerator_WithSkipping()
    {
        var componentManager = new ComponentManager<TestComponent>(3);
        componentManager.AddComponent(new Entity(1), new TestComponent(11));
        componentManager.AddComponent(new Entity(2), new TestComponent(12));
        componentManager.AddComponent(new Entity(3), new TestComponent(13));

        var componentManager2 = new ComponentManager<TestComponent>(3);
        componentManager2.AddComponent(new Entity(1), new TestComponent(11));
        componentManager2.AddComponent(new Entity(3), new TestComponent(12));
        componentManager2.AddComponent(new Entity(4), new TestComponent(13));

        var result = new List<Entity>();

        var enumerator = componentManager.GetEnumerator();
        var enumerator2 = componentManager2.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (!enumerator2.MoveTo(enumerator.CurrentEntity))
                continue;

            Assert.Equal(enumerator.CurrentEntity, enumerator2.CurrentEntity);
            result.Add(enumerator.CurrentEntity);
        }

        Assert.Equal(result, new[] { new Entity(1), new Entity(3) });
    }
}
