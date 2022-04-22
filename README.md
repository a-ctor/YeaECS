# Wildfire.Ecs

A low-level entity component system (ECS) targeting .NET Standard 2.0.

## Features

- Any structure can be a component
- Efficient iteration over entities with certain components using views
- No component/view registration is necessary
- Basic lifetime events
- Minimal allocations

## Trade-offs

This ECS is built to deliver fast views over components without any previous setup. This has the effect that adding components to older entities is a slower operation. The older the component the slower the operation is (best: `O(1)` worst: `O(N)`).

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
  transformComponent = ref view.Get<TransformComponent>();
  Console.WriteLine($"entity: {entity.Id}; position: {transformComponent.Position}");
}
```

