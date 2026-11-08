using System.Globalization;
using BenchmarkDotNet.Attributes;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace BenchmarkTest;

[MemoryDiagnoser]
public class CollectionFastPathBenchmarks
{
    [Params(64, 1024)]
    public int Count { get; set; }

    private int[] _items = null!;
    private List<int> _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(0, Count).ToArray();
        _target = new List<int>(Count);
    }

    // 一次完整批量插入, 包含清空复用目标的成本, 不按元素归一化.
    [Benchmark]
    public int InsertRangeNonAliased()
    {
        _target.Clear();
        IList<int> target = _target;
        target.InsertRange(0, _items);
        return _target.Count;
    }

    [GlobalCleanup]
    public void Validate()
    {
        if (_target.SequenceEqual(_items) is false)
            throw new InvalidOperationException("Non-aliased insertion produced incorrect contents.");
    }
}

[MemoryDiagnoser]
public class FilterUpdateBenchmarks
{
    [Params(64, 1024)]
    public int Count { get; set; }

    private FilteredListWrapper<int, List<int>, List<int>> _list = null!;

    [GlobalSetup]
    public void Setup() => _list = new(
        Enumerable.Range(0, Count).ToList(), [], static value => (value & 1) == 0);

    [Benchmark]
    public int RefreshUnchanged()
    {
        _list.Refresh();
        return _list.FilteredList.Count;
    }

    // 每次切换中间位置的匹配状态, 避免首轮后退化为无变化写入.
    [Benchmark]
    public int ReplaceMiddle()
    {
        var index = Count / 2;
        _list[index] ^= 1;
        return _list.FilteredList.Count;
    }

    [GlobalCleanup]
    public void Validate()
    {
        if (_list.IsFilterValid is false
            || _list.FilteredList.SequenceEqual(_list.SourceList.Where(static value => (value & 1) == 0)) is false)
            throw new InvalidOperationException("Filtered contents differ from the source predicate.");
    }
}

[MemoryDiagnoser]
public class BidirectionalBenchmarks
{
    [Params(64, 1024)]
    public int Count { get; set; }

    private KeyValuePair<int, string>[] _items = null!;
    private FrozenBidirectionalDictionary<int, string> _frozen = null!;
    private ConcurrentBidirectionalDictionary<int, string> _concurrent = null!;
    private string _key = null!;

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(0, Count)
            .Select(static value => KeyValuePair.Create(value, value.ToString(CultureInfo.InvariantCulture)))
            .ToArray();
        _key = _items[Count / 2].Value;
        _frozen = new(_items, comparer2: StringComparer.Ordinal);
        _concurrent = new(_items, comparer2: StringComparer.Ordinal);
    }

    [Benchmark]
    public string FrozenForward() => _frozen.GetForward(Count / 2);

    [Benchmark]
    public int FrozenReverse() => _frozen.GetReverse(_key);

    [Benchmark]
    public string ConcurrentForward() => _concurrent.GetForward(Count / 2);

    [Benchmark]
    public int ConcurrentReverse() => _concurrent.GetReverse(_key);

    // 快照复制与冻结构建均为一次完整操作, 不能和单次查询解释为优化比值.
    [Benchmark]
    public IReadOnlyDictionary<int, string> ForwardSnapshot() => _concurrent.Dictionary1;

    [Benchmark]
    public IReadOnlyDictionary<string, int> ReverseSnapshot() => _concurrent.Dictionary2;

    [Benchmark]
    public FrozenBidirectionalDictionary<int, string> BuildFrozen() => new(_items, comparer2: StringComparer.Ordinal);

    public void Validate()
    {
        var frozen = BuildFrozen();
        var forward = ForwardSnapshot();
        var reverse = ReverseSnapshot();
        if (FrozenForward() != _key || FrozenReverse() != Count / 2
            || ConcurrentForward() != _key || ConcurrentReverse() != Count / 2
            || frozen.Count != Count || forward.Count != Count || reverse.Count != Count
            || _items.Any(pair => frozen.GetForward(pair.Key) != pair.Value
                || frozen.GetReverse(pair.Value) != pair.Key
                || forward[pair.Key] != pair.Value || reverse[pair.Value] != pair.Key))
            throw new InvalidOperationException("Bidirectional lookup, snapshot or frozen construction failed.");
        if (_concurrent.TryAddForward(Count, "extra") is false)
            throw new InvalidOperationException("Snapshot independence probe could not add an item.");
        if (forward.Count != Count || reverse.Count != Count)
            throw new InvalidOperationException("Snapshots must be independent of subsequent writes.");
        _concurrent.RemoveForward(Count);
    }

    [GlobalCleanup]
    public void Cleanup() => _concurrent.Dispose();
}
