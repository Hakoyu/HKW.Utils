using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace BenchmarkTest;

[MemoryDiagnoser]
public class EventArgsBenchmarks
{
    private static readonly PropertyChangedEventArgs _countChangedArgs = new("Count");
    private PropertyChangedEventArgs? _received;

    [Params(false, true)]
    public bool Subscribed { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _received = null;
        PropertyChanged = Subscribed ? OnChanged : null;
    }

    private void OnChanged(object? sender, PropertyChangedEventArgs args) => _received = args;

    // 每次调用只通知一次, 无订阅时条件调用也不会创建参数.
    [Benchmark(Baseline = true)]
    public void Allocate() => PropertyChanged?.Invoke(this, new("Count"));

    [Benchmark]
    public void LocalCache() => PropertyChanged?.Invoke(this, _countChangedArgs);

    [Benchmark]
    public void PublicCache() => PropertyChanged?.Invoke(this, PropertyChangedArgsCache.Count);

    [GlobalCleanup]
    public void Validate()
    {
        if (Subscribed ? _received?.PropertyName != "Count" : _received is not null)
            throw new InvalidOperationException("Event delivery does not match the subscription state.");
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

[MemoryDiagnoser]
public class GeometryNotificationBenchmarks
{
    [Params(false, true)]
    public bool Subscribed { get; set; }

    private ObservablePoint<int> _point = null!;
    private long _changing;
    private long _changed;

    [GlobalSetup]
    public void Setup()
    {
        _point = new(0, 1);
        _changing = _changed = 0;
        if (Subscribed)
        {
            _point.PropertyChanging += (_, _) => _changing++;
            _point.PropertyChanged += (_, _) => _changed++;
        }
    }

    [Benchmark]
    public int EqualAssignment()
    {
        _point.X = _point.X;
        return _point.X;
    }

    [Benchmark]
    public int ChangedAssignment()
    {
        _point.X ^= 1;
        return _point.X;
    }

    [GlobalCleanup]
    public void Validate()
    {
        var before = _point.X;
        var changing = _changing;
        var changed = _changed;
        if (EqualAssignment() != before || _changing != changing || _changed != changed)
            throw new InvalidOperationException("Equal geometry assignment must not notify.");
        var expectedEvents = Subscribed ? 2 : 0;
        if (ChangedAssignment() != (before ^ 1)
            || _changing - changing != expectedEvents || _changed - changed != expectedEvents)
            throw new InvalidOperationException("Changed geometry assignment must update and notify derived properties.");
    }
}

[MemoryDiagnoser]
public class BindingBatchBenchmarks
{
    [Params(64, 1024)]
    public int Count { get; set; }

    [Params(false, true)]
    public bool CustomEvents { get; set; }

    private ObservableList<int> _source = null!;
    private List<int> _target = null!;
    private int[] _items = null!;
    private IDisposable _binding = null!;

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(0, Count).ToArray();
        _source = new(Count);
        _target = new(Count);
        _binding = CustomEvents ? _source.BindingListX(_target) : _source.BindingList(_target);
    }

    // 一次清空及 Count 项添加, 当前列表按项发布事件, 不是单个范围通知.
    // 复用容量, 不包含建立绑定成本, 所有逐项通知和目标同步均包含在计时内.
    [Benchmark]
    public int ClearAndAddBatch()
    {
        _source.Clear();
        _source.AddRange(_items);
        return _target.Count;
    }

    public void Validate()
    {
        if (_source.SequenceEqual(_items) is false || _target.SequenceEqual(_items) is false)
            throw new InvalidOperationException("Bound target contents do not match the source batch.");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try { Validate(); }
        finally { _binding.Dispose(); }
    }
}
