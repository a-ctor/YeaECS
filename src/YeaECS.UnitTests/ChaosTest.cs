namespace YeaECS.UnitTests;

using System;
using System.Linq;
using Chaos;
using Xunit;

public class ChaosTest
{
    private struct Component1
    {
        public int Value;

        public Component1(int value)
        {
            Value = value;
        }
    }

    private struct Component2
    {
        public int Value;

        public Component2(int value)
        {
            Value = value;
        }
    }

    [Fact]
    public void AddRemoveEntitiesAndComponents_Chaos()
    {
        const int c_capacity = 1000;
        var chaosTester = new ChaosTester(c_capacity);

        var current = 0;
        var random = new Random(1337);
        for (var i = 0; i < 10; i++)
        {
            var fill = i % 2 == 0;
            if (fill)
            {
                var toAdd = random.Next(0, 100 - current);

                for (var j = 0; j < toAdd; j++)
                {
                    var scenario = random.Next(0, 4);
                    var entity = chaosTester.AddEntity();
                    if (scenario is 1 or 4)
                        chaosTester.AddComponent(entity, new Component1(random.Next(0, 40)));
                    if (scenario is 2 or 4)
                        chaosTester.AddComponent(entity, new Component2(random.Next(0, 40)));
                    current++;
                }
            }
            else
            {
                var toRemove = random.Next(0, current);
                for (var j = 0; j < toRemove; j++)
                {
                    var scenario = random.Next(0, 4);
                    var target = chaosTester.Tracker.Entities[random.Next(chaosTester.Tracker.Entities.Count)];
                    if (scenario == 0)
                    {
                        chaosTester.DestroyEntity(target);
                        current--;
                    }
                    if (scenario is 1 or 4)
                        chaosTester.RemoveComponent<Component1>(target);
                    if (scenario is 2 or 4)
                        chaosTester.RemoveComponent<Component2>(target);
                }
            }
        }

        var expectedSum1 = chaosTester.Tracker.GetAllComponents<Component1>().Sum(e => e.Value.Value);
        var actualSum1 = 0;
        foreach (var entity in chaosTester.Registry.GetComponentAccessor<Component1>())
            actualSum1 += chaosTester.Registry.GetComponent<Component1>(entity).Value;
        Assert.Equal(expectedSum1, actualSum1);

        var expectedSum2 = chaosTester.Tracker.GetAllComponents<Component2>().Sum(e => e.Value.Value);
        var actualSum2 = 0;
        foreach (var entity in chaosTester.Registry.GetComponentAccessor<Component2>())
            actualSum2 += chaosTester.Registry.GetComponent<Component2>(entity).Value;
        Assert.Equal(expectedSum2, actualSum2);
    }
}
