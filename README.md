# Wildfire.Ecs

A low-level entity component system (ECS) targeting .NET Standard 2.0.

## Features

- Any type (reference or value type) can be a component
- No component/view registration is necessary
- Get direct references to the components
- Minimal allocations
- Pretty speedy

## Usage

The `EntityRegistry` class is the heart of the ECS where all operations are started. It is used to create entities, manipulate components, and access views of components. Here is a simple example of how to create an entity, add a component, and how to iterate over all entities with a certain component:

```c#
var entityRegistry = new EntityRegistry(capacity: 100);

var myEntity = entityRegistry.CreateEntity();
myEntity.AddComponent(new TransformComponent());

ref var transformComponent = ref myEntity.GetComponent<TransformComponent>();
transformComponent.Position = new Vector2(1, 1);

var view = entityRegistry.ViewOf<TransformComponent>();
foreach (var entity in view)
{
  transformComponent = ref entity.GetComponent<TransformComponent>();
  Console.WriteLine($"entity: {entity}; position: {transformComponent.Position}");
}
```

## Implementation details

Entities are implemented as generational identifiers (`uint Generation` and `uint Id`), with the ids being reused in different generations. Components are stored in a sparse set using the entity id as index. This allows accessing and manipulating components in constant time. 

Iteration over one or more components is done in linear time as it depends on the number of registered components. Using views to search for entities with a certain set of components is very efficient as the smallest component count determines the iteration time.
