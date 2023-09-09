namespace Wildfire.Ecs;

public delegate void EntityCreatingEventHandler(EntityReference entity);

public delegate void EntityCreatedEventHandler(EntityReference entity);

public delegate void EntityDeletingEventHandler(EntityReference entity);

public delegate void EntityDeletedEventHandler(Entity entity);
