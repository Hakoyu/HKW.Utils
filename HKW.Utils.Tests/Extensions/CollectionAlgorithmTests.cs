using System.Collections;
using System.Collections.ObjectModel;
using HKW.HKWUtils.Extensions;
using CollectionExtensions = HKW.HKWUtils.Extensions.CollectionExtensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class CollectionAlgorithmTests
{
    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(0, 3)]
    [DataRow(3, 0)]
    [DataRow(1, 1)]
    [DataRow(2, 3)]
    public void ArrayShapes(int rows, int columns)
    {
        var array = new int[rows, columns];
        foreach (var separator in new[] { "", " | " })
        {
            var expected = string.Concat(
                Enumerable.Repeat(
                    string.Join(separator, Enumerable.Repeat("0", columns)) + Environment.NewLine,
                    rows
                )
            );
            Assert.AreEqual(expected, array.ToDelimitedString(separator));
            Assert.AreEqual(expected, array.ToDelimitedString(x => x.ToString(), separator));
        }
    }

    [TestMethod]
    public void NonzeroLowerBoundsAndNullFormatting()
    {
        var array = (string?[,])Array.CreateInstance(typeof(string), [1, 2], [-1, 4]);
        array[-1, 4] = null;
        array[-1, 5] = "a";
        Assert.AreEqual("|a" + Environment.NewLine, array.ToDelimitedString("|"));
        Assert.AreEqual(
            "NULL|A" + Environment.NewLine,
            array.ToDelimitedString(x => x?.ToUpperInvariant() ?? "NULL", "|")
        );
    }

    [TestMethod]
    [DataRow("set")]
    [DataRow("yield")]
    [DataRow("empty")]
    [DataRow("self")]
    [DataRow("wrapper")]
    [DataRow("delayed")]
    [DataRow("duplicates")]
    public void RemoveAllStableInput(string shape)
    {
        ICollection<int> target = new Collection<int>(new List<int> { 1, 2, 2, 3 });
        IEnumerable<int> items = shape switch
        {
            "set" => new HashSet<int> { 2 },
            "yield" => Yield(2),
            "empty" => Yield<int>(),
            "self" => target,
            "wrapper" => new ReadOnlyCollection<int>((IList<int>)target),
            "delayed" => target.Where(x => x == 2),
            _ => Yield(2, 2),
        };
        target.RemoveAll(items);
        int[] expected = shape switch
        {
            "empty" => [1, 2, 2, 3],
            "self" or "wrapper" => [],
            "delayed" or "duplicates" => [1, 3],
            _ => [1, 2, 3],
        };
        CollectionAssert.AreEqual(expected, target.ToArray());
    }

    [TestMethod]
    [DataRow(false, "self")]
    [DataRow(false, "wrapper")]
    [DataRow(false, "delayed")]
    [DataRow(true, "self")]
    [DataRow(true, "wrapper")]
    [DataRow(true, "delayed")]
    public void AddAndInsertSnapshotAliases(bool useList, string shape)
    {
        foreach (var insert in new[] { false, true })
        {
            IList<int> target = useList ? new List<int> { 1, 2, 3 } : new BoundedCollection();
            IEnumerable<int> source = shape switch
            {
                "self" => target,
                "wrapper" => new ReadOnlyCollection<int>(target),
                _ => target.Where(x => x > 0),
            };
            if (insert)
                ListExtensions.InsertRange(target, 1, source);
            else
                CollectionExtensions.AddRange(target, source);
            CollectionAssert.AreEqual(
                insert ? new[] { 1, 1, 2, 3, 2, 3 } : [1, 2, 3, 1, 2, 3],
                target.ToArray()
            );
        }
    }

    [TestMethod]
    public void SingleUseInputsAndInvalidInsertDoNotWrite()
    {
        IList<int> target = new Collection<int>();
        CollectionExtensions.AddRange(target, new OnceEnumerable<int>([1, 2]));
        ListExtensions.InsertRange(target, 1, new OnceEnumerable<int>([3, 4]));
        CollectionAssert.AreEqual(new[] { 1, 3, 4, 2 }, target.ToArray());
        var source = new OnceEnumerable<int>([5]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => target.InsertRange(-1, source));
        Assert.AreEqual(0, source.Enumerations);
        Assert.ThrowsExactly<InvalidOperationException>(() => target.AddRange(ThrowAfterOne()));
        CollectionAssert.AreEqual(new[] { 1, 3, 4, 2 }, target.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void TryElementAtBackendParity(int backend)
    {
        IEnumerable<int> source = backend switch
        {
            0 => new[] { 42 },
            1 => new List<int> { 42 },
            2 => new Collection<int> { 42 },
            _ => Yield(42),
        };
        Assert.IsTrue(source.TryElementAt(0, out var value));
        Assert.AreEqual(42, value);
        foreach (var index in new[] { 1, int.MaxValue })
        {
            Assert.IsFalse(source.TryElementAt(index, out value));
            Assert.AreEqual(0, value);
        }
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.TryElementAt(-1, out _));
        Assert.IsFalse(source.Take(0).TryElementAt(0, out _));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void FindRangesMatchList(bool useList)
    {
        foreach (var values in new[] { Array.Empty<int>(), new[] { 7 }, new[] { 1, 2, 1, 3 } })
        {
            IList<int> target = useList ? values.ToList() : new Collection<int>(values.ToList());
            var expected = values.ToList();
            foreach (
                var start in new[]
                {
                    int.MinValue,
                    -2,
                    -1,
                    0,
                    1,
                    values.Length - 1,
                    values.Length,
                    values.Length + 1,
                    int.MaxValue,
                }
            )
            foreach (var count in new[] { int.MinValue, -1, 0, 1, 2, values.Length, int.MaxValue })
            {
                CompareOutcome(
                    () => expected.FindIndex(start, count, x => x == 1),
                    () => target.FindIndex(start, count, 1, static (x, value) => x == value)
                );
                CompareOutcome(
                    () => expected.FindIndex(start, count, x => x == 1),
                    () => target.FindPair(start, count, 1, static (x, value) => x == value).index
                );
                CompareOutcome(
                    () => expected.FindIndex(start, count, x => x == 1),
                    () =>
                    {
                        var found = target.TryFindPair(
                            start,
                            count,
                            1,
                            static (x, value) => x == value,
                            out var pair
                        );
                        Assert.AreEqual(pair.Index >= 0, found);
                        return pair.Index;
                    }
                );
                CompareOutcome(
                    () => expected.FindLastIndex(start, count, x => x == 1),
                    () => target.FindLastIndex(start, count, 1, static (x, value) => x == value)
                );
                CompareOutcome(
                    () => expected.FindLastIndex(start, count, x => x == 1),
                    () =>
                        target.FindLastPair(start, count, 1, static (x, value) => x == value).index
                );
                CompareOutcome(
                    () => expected.FindLastIndex(start, count, x => x == 1),
                    () =>
                    {
                        var found = target.TryFindLastPair(
                            start,
                            count,
                            1,
                            static (x, value) => x == value,
                            out var pair
                        );
                        Assert.AreEqual(pair.Index >= 0, found);
                        return pair.Index;
                    }
                );
            }
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void FindStartOnlyAndTryValues(bool useList)
    {
        foreach (
            var values in new[] { Array.Empty<string?>(), new string?[] { null, "a", "a", "b" } }
        )
        {
            var expected = values.ToList();
            IList<string?> target = useList
                ? expected.ToList()
                : new Collection<string?>(expected.ToList());
            foreach (var value in new string?[] { null, "a", "missing" })
            {
                var first = expected.FindIndex(x => x == value);
                var last = expected.FindLastIndex(x => x == value);
                Assert.AreEqual(
                    first >= 0,
                    target.TryFind(value, static (x, item) => x == item, out var firstValue)
                );
                Assert.AreEqual(first >= 0 ? values[first] : null, firstValue);
                Assert.AreEqual(
                    last >= 0,
                    target.TryFindLast(value, static (x, item) => x == item, out var lastValue)
                );
                Assert.AreEqual(last >= 0 ? values[last] : null, lastValue);
                foreach (var start in new[] { int.MinValue, -1, 0, 1, values.Length, int.MaxValue })
                {
                    CompareOutcome(
                        () => expected.FindIndex(start, x => x == value),
                        () => target.FindIndex(start, value, static (x, item) => x == item)
                    );
                    CompareOutcome(
                        () => expected.FindLastIndex(start, x => x == value),
                        () => target.FindLastIndex(start, value, static (x, item) => x == item)
                    );
                }
            }
        }
    }

    [TestMethod]
    public void EmptyFindOverloadsAndNullPredicates()
    {
        IList<int> empty = new Collection<int>();
        Assert.AreEqual(-1, empty.FindIndex(1, static (x, value) => x == value));
        Assert.AreEqual(-1, empty.FindLastIndex(1, static (x, value) => x == value));
        Assert.AreEqual((-1, 0), empty.FindPair(1, static (x, value) => x == value));
        Assert.AreEqual((-1, 0), empty.FindLastPair(1, static (x, value) => x == value));
        Assert.IsFalse(empty.TryFind(1, static (x, value) => x == value, out _));
        Assert.IsFalse(empty.TryFindLast(1, static (x, value) => x == value, out _));
        Assert.IsFalse(empty.TryFindPair(1, static (x, value) => x == value, out _));
        Assert.IsFalse(empty.TryFindLastPair(1, static (x, value) => x == value, out _));
        Assert.IsFalse(empty.TryFindPair(0, 1, static (x, value) => x == value, out _));
        Assert.IsFalse(empty.TryFindLastPair(-1, 1, static (x, value) => x == value, out _));
        Assert.AreEqual((-1, 0), empty.FindPair(0, 1, static (x, value) => x == value));
        Assert.AreEqual((-1, 0), empty.FindLastPair(-1, 1, static (x, value) => x == value));
        Assert.ThrowsExactly<ArgumentNullException>(() => empty.FindIndex(0, 0, 1, null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => empty.FindLastIndex(-1, 0, 1, null!));
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void RangeMutationParity(bool useList, bool reverse)
    {
        foreach (var length in new[] { 0, 1, 6 })
        foreach (
            var start in new[] { int.MinValue, -1, 0, 1, 2, 3, length, length + 1, int.MaxValue }
        )
        foreach (var count in new[] { int.MinValue, -1, 0, 1, 2, 3, length, int.MaxValue })
        {
            var expected = Enumerable.Range(0, length).ToList();
            IList<int> target = useList
                ? expected.ToList()
                : new Collection<int>(expected.ToList());
            CompareOutcome(
                () =>
                {
                    if (reverse)
                        expected.Reverse(start, count);
                    else
                        expected.RemoveRange(start, count);
                    return 0;
                },
                () =>
                {
                    if (reverse)
                        ListExtensions.Reverse(target, start, count);
                    else
                        ListExtensions.RemoveRange(target, start, count);
                    return 0;
                }
            );
            CollectionAssert.AreEqual(expected.ToArray(), target.ToArray());
        }
    }

    [TestMethod]
    public void NonGenericMultisets()
    {
        IEnumerable a = new object?[] { null, 1, 1, "a" };
        IEnumerable b = new ArrayList { "a", 1, null, 1 };
        Assert.IsTrue(a.ItemsEqual(b));
        Assert.IsTrue(b.ItemsEqual(a));
        foreach (
            IEnumerable other in new IEnumerable[]
            {
                Array.Empty<object>(),
                new object?[] { null, 1, "a" },
                new object?[] { null, 1, "a", "a" },
                new object?[] { null, 1, 1, "a", 2 },
            }
        )
        {
            Assert.IsFalse(a.ItemsEqual(other));
            Assert.IsFalse(other.ItemsEqual(a));
        }
    }

    [TestMethod]
    public void NullAndDuplicateCounts()
    {
        Assert.IsTrue(
            new string?[] { null, "a", null }.UnorderedEqual(new string?[] { "a", null, null })
        );
        Assert.IsFalse(
            new string?[] { null, "a", null }.UnorderedEqual(new string?[] { "a", "a", null })
        );
        Assert.IsTrue(new int?[] { 1, null, 1 }.UnorderedEqual(Yield<int?>(null, 1, 1)));
        Assert.IsFalse(Yield<int?>(null, 1).UnorderedEqual(Yield<int?>(null, null, 1)));
        Assert.IsTrue(Yield<int?>().UnorderedEqual(Array.Empty<int?>()));
        Assert.IsFalse(Yield(1, 1, 2).UnorderedEqual(Yield(1, 2, 2)));
    }

    [TestMethod]
    public void DefaultEqualityIndependentOfSetComparer()
    {
        var a = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a" };
        var b = new HashSet<string>(StringComparer.Ordinal) { "A" };
        Assert.IsFalse(a.UnorderedEqual(b));
        Assert.IsFalse(b.UnorderedEqual(a));
        Assert.IsFalse(a.ToArray().UnorderedEqual(b));
        Assert.IsTrue(a.UnorderedEqual(new HashSet<string>(StringComparer.Ordinal) { "a" }));
        var c = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "A" };
        Assert.IsFalse(a.UnorderedEqual(c));
    }

    [TestMethod]
    public void F10_RandomSinglePassAndListFastPath()
    {
        var source = new OnceEnumerable<int>([10, 20, 30]);
        var random = new ScriptedRandom(0, 1, 0);
        Assert.AreEqual(30, source.Random(random));
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, random.Bounds.ToArray());
        Assert.AreEqual(1, source.Enumerations);
        Assert.AreEqual(1, source.Disposals);
        random = new ScriptedRandom(1);
        Assert.AreEqual(20, ((IEnumerable<int>)new[] { 10, 20, 30 }).Random(random));
        CollectionAssert.AreEqual(new[] { 3 }, random.Bounds.ToArray());
        Assert.ThrowsExactly<InvalidOperationException>(() => Yield<int>().Random(random));
        Assert.ThrowsExactly<InvalidOperationException>(() => Array.Empty<int>().Random(random));
        Assert.AreEqual(7, Yield(7).Random(new ScriptedRandom(0)));
    }

    [TestMethod]
    public void F10_ReservoirChoicesAreUnbiased()
    {
        var counts = new int[3];
        for (var second = 0; second < 2; second++)
        for (var third = 0; third < 3; third++)
            counts[Yield(0, 1, 2).Random(new ScriptedRandom(0, second, third))]++;
        CollectionAssert.AreEqual(new[] { 2, 2, 2 }, counts);
    }

    [TestMethod]
    public void F10_ShuffleDoesNotMutateInputAndCapturesSnapshot()
    {
        var source = new List<int> { 1, 2 };
        var result = source.RandomOrder(new ScriptedRandom(0));
        CollectionAssert.AreEqual(new[] { 1, 2 }, source);
        source[0] = 9;
        CollectionAssert.AreEqual(new[] { 2, 1 }, result.ToArray());
        CollectionAssert.AreEqual(
            new[] { 1, 2 },
            Yield(1, 2).RandomOrder(new ScriptedRandom(1)).ToArray()
        );
    }

    [TestMethod]
    public void F10_FisherYatesAllThreeItemChoicesAndStableReplay()
    {
        var permutations = new HashSet<string>();
        for (var first = 0; first < 3; first++)
        for (var second = 0; second < 2; second++)
        {
            var source = new OnceEnumerable<int>([1, 2, 3]);
            var random = new ScriptedRandom(first, second);
            var shuffled = source.RandomOrder(random);
            var values = shuffled.ToArray();
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, values);
            CollectionAssert.AreEqual(values, shuffled.ToArray());
            CollectionAssert.AreEqual(new[] { 3, 2 }, random.Bounds.ToArray());
            Assert.AreEqual(1, source.Enumerations);
            Assert.AreEqual(1, source.Disposals);
            permutations.Add(string.Join(',', values));
        }
        Assert.AreEqual(6, permutations.Count);
        CollectionAssert.AreEqual(
            Array.Empty<int>(),
            Yield<int>().RandomOrder(new ScriptedRandom()).ToArray()
        );
        CollectionAssert.AreEqual(
            new[] { 7 },
            Yield(7).RandomOrder(new ScriptedRandom()).ToArray()
        );
        CollectionAssert.AreEquivalent(
            new string?[] { null, "a", "a" },
            Yield<string?>(null, "a", "a").RandomOrder(new ScriptedRandom(0, 0)).ToArray()
        );
    }

    [TestMethod]
    [DataRow("equal")]
    [DataRow("different")]
    [DataRow("shorter")]
    [DataRow("longer")]
    [DataRow("comparer")]
    [DataRow("second-enumerator")]
    [DataRow("move")]
    [DataRow("first-move")]
    public void F11_NonGenericEnumeratorDisposal(string scenario)
    {
        var first = new OnceEnumerable<int>(scenario == "first-move" ? ThrowAfterOne() : [1, 2]);
        var second = new OnceEnumerable<int>(
            scenario switch
            {
                "different" => [9, 2],
                "shorter" => [1],
                "longer" => [1, 2, 3],
                "move" => ThrowAfterOne(),
                _ => [1, 2],
            },
            scenario == "second-enumerator"
        );
        Func<object?, object?, bool> comparer =
            scenario == "comparer"
                ? (_, _) => throw new InvalidOperationException("Comparison failed.")
                : (x, y) => Equals(x, y);
        if (scenario is "comparer" or "second-enumerator" or "move" or "first-move")
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ((IEnumerable)first).SequenceEqual(second, comparer)
            );
        else
            Assert.AreEqual(
                scenario == "equal",
                ((IEnumerable)first).SequenceEqual(second, comparer)
            );
        Assert.AreEqual(1, first.Disposals);
        Assert.AreEqual(scenario == "second-enumerator" ? 0 : 1, second.Disposals);
    }

    [TestMethod]
    public void F11_ComparisonWithoutDisposableAndNullValues()
    {
        IEnumerable first = new ArrayList { null, "A" };
        IEnumerable second = new ArrayList { null, "a" };
        Assert.IsTrue(
            first.SequenceEqual(second, (x, y) => StringComparer.OrdinalIgnoreCase.Equals(x, y))
        );
        Assert.IsTrue(
            new NonDisposableSequence().SequenceEqual(new NonDisposableSequence(), Equals)
        );
    }

    [TestMethod]
    [DataRow("index")]
    [DataRow("equal")]
    [DataRow("random")]
    [DataRow("shuffle")]
    public void EnumerationFailureDisposes(string operation)
    {
        var source = new OnceEnumerable<int>(ThrowAfterOne());
        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            switch (operation)
            {
                case "index":
                    source.TryElementAt(2, out _);
                    break;
                case "equal":
                    source.UnorderedEqual(Yield(1));
                    break;
                case "random":
                    source.Random(new ScriptedRandom(0));
                    break;
                default:
                    source.RandomOrder(new ScriptedRandom()).ToArray();
                    break;
            }
        });
        Assert.AreEqual(1, source.Disposals);
    }

    [TestMethod]
    public void F12_StringRangeEndpointsAndDeferredValidation()
    {
        CollectionAssert.AreEqual(
            new[] { int.MaxValue.ToString() },
            Enumerable.StringRange(int.MaxValue, 1).ToArray()
        );
        CollectionAssert.AreEqual(
            new[] { (int.MaxValue - 1).ToString(), int.MaxValue.ToString() },
            Enumerable.StringRange(int.MaxValue - 1, 2).ToArray()
        );
        CollectionAssert.AreEqual(
            Array.Empty<string>(),
            Enumerable.StringRange(int.MaxValue, 0).ToArray()
        );
        CollectionAssert.AreEqual(
            new[] { int.MinValue.ToString(), (int.MinValue + 1).ToString() },
            Enumerable.StringRange(int.MinValue, 2).ToArray()
        );
        var negative = Enumerable.StringRange(0, -1);
        var overflow = Enumerable.StringRange(int.MaxValue, 2);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => negative.ToArray());
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => overflow.ToArray());
    }

    private static void CompareOutcome(Func<int> expected, Func<int> actual)
    {
        int expectedValue = 0;
        Exception? expectedError = null;
        try
        {
            expectedValue = expected();
        }
        catch (Exception error)
        {
            expectedError = error;
        }
        int actualValue = 0;
        Exception? actualError = null;
        try
        {
            actualValue = actual();
        }
        catch (Exception error)
        {
            actualError = error;
        }
        Assert.AreEqual(expectedError?.GetType(), actualError?.GetType());
        if (expectedError is null)
            Assert.AreEqual(expectedValue, actualValue);
    }

    private static IEnumerable<T> Yield<T>(params T[] values)
    {
        foreach (var value in values)
            yield return value;
    }

    private static IEnumerable<int> ThrowAfterOne()
    {
        yield return 1;
        throw new InvalidOperationException("Enumeration failed.");
    }

    private sealed class BoundedCollection() : Collection<int>(new List<int> { 1, 2, 3 })
    {
        protected override void InsertItem(int index, int item)
        {
            if (Count >= 12)
                throw new InvalidOperationException("Growth limit exceeded.");
            base.InsertItem(index, item);
        }
    }

    private sealed class NonDisposableSequence : IEnumerable
    {
        public IEnumerator GetEnumerator() => new Enumerator();

        private sealed class Enumerator : IEnumerator
        {
            private int _index;
            public object Current => _index;

            public bool MoveNext() => ++_index <= 2;

            public void Reset() => _index = 0;
        }
    }

    private sealed class ScriptedRandom(params int[] choices) : Random
    {
        private int _index;
        public List<int> Bounds { get; } = [];

        public override int Next(int maxValue)
        {
            Bounds.Add(maxValue);
            Assert.IsTrue(_index < choices.Length, "Unexpected random draw.");
            var value = choices[_index++];
            Assert.IsTrue(value >= 0 && value < maxValue);
            return value;
        }
    }

    private sealed class OnceEnumerable<T>(IEnumerable<T> values, bool failGetEnumerator = false)
        : IEnumerable<T>
    {
        public int Enumerations { get; private set; }
        public int Disposals { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            Enumerations++;
            if (failGetEnumerator || Enumerations > 1)
                throw new InvalidOperationException("Enumeration is unavailable.");
            return new TrackingEnumerator(values.GetEnumerator(), this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class TrackingEnumerator(IEnumerator<T> inner, OnceEnumerable<T> owner)
            : IEnumerator<T>
        {
            public T Current => inner.Current;
            object? IEnumerator.Current => Current;

            public bool MoveNext() => inner.MoveNext();

            public void Reset() => inner.Reset();

            public void Dispose()
            {
                owner.Disposals++;
                inner.Dispose();
            }
        }
    }
}
