using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace BenchmarkTest;

internal class BenchmarkProgram
{
    static void Main(string[] args)
    {
        _ = BenchmarkRunner.Run<Test>();
    }
}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.HostProcess, warmupCount: 3, iterationCount: 50)]
public class Test
{
    private const int Count = 1_000_000;

    private KeyValuePair<int, string>[] _items;

    private ObservableDictionary<int, string> _dictionary1;
    private ObservableDictionaryWrapper<int, string, Dictionary<int, string>> _dictionary2;
    private ObservableDictionaryWrapper<int, string, OrderedDictionary<int, string>> _dictionary3;

    private static readonly PropertyChangedEventArgs _countChangedArgs = new(nameof(Count));
    public static PropertyChangedEventArgs CountChangedArgs = new(nameof(Count));

    [GlobalSetup]
    public void GlobalSetup()
    {
        PropertyChanged += Test_PropertyChanged;
    }

    private void Test_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        return;
    }

    [Benchmark]
    public void Test1()
    {
        for (var i = 0; i < Count; i++)
            PropertyChanged?.Invoke(this, new(nameof(Count)));
    }

    [Benchmark]
    public void Test2()
    {
        for (var i = 0; i < Count; i++)
            PropertyChanged?.Invoke(this, _countChangedArgs);
    }

    [Benchmark]
    public void Test3()
    {
        for (var i = 0; i < Count; i++)
            PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    //[GlobalSetup]
    //public void GlobalSetup()
    //{
    //    _items = Enumerable
    //        .Range(0, Count)
    //        .Select(i => KeyValuePair.Create(i, i.ToString()))
    //        .ToArray();
    //}

    //[IterationSetup(Targets = [nameof(TryGetValue1), nameof(TryGetValue2), nameof(TryGetValue3)])]
    //public void IterationSetupForTryGetValue()
    //{
    //    _dictionary1 = new(_items);
    //    _dictionary2 = new(new(_items));
    //    _dictionary3 = new(new(_items));
    //}

    //[IterationSetup(Targets = [nameof(AddValue1), nameof(AddValue2), nameof(AddValue3)])]
    //public void IterationSetupForAddValue()
    //{
    //    _dictionary1 = new();
    //    _dictionary2 = new(new());
    //    _dictionary3 = new(new());
    //}

    //[Benchmark]
    //public object? TryGetValue1()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary1.TryGetValue(item.Key, out _);
    //    }
    //    return _dictionary1;
    //}

    //[Benchmark]
    //public object? TryGetValue2()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary2.TryGetValue(item.Key, out _);
    //    }
    //    return _dictionary2;
    //}

    //[Benchmark]
    //public object? TryGetValue3()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary3.TryGetValue(item.Key, out _);
    //    }
    //    return _dictionary3;
    //}

    //[Benchmark]
    //public object? AddValue1()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary1.Add(item.Key, item.Value);
    //    }
    //    return _dictionary1;
    //}

    //[Benchmark]
    //public object? AddValue2()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary2.Add(item.Key, item.Value);
    //    }
    //    return _dictionary2;
    //}

    //[Benchmark]
    //public object? AddValue3()
    //{
    //    for (var i = 0; i < Count; i++)
    //    {
    //        var item = _items[i];
    //        _dictionary3.Add(item.Key, item.Value);
    //    }
    //    return _dictionary3;
    //}
}
