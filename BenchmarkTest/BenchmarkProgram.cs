using BenchmarkDotNet.Running;

namespace BenchmarkTest;

internal class BenchmarkProgram
{
    static int Main(string[] args)
    {
        if (args is ["--validate"])
        {
            BenchmarkValidation.Run();
            return 0;
        }

        // 无参数时只列出场景, 不启动交互选择或全场景长跑.
        var summaries = BenchmarkSwitcher.FromAssembly(typeof(BenchmarkProgram).Assembly)
            .Run(args.Length == 0 ? ["--list", "flat"] : args);
        return summaries.Any(summary => summary.HasCriticalValidationErrors
            || summary.Reports.Any(report => report.Success is false)) ? 1 : 0;
    }

    #region 历史字典基准草稿, 仅保留原始注释, 不参与测量
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
    #endregion
}
