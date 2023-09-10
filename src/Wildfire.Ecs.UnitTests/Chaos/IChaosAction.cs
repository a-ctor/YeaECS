﻿namespace Wildfire.Ecs.UnitTests.Chaos;

public interface IChaosAction
{
    void Record(ChaosTracker tracker);

    void Apply(EntityRegistry entityRegistry);
}
