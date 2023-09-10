namespace YeaECS.Benchmark;

using BenchmarkDotNet.Attributes;

public class ViewBenchmark
{
    private record struct Component1(int Value);

    private record struct Component2(int Value);

    private record struct Component3(int Value);

    private record struct Component4(int Value);

    private record struct Component5(int Value);

    private EntityRegistry _entityRegistry;

    /// <summary>
    /// Controls whether one component has a lot less instances when iterating (=optimistic).
    /// Otherwise each component has roughly the same amount of instances.
    /// </summary>
    [Params(false, true)]
    public bool Optimisitic;
    
    /// <summary>
    /// Controls the amount of entities in the registry.
    /// </summary>
    [Params(100, 1_000, 10_000)]
    public int N;
    
    [GlobalSetup]
    public void Setup()
    {
        _entityRegistry = new EntityRegistry(N * 2);
        
        var random = new Random(1337);

        var component1Chances = Optimisitic ? 10 : 2;
        for (var i = 0; i < N; i++)
        {
            var entity = _entityRegistry.CreateEntity();

            var hasComponent1 = random.Next(0, component1Chances) == 0;
            if (hasComponent1)
                entity.AddComponent(new Component1());

            var hasComponent2 = random.Next(0, 2) == 0;
            if (hasComponent2)
                entity.AddComponent(new Component2());

            var hasComponent3 = random.Next(0, 2) == 0;
            if (hasComponent3)
                entity.AddComponent(new Component3());

            var hasComponent4 = random.Next(0, 2) == 0;
            if (hasComponent4)
                entity.AddComponent(new Component4());

            var hasComponent5 = random.Next(0, 2) == 0;
            if (hasComponent5)
                entity.AddComponent(new Component5());
        }
    }

    [Benchmark]
    public int SingleComponentIteration()
    {
        int total = 0;
        
        var view = _entityRegistry.ViewOf<Component1>();
        foreach (var entity in view)
        {
            ref var component1 = ref entity.GetComponent<Component1>();

            total += component1.Value;
        }

        return total;
    }

    [Benchmark]
    public int TwoComponentIteration()
    {
        int total = 0;
        
        var view = _entityRegistry.ViewOf<Component2, Component1>();
        foreach (var entity in view)
        {
            ref var component1 = ref entity.GetComponent<Component1>();
            ref var component2 = ref entity.GetComponent<Component2>();

            total += component1.Value;
            total += component2.Value;
        }

        return total;
    }

    [Benchmark]
    public int ThreeComponentIteration()
    {
        int total = 0;
        
        var view = _entityRegistry.ViewOf<Component3, Component2, Component1>();
        foreach (var entity in view)
        {
            ref var component1 = ref entity.GetComponent<Component1>();
            ref var component2 = ref entity.GetComponent<Component2>();
            ref var component3 = ref entity.GetComponent<Component3>();

            total += component1.Value;
            total += component2.Value;
            total += component3.Value;
        }

        return total;
    }

    [Benchmark]
    public int FourComponentIteration()
    {
        int total = 0;
        
        var view = _entityRegistry.ViewOf<Component4, Component3, Component2, Component1>();
        foreach (var entity in view)
        {
            ref var component1 = ref entity.GetComponent<Component1>();
            ref var component2 = ref entity.GetComponent<Component2>();
            ref var component3 = ref entity.GetComponent<Component3>();
            ref var component4 = ref entity.GetComponent<Component4>();

            total += component1.Value;
            total += component2.Value;
            total += component3.Value;
            total += component4.Value;
        }

        return total;
    }

    [Benchmark]
    public int FiveComponentIteration()
    {
        int total = 0;
        
        var view = _entityRegistry.ViewOf<Component5, Component4, Component3, Component2, Component1>();
        foreach (var entity in view)
        {
            ref var component1 = ref entity.GetComponent<Component1>();
            ref var component2 = ref entity.GetComponent<Component2>();
            ref var component3 = ref entity.GetComponent<Component3>();
            ref var component4 = ref entity.GetComponent<Component4>();
            ref var component5 = ref entity.GetComponent<Component5>();

            total += component1.Value;
            total += component2.Value;
            total += component3.Value;
            total += component4.Value;
            total += component5.Value;
        }

        return total;
    }
}
