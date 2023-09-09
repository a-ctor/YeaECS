namespace Wildfire.Ecs.UnitTests;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Moq;
using Xunit;

public class EntityRegistryTest
{
    [Fact]
    public void Constructor()
    {
        var entityRegistry = new EntityRegistry(3);

        Assert.Equal(3, entityRegistry.Capacity);
        Assert.Equal(0, entityRegistry.EntityCount);
    }

    [Fact]
    public void CreateEntity()
    {
        var entityRegistry = new EntityRegistry(3);

        var entity = entityRegistry.CreateEntity();
        Assert.True(entity.Exists);
        Assert.Same(entityRegistry, entity.EntityRegistry);
    }

    [Fact]
    public void CreateEntity_WithFullCapacity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);
        entityRegistry.CreateEntity();
        entityRegistry.CreateEntity();
        entityRegistry.CreateEntity();

        Assert.Throws<InvalidOperationException>(() => entityRegistry.CreateEntity());
    }

    [Fact]
    public void HasEntity()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        Assert.True(entity.Exists);
    }

    [Fact]
    public void HasEntity_WithNonExistingEntity_ReturnsFalse()
    {
        var entityRegistry = new EntityRegistry(3);

        Assert.False(entityRegistry.HasEntity(new Entity(1, 1)));
    }

    [Fact]
    public void DestroyEntity()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();
        Assert.Equal(1, entityRegistry.EntityCount);

        entity.Destroy();
        Assert.Equal(0, entityRegistry.EntityCount);
    }

    [Fact]
    public void DestroyEntity_WithNonExistingEntity_DoesNothing()
    {
        var entityRegistry = new EntityRegistry(3);
        entityRegistry.CreateEntity();
        Assert.Equal(1, entityRegistry.EntityCount);

        entityRegistry.DestroyEntity(new Entity(1, 2));
        Assert.Equal(1, entityRegistry.EntityCount);
    }

    [Fact]
    public void AddComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);
    }

    [Fact]
    public void AddComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        var testComponent = new TestComponent(3);
        Assert.Throws<InvalidOperationException>(() => entityRegistry.AddComponent(new Entity(1, 1), in testComponent));
    }

    [Fact]
    public void HasComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);

        Assert.True(entity.HasComponent<TestComponent>());
    }

    [Fact]
    public void HasComponent_WithNonExistingComponent_ReturnsFalse()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        Assert.False(entity.HasComponent<TestComponent>());
    }

    [Fact]
    public void HasComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        Assert.Throws<InvalidOperationException>(() => entityRegistry.HasComponent<TestComponent>(new Entity(1, 2)));
    }

    [Fact]
    public void GetComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);

        var component = entity.GetComponent<TestComponent>();
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void GetComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        var ex = Assert.Throws<InvalidOperationException>(() => entityRegistry.GetComponent<TestComponent>(new Entity(1, 2)));
        Assert.Equal("The specified entity does not exist.", ex.Message);
    }

    [Fact]
    public void GetComponent_WithNonExistingComponent_Throws()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var ex = Assert.Throws<InvalidOperationException>(() => entity.GetComponent<TestComponent>());
        Assert.Equal("Could not find a component 'Wildfire.Ecs.UnitTests.TestComponent' for entity <0@1>.", ex.Message);
    }

    [Fact]
    public void GetOrAddComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);

        var component = entity.GetOrAddComponent<TestComponent>();
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void GetOrAddComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        var ex = Assert.Throws<InvalidOperationException>(() => entityRegistry.GetOrAddComponent<TestComponent>(new Entity(1, 2)));
        Assert.Equal("The specified entity does not exist.", ex.Message);
    }

    [Fact]
    public void GetOrAddComponent_WithNonExistingComponent_AddsDefaultComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        ref var component = ref entity.GetOrAddComponent<TestComponent>();
        Assert.Equal(0, component.Value);

        component.Value = 13;

        Assert.Equal(13, entity.GetComponent<TestComponent>().Value);
    }

    [Fact]
    public void GetComponentAccessor()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        entity.AddComponent(new TestComponent(3));

        var accessor = entityRegistry.GetComponentAccessor<TestComponent>();

        Assert.True(accessor.HasComponent(entity));
        Assert.Equal(3, accessor.GetComponent(entity).Value);
        Assert.Equal(3, accessor.TryGetComponent(entity, out var success).Value);
        Assert.True(success);

        var results = new List<int>();
        var view = accessor.GetView();
        foreach (var entityRef in view)
            results.Add(entityRef.GetComponent<TestComponent>().Value);
        Assert.Single(results, 3);
    }

    [Fact]
    public void GetComponentAccessor_WithNonExistingComponent_ReturnsFalseOrThrows()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        entity.AddComponent(new TestComponent(3));
        entity.RemoveComponent<TestComponent>();

        var accessor = entityRegistry.GetComponentAccessor<TestComponent>();

        Assert.False(accessor.HasComponent(entity));
        Assert.Throws<InvalidOperationException>(() => entityRegistry.GetComponentAccessor<TestComponent>().GetComponent(entity).Value);
        Assert.True(Unsafe.IsNullRef(ref accessor.TryGetComponent(entity, out var success)));
        Assert.False(success);

        var results = new List<int>();
        var view = accessor.GetView();
        foreach (var entityRef in view)
            results.Add(entityRef.GetComponent<TestComponent>().Value);
        Assert.Empty(results);
    }

    [Fact]
    public void GetComponentAccessor_WithNonExistingComponentManager_BehavesLikeNonExistingComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var accessor = entityRegistry.GetComponentAccessor<TestComponent>();

        Assert.False(accessor.HasComponent(entity));
        Assert.Throws<InvalidOperationException>(() => entityRegistry.GetComponentAccessor<TestComponent>().GetComponent(entity).Value);
        Assert.True(Unsafe.IsNullRef(ref accessor.TryGetComponent(entity, out var success).Value));
        Assert.False(success);

        var results = new List<int>();
        var view = accessor.GetView();
        foreach (var entityRef in view)
            results.Add(entityRef.GetComponent<TestComponent>().Value);
        Assert.Empty(results);
    }

    [Fact]
    public void TryGetComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);

        var component = entity.TryGetComponent<TestComponent>(out var success);
        Assert.True(success);
        Assert.Equal(3, component.Value);
    }

    [Fact]
    public void TryGetComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        var ex = Assert.Throws<InvalidOperationException>(() => entityRegistry.TryGetComponent<TestComponent>(new Entity(1, 2), out var success));
        Assert.Equal("The specified entity does not exist.", ex.Message);
    }

    [Fact]
    public void TryGetComponent_WithNonExistingComponent_ReturnsFalse()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var component = entity.TryGetComponent<TestComponent>(out var success);
        Assert.False(success);
        Assert.Equal(0, component.Value);
    }

    [Fact]
    public void RemoveComponent()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        var testComponent = new TestComponent(3);
        entity.AddComponent(in testComponent);
        Assert.True(entity.HasComponent<TestComponent>());

        entity.RemoveComponent<TestComponent>();
        Assert.False(entity.HasComponent<TestComponent>());
    }

    [Fact]
    public void RemoveComponent_WithNonExistingComponent_DoesNothing()
    {
        var entityRegistry = new EntityRegistry(3);
        var entity = entityRegistry.CreateEntity();

        entity.RemoveComponent<TestComponent>();
    }

    [Fact]
    public void RemoveComponent_WithNonExistingEntity_Throws()
    {
        var entityRegistry = new EntityRegistry(3);

        var ex = Assert.Throws<InvalidOperationException>(() => entityRegistry.RemoveComponent<TestComponent>(new Entity(1, 2)));
        Assert.Equal("The specified entity does not exist.", ex.Message);
    }

    [Fact]
    public void OnEntityCreating()
    {
        var entityRegistry = new EntityRegistry(3);

        var entityCreatedEventHandlerMock = new Mock<EntityCreatedEventHandler>();
        entityCreatedEventHandlerMock.Setup(e => e(new EntityReference(entityRegistry, new Entity(1, 0))));
        entityRegistry.OnEntityCreated += entityCreatedEventHandlerMock.Object;

        var entityCreatingEventHandlerMock = new Mock<EntityCreatingEventHandler>();
        entityCreatingEventHandlerMock.Setup(e => e(new EntityReference(entityRegistry, new Entity(1, 0))));
        entityRegistry.OnEntityCreating += entityCreatingEventHandlerMock.Object;

        var builder = entityRegistry.BuildEntity();
        entityCreatingEventHandlerMock.Verify(e => e(new EntityReference(entityRegistry, new Entity(1, 0))));
        entityCreatedEventHandlerMock.Verify(e => e(new EntityReference(entityRegistry, new Entity(1, 0))), Times.Never());
        
        builder.Dispose();
        entityCreatedEventHandlerMock.Verify(e => e(new EntityReference(entityRegistry, new Entity(1, 0))));

        entityCreatingEventHandlerMock.VerifyAll();
    }

    [Fact]
    public void OnEntityCreated()
    {
        var entityRegistry = new EntityRegistry(3);

        var entityCreatedEventHandlerMock = new Mock<EntityCreatedEventHandler>();
        entityCreatedEventHandlerMock.Setup(e => e(new EntityReference(entityRegistry, new Entity(1, 0))));
        entityRegistry.OnEntityCreated += entityCreatedEventHandlerMock.Object;

        entityRegistry.CreateEntity();

        entityCreatedEventHandlerMock.VerifyAll();
    }

    [Fact]
    public void OnEntityDeleting()
    {
        var entityRegistry = new EntityRegistry(3);

        var entity = entityRegistry.CreateEntity();
        entity.AddComponent(new TestComponent(1337));

        var called = 0;
        EntityDeletingEventHandler handler = reference =>
        {
            Assert.Same(entityRegistry, reference.EntityRegistry);
            Assert.Equal(entity.Entity, reference.Entity);
            Assert.Equal(1337, reference.GetComponent<TestComponent>().Value);
            called++;
        };
        entityRegistry.OnEntityDeleting += handler;

        entity.Destroy();
        Assert.Equal(1, called);
    }

    [Fact]
    public void OnEntityDeleted()
    {
        var entityRegistry = new EntityRegistry(3);

        var entity = entityRegistry.CreateEntity();
        entity.AddComponent(new TestComponent(1377));

        var called = 0;
        EntityDeletedEventHandler handler = deletedEntity =>
        {
            Assert.Equal(entity.Entity, deletedEntity);
            Assert.False(entityRegistry.HasEntity(deletedEntity));
            called++;
        };
        entityRegistry.OnEntityDeleted += handler;

        entity.Destroy();
        Assert.Equal(1, called);
    }
}
