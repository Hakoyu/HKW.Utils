using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using HKW.HKWUtils.Collections;

namespace BenchmarkTest;

internal class BenchmarkProgram
{
    static void Main(string[] args)
    {
        _ = BenchmarkRunner.Run<Test>();
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.HostProcess, warmupCount: 3, iterationCount: 100)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Test
{
    private const int BatchSize = 1_000_000;

    [Params(1024, 16384)]
    public int Size { get; set; }

    private int _sizeMask;
    private string[] _values = Array.Empty<string>();
    private string[] _altValues = Array.Empty<string>();

    private BidirectionalDictionary<int, string> _dictionary = new();
    private BidirectionalDictionaryWrapper<
        int,
        string,
        Dictionary<int, string>,
        Dictionary<string, int>
    > _wrapper = new(new Dictionary<int, string>(), new Dictionary<string, int>(), null, null);

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sizeMask = Size - 1;
        _values = Enumerable.Range(0, Size).Select(i => $"V{i}").ToArray();
        _altValues = Enumerable.Range(0, Size).Select(i => $"A{i}").ToArray();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _dictionary = new(Size, EqualityComparer<int>.Default, EqualityComparer<string>.Default);

        _wrapper = new(
            new Dictionary<int, string>(Size, EqualityComparer<int>.Default),
            new Dictionary<string, int>(Size, EqualityComparer<string>.Default),
            EqualityComparer<int>.Default,
            EqualityComparer<string>.Default
        );

        for (var i = 0; i < Size; i++)
        {
            var value = _values[i];
            _ = _dictionary.TryAdd(i, value);
            _ = _wrapper.TryAdd(i, value);
        }
    }

    [Benchmark(Baseline = true)]
    public int TryGetValue()
    {
        var hit = 0;
        for (var i = 0; i < BatchSize; i++)
        {
            var key = i & _sizeMask;
            if (_dictionary.TryGetValue(key, out _))
                hit++;
        }
        return hit;
    }

    [Benchmark]
    public int TryGetValue_Wrapper()
    {
        var hit = 0;
        for (var i = 0; i < BatchSize; i++)
        {
            var key = i & _sizeMask;
            if (_wrapper.TryGetValue(key, out _))
                hit++;
        }
        return hit;
    }

    [Benchmark]
    public int TrySetValue()
    {
        var okCount = 0;
        for (var i = 0; i < BatchSize; i++)
        {
            var key = i & _sizeMask;
            if (_dictionary.TrySetValue(key, _altValues[key]))
                okCount++;
            if (_dictionary.TrySetValue(key, _values[key]))
                okCount++;
        }
        return okCount;
    }

    [Benchmark]
    public int TrySetValue1()
    {
        var okCount = 0;
        for (var i = 0; i < BatchSize; i++)
        {
            var key = i & _sizeMask;
            if (_dictionary.TrySetValue1(key, _altValues[key]))
                okCount++;
            if (_dictionary.TrySetValue1(key, _values[key]))
                okCount++;
        }
        return okCount;
    }

    [Benchmark]
    public int TrySetValue_Wrapper()
    {
        var okCount = 0;
        for (var i = 0; i < BatchSize; i++)
        {
            var key = i & _sizeMask;
            if (_wrapper.TrySetValue(key, _altValues[key]))
                okCount++;
            if (_wrapper.TrySetValue(key, _values[key]))
                okCount++;
        }
        return okCount;
    }
}
