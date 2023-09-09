namespace Wildfire.Ecs.UnitTests;

using System;
using Xunit;

public class EntityManagerTest
{
    [Fact]
    public void Constructor()
    {
        var entityManager = new EntityManager(3);

        Assert.Equal(3, entityManager.Capacity);
        Assert.Equal(0, entityManager.EntityCount);
    }

    [Fact]
    public void CreateEntity()
    {
        var entityManager = new EntityManager(3);

        Assert.Equal("<1>", entityManager.CreateEntity().ToString());
        Assert.Equal("<2>", entityManager.CreateEntity().ToString());
        Assert.Equal("<3>", entityManager.CreateEntity().ToString());
    }

    [Fact]
    public void CreateEntity_WithFullCapacity_Throws()
    {
        var entityManager = new EntityManager(3);
        entityManager.CreateEntity();
        entityManager.CreateEntity();
        entityManager.CreateEntity();

        Assert.Throws<InvalidOperationException>(() => entityManager.CreateEntity());
    }

    [Fact]
    public void HasEntity()
    {
        var entityManager = new EntityManager(3);
        Assert.False(entityManager.HasEntity(new Entity(1)));

        entityManager.CreateEntity();
        Assert.True(entityManager.HasEntity(new Entity(1)));
    }

    [Fact]
    public void DestroyEntity()
    {
        var entityManager = new EntityManager(3);
        var entity = entityManager.CreateEntity();
        Assert.True(entityManager.HasEntity(entity));

        entityManager.DestroyEntity(entity);
        Assert.False(entityManager.HasEntity(entity));
    }

    [Fact]
    public void DestroyEntity_WithNonExistentEntity_DoesNothing()
    {
        var entityManager = new EntityManager(3);
        entityManager.DestroyEntity(new Entity(1));
    }
}
