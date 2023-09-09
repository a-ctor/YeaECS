namespace Wildfire.Ecs;

public interface IViewEnumerator
{
    EntityReference Current { get; }

    bool MoveNext();

    bool MoveTo(Entity entity);
}
