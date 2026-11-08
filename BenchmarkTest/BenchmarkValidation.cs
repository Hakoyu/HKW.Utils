namespace BenchmarkTest;

internal static class BenchmarkValidation
{
    public static void Run()
    {
        // 首次枚举验证必须先于稳态验证, 真正冷测量仍由独立进程完成.
        var cold = new EnumFirstUseBenchmarks();
        cold.FirstGetInfo();
        cold.Validate();
        var steady = new EnumSteadyBenchmarks();
        steady.Setup();
        EnumSteadyBenchmarks.Validate(steady.GetInfo());
        Console.WriteLine("PASS enum first-use and steady-state metadata");

        foreach (var length in new[] { 2048, 2049 })
        {
            var span = new SpanPoolBenchmarks { Length = length };
            span.Setup();
            span.Validate(span.ToPascal());
            Console.WriteLine($"PASS span length={length}, output length={span.ToPascal().Length}");
        }

        foreach (var subscribed in new[] { false, true })
        {
            var events = new EventArgsBenchmarks { Subscribed = subscribed };
            foreach (var invoke in new Action[] { events.Allocate, events.LocalCache, events.PublicCache })
            {
                events.Setup();
                invoke();
                events.Validate();
            }
            var geometry = new GeometryNotificationBenchmarks { Subscribed = subscribed };
            geometry.Setup();
            geometry.Validate();
            geometry.Validate();
            Console.WriteLine($"PASS event delivery and geometry notifications, subscribed={subscribed}");
        }

        foreach (var count in new[] { 64, 1024 })
        {
            var collection = new CollectionFastPathBenchmarks { Count = count };
            collection.Setup();
            for (var repeat = 0; repeat < 2; repeat++)
            {
                if (collection.InsertRangeNonAliased() != count)
                    throw new InvalidOperationException("Insertion returned an incorrect count.");
                collection.Validate();
            }

            var filter = new FilterUpdateBenchmarks { Count = count };
            filter.Setup();
            filter.RefreshUnchanged();
            filter.Validate();
            filter.ReplaceMiddle();
            filter.Validate();
            filter.ReplaceMiddle();
            filter.Validate();

            var bidirectional = new BidirectionalBenchmarks { Count = count };
            bidirectional.Setup();
            try { bidirectional.Validate(); }
            finally { bidirectional.Cleanup(); }

            foreach (var customEvents in new[] { false, true })
            {
                var binding = new BindingBatchBenchmarks { Count = count, CustomEvents = customEvents };
                binding.Setup();
                try
                {
                    for (var repeat = 0; repeat < 2; repeat++)
                    {
                        if (binding.ClearAndAddBatch() != count)
                            throw new InvalidOperationException("Binding returned an incorrect count.");
                        binding.Validate();
                    }
                }
                finally { binding.Cleanup(); }
            }
            Console.WriteLine($"PASS collection, filtering, bidirectional snapshots/frozen build and binding batches, count={count}");
        }
        Console.WriteLine("PASS all benchmark correctness checks");
    }
}
