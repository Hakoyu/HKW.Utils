using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace BenchmarkTest;

[MemoryDiagnoser]
public class SpanPoolBenchmarks
{
    // 库在长度大于 2048 时使用共享池, 相邻长度覆盖两侧路径.
    [Params(2048, 2049)]
    public int Length { get; set; }

    private string _text = null!;
    private string _expected = null!;

    [GlobalSetup]
    public void Setup()
    {
        _text = "ab " + new string('C', Length - 3);
        _expected = "AbC" + new string('c', Length - 4);
        Validate(ToPascal());
    }

    [Benchmark]
    public string ToPascal() => _text.AsSpan().ToPascal(' ', true, CultureInfo.InvariantCulture);

    public void Validate(string result)
    {
        if (result != _expected)
            throw new InvalidOperationException("Span conversion produced incorrect text.");
    }
}

public enum BenchmarkEnum
{
    None,
    Ready,
    Done,
}

[MemoryDiagnoser]
public class EnumSteadyBenchmarks
{
    private readonly Enum _value = BenchmarkEnum.Ready;

    [GlobalSetup]
    public void Setup() => Validate(GetInfo());

    [Benchmark]
    public IEnumInfo GetInfo() => _value.GetInfo();

    public static void Validate(IEnumInfo info)
    {
        if (info.Value.Equals(BenchmarkEnum.Ready) is false || info.EnumType != typeof(BenchmarkEnum))
            throw new InvalidOperationException("Enum metadata does not describe the requested value.");
    }
}

// 每个独立进程只调用一次, 禁止在准备阶段触碰枚举缓存, 包含首次工厂及元数据初始化.
[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, launchCount: 5, warmupCount: 0, iterationCount: 1)]
[InvocationCount(1, unrollFactor: 1)]
public class EnumFirstUseBenchmarks
{
    private readonly Enum _value = BenchmarkEnum.Ready;
    private IEnumInfo? _result;
    private long _firstCallAllocatedBytes;

    [Benchmark]
    public IEnumInfo FirstGetInfo()
    {
        // 内存诊断会再次执行方法, 单独保留真正首次调用的当前线程托管分配.
        var isFirstCall = _result is null;
        var before = GC.GetAllocatedBytesForCurrentThread();
        var result = _value.GetInfo();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (isFirstCall)
            _firstCallAllocatedBytes = allocated;
        return _result = result;
    }

    [GlobalCleanup]
    public void Validate()
    {
        EnumSteadyBenchmarks.Validate(
            _result ?? throw new InvalidOperationException("The first-use benchmark was not invoked."));
        Console.WriteLine($"FIRST_USE_ALLOCATED_BYTES={_firstCallAllocatedBytes}");
    }
}
