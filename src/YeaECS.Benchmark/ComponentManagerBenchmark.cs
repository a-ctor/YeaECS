namespace YeaECS.Benchmark;

using BenchmarkDotNet.Attributes;

public class ComponentManagerBenchmark
{
    private record struct Component1(int Value);

    private ComponentManager<Component1> _initialComponentManager;
    private ComponentManager<Component1> _componentManager;

    [Params(100, 1_000, 10_000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        _initialComponentManager = new ComponentManager<Component1>(N + 200);
        for (var i = 0; i < N; i++)
        {
            var component = new Component1(1);
            _initialComponentManager.AddComponent(new Entity(1, (uint)((i + 1) * 2)), component);
        }

        _componentManager = _initialComponentManager.Clone();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _componentManager = _initialComponentManager.Clone();
    }
    
    [Benchmark]
    public void ComponentInsertFront()
    {
        var component1 = new Component1(3);
        for (var i = 0; i < 100; i++)
            _componentManager.AddComponent(new Entity(1, (uint)(i * 2 + 1)), in component1);
    }

    [Benchmark]
    public void ComponentInsertMiddle()
    {
        var component1 = new Component1(3);
        for (var i = 0; i < 100; i++)
            _componentManager.AddComponent(new Entity(1, (uint)((N / 2 + i) * 2 + 1)), in component1);
    }

    [Benchmark]
    public void ComponentInsertBack()
    {
        var component1 = new Component1(3);
        for (var i = 0; i < 100; i++)
            _componentManager.AddComponent(new Entity(1, (uint)((N - 100 + i) * 2 + 1)), in component1);
    }
}
