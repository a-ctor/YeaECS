namespace Wildfire.Ecs;

/// <summary>
/// Provides a value that can be returned with ref return.
/// </summary>
public static class RefDummy<T>
{
#pragma warning disable CS8618
    public static T Value;
#pragma warning restore CS8618
}
