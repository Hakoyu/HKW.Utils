using System.Collections;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public sealed class BidirectionalDictionaryRepairTests
{
    private static IBidirectionalDictionary<int, string> Create(
        int kind,
        IEnumerable<KeyValuePair<int, string>> pairs
    )
    {
        if (kind == 3)
        {
            var forward = new Dictionary<int, string>();
            var reverse = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in pairs)
            {
                forward.Add(pair.Key, pair.Value);
                reverse.Add(pair.Value, pair.Key);
            }
            return new BidirectionalDictionaryWrapper<
                int,
                string,
                Dictionary<int, string>,
                Dictionary<string, int>
            >(forward, reverse, null, StringComparer.OrdinalIgnoreCase);
        }
        return kind switch
        {
            0 => new BidirectionalDictionary<int, string>(
                pairs,
                null,
                StringComparer.OrdinalIgnoreCase
            ),
            1 => new ConcurrentBidirectionalDictionary<int, string>(
                pairs,
                null,
                StringComparer.OrdinalIgnoreCase
            ),
            _ => new FrozenBidirectionalDictionary<int, string>(
                pairs,
                null,
                StringComparer.OrdinalIgnoreCase
            ),
        };
    }

    private static void Invariant(IBidirectionalDictionary<int, string> dictionary)
    {
        Assert.AreEqual(dictionary.Count, dictionary.Dictionary2.Count);
        foreach (var pair in dictionary.Dictionary1)
            Assert.AreEqual(pair.Key, dictionary.Dictionary2[pair.Value]);
        foreach (var pair in dictionary.Dictionary2)
            Assert.AreEqual(
                pair.Key.ToUpperInvariant(),
                dictionary.Dictionary1[pair.Value].ToUpperInvariant()
            );
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void ConstructorConsumesOnceAndPreservesBijection(int kind)
    {
        var dictionary = Create(
            kind,
            new Once<KeyValuePair<int, string>>([new(1, "A"), new(2, "B")])
        );
        try
        {
            Invariant(dictionary);
            Assert.AreEqual(1, dictionary["a"]);
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void ConstructorRejectsDuplicates(int kind)
    {
        Assert.Throws<ArgumentException>(() => Create(kind, [new(1, "A"), new(2, "a")]));
        Assert.Throws<ArgumentException>(() => Create(kind, [new(1, "A"), new(1, "B")]));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    public void NullWritesLeaveBothSidesUnchanged(int kind)
    {
        var dictionary = Create(kind, [new(1, "A")]);
        try
        {
            Assert.Throws<ArgumentNullException>(() => dictionary.TryAdd(2, null!));
            Invariant(dictionary);
            Assert.Throws<ArgumentNullException>(() => dictionary.TrySetValue(1, null!));
            Invariant(dictionary);
            Assert.Throws<ArgumentNullException>(() => dictionary.TryAdd(null!, 2));
            Assert.AreEqual(1, dictionary.Count);
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    public void WrapperRejectsInconsistentAndAliasedBackends()
    {
        Assert.Throws<ArgumentException>(() =>
            new BidirectionalDictionaryWrapper<
                int,
                string,
                Dictionary<int, string>,
                Dictionary<string, int>
            >(new() { [1] = "A" }, new(), null, null)
        );
        var same = new Dictionary<int, int>();
        Assert.Throws<ArgumentException>(() =>
            new BidirectionalDictionaryWrapper<
                int,
                int,
                Dictionary<int, int>,
                Dictionary<int, int>
            >(same, same, null, null)
        );
    }

    private static IBidirectionalDictionary<string, string> CreateStrings(
        int kind,
        IEnumerable<KeyValuePair<string, string>> pairs
    )
    {
        if (kind == 3)
        {
            var forward = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var reverse = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in pairs)
            {
                forward.Add(pair.Key, pair.Value);
                reverse.Add(pair.Value, pair.Key);
            }
            return new BidirectionalDictionaryWrapper<
                string,
                string,
                Dictionary<string, string>,
                Dictionary<string, string>
            >(forward, reverse, null, null);
        }
        return kind switch
        {
            0 => new BidirectionalDictionary<string, string>(
                pairs,
                StringComparer.OrdinalIgnoreCase,
                StringComparer.OrdinalIgnoreCase
            ),
            1 => new ConcurrentBidirectionalDictionary<string, string>(
                pairs,
                StringComparer.OrdinalIgnoreCase,
                StringComparer.OrdinalIgnoreCase
            ),
            _ => new FrozenBidirectionalDictionary<string, string>(
                pairs,
                StringComparer.OrdinalIgnoreCase,
                StringComparer.OrdinalIgnoreCase
            ),
        };
    }

    private static void StringInvariant(IBidirectionalDictionary<string, string> dictionary)
    {
        Assert.AreEqual(dictionary.Count, dictionary.Dictionary2.Count);
        foreach (var pair in dictionary.Dictionary1)
            Assert.IsTrue(
                StringComparer.OrdinalIgnoreCase.Equals(
                    pair.Key,
                    dictionary.Dictionary2[pair.Value]
                )
            );
        foreach (var pair in dictionary.Dictionary2)
            Assert.IsTrue(
                StringComparer.OrdinalIgnoreCase.Equals(
                    pair.Key,
                    dictionary.Dictionary1[pair.Value]
                )
            );
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void SameTypeDirectionsAndBothComparers(int kind)
    {
        var dictionary = CreateStrings(
            kind,
            new Once<KeyValuePair<string, string>>([new("A", "X"), new("B", "Y")])
        );
        try
        {
            Assert.AreEqual("X", dictionary.GetForward("a"));
            Assert.AreEqual("A", dictionary.GetReverse("x"));
            Assert.IsTrue(dictionary.TryGetForward("a", out var value));
            Assert.AreEqual("X", value);
            Assert.IsTrue(dictionary.TryGetReverse("x", out var key));
            Assert.AreEqual("A", key);
            Assert.IsTrue(dictionary.ContainsKeyForward("a"));
            Assert.IsTrue(dictionary.ContainsKeyReverse("x"));
            Assert.IsTrue(dictionary.ContainsForward(new("a", "x")));
            Assert.IsTrue(dictionary.ContainsReverse(new("x", "a")));
            var forward = new KeyValuePair<string, string>[2];
            var reverse = new KeyValuePair<string, string>[2];
            dictionary.CopyToForward(forward, 0);
            dictionary.CopyToReverse(reverse, 0);
            Assert.AreEqual(2, forward.Length);
            Assert.IsTrue(reverse.Contains(new("X", "A")));
            IReadOnlyBidirectionalDictionary<string, string> readOnly =
                (IReadOnlyBidirectionalDictionary<string, string>)dictionary;
            Assert.AreEqual("X", readOnly.GetForward("a"));
            Assert.AreEqual("A", readOnly.GetReverse("x"));
            Assert.Throws<NotSupportedException>(() =>
                ((IDictionary<string, string>)dictionary.Dictionary1).Add("C", "Z")
            );
            StringInvariant(dictionary);
            if (kind == 2)
            {
                Assert.Throws<NotSupportedException>(() => dictionary.TryAddForward("C", "Z"));
                Assert.Throws<NotSupportedException>(() => dictionary.TryAddReverse("Z", "C"));
                Assert.Throws<NotSupportedException>(() => dictionary.TrySetForward("A", "Z"));
                Assert.Throws<NotSupportedException>(() => dictionary.TrySetReverse("X", "C"));
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveForward("A"));
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveReverse("X"));
                Assert.Throws<NotSupportedException>(() =>
                    dictionary.RemoveForward(new KeyValuePair<string, string>("A", "X"))
                );
                Assert.Throws<NotSupportedException>(() =>
                    dictionary.RemoveReverse(new KeyValuePair<string, string>("X", "A"))
                );
                Assert.Throws<NotSupportedException>(() => dictionary.Clear());
            }
            else
            {
                Assert.IsFalse(dictionary.TryAddForward("a", "Z"));
                Assert.IsFalse(dictionary.TryAddReverse("x", "C"));
                Assert.IsFalse(dictionary.TrySetForward("A", "y"));
                Assert.IsFalse(dictionary.TrySetReverse("X", "b"));
                StringInvariant(dictionary);
                Assert.IsTrue(dictionary.TrySetForward("a", "Z"));
                Assert.AreEqual("A", dictionary.GetReverse("z"));
                StringInvariant(dictionary);
                Assert.IsTrue(dictionary.TrySetReverse("z", "C"));
                StringInvariant(dictionary);
                Assert.IsFalse(
                    dictionary.RemoveForward(new KeyValuePair<string, string>("C", "Y"))
                );
                Assert.IsTrue(dictionary.RemoveReverse(new KeyValuePair<string, string>("z", "c")));
                Assert.IsTrue(dictionary.TryAddReverse("Q", "D"));
                Assert.IsTrue(dictionary.RemoveForward("d"));
                Assert.IsTrue(dictionary.TryAddForward("E", "R"));
                Assert.IsTrue(dictionary.RemoveForward(new KeyValuePair<string, string>("e", "r")));
                Assert.IsFalse(dictionary.RemoveReverse("missing"));
                StringInvariant(dictionary);
                dictionary.Clear();
                Assert.AreEqual(0, dictionary.Count);
            }
            StringInvariant(dictionary);
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
        Assert.Throws<ArgumentException>(() => CreateStrings(kind, [new("A", "X"), new("a", "Y")]));
        Assert.Throws<ArgumentException>(() => CreateStrings(kind, [new("A", "X"), new("B", "x")]));
        Assert.Throws<ArgumentNullException>(() => CreateStrings(kind, [new(null!, "X")]));
        Assert.Throws<ArgumentNullException>(() => CreateStrings(kind, [new("A", null!)]));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    public void AllNullMutationArgumentsArePrevalidated(int kind)
    {
        var dictionary = CreateStrings(kind, [new("A", "X")]);
        try
        {
            Action[] operations =
            [
                () => dictionary.TryAddForward(null!, "Y"),
                () => dictionary.TryAddForward("B", null!),
                () => dictionary.TryAddReverse(null!, "B"),
                () => dictionary.TryAddReverse("Y", null!),
                () => dictionary.TrySetForward(null!, "Y"),
                () => dictionary.TrySetForward("A", null!),
                () => dictionary.TrySetReverse(null!, "B"),
                () => dictionary.TrySetReverse("X", null!),
                () => dictionary.RemoveForward((string)null!),
                () => dictionary.RemoveReverse((string)null!),
                () => dictionary.RemoveForward(new KeyValuePair<string, string>("A", null!)),
                () => dictionary.RemoveReverse(new KeyValuePair<string, string>("X", null!)),
            ];
            foreach (var operation in operations)
            {
                Assert.Throws<ArgumentNullException>(operation);
                Assert.AreEqual(1, dictionary.Count);
                Assert.AreEqual("X", dictionary.GetForward("A"));
                StringInvariant(dictionary);
            }
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    [DataRow("Add", false)]
    [DataRow("Add", true)]
    [DataRow("Set", false)]
    [DataRow("Set", true)]
    [DataRow("Remove", false)]
    [DataRow("Remove", true)]
    [DataRow("RemovePair", false)]
    [DataRow("RemovePair", true)]
    [DataRow("Clear", false)]
    [DataRow("Clear", true)]
    public void CustomBackendControlledFailureRollsBack(string operation, bool reverseDirection)
    {
        var forward = new FaultDictionary<string, string>();
        var reverse = new FaultDictionary<string, string>();
        forward.Add("A", "X");
        reverse.Add("X", "A");
        var dictionary = new BidirectionalDictionaryWrapper<
            string,
            string,
            FaultDictionary<string, string>,
            FaultDictionary<string, string>
        >(forward, reverse, StringComparer.Ordinal, StringComparer.Ordinal);
        var first = reverseDirection ? reverse : forward;
        var second = reverseDirection ? forward : reverse;
        switch (operation)
        {
            case "Add":
                second.FailNext = "Add";
                break;
            case "Set":
                first.FailNext = "Set";
                break;
            case "Remove":
            case "RemovePair":
                first.FailNext = "Remove";
                break;
            case "Clear":
                reverse.FailNext = "Clear";
                break;
        }
        Assert.Throws<InvalidOperationException>(() =>
        {
            switch (operation)
            {
                case "Add":
                    if (reverseDirection)
                        dictionary.TryAddReverse("Y", "B");
                    else
                        dictionary.TryAddForward("B", "Y");
                    break;
                case "Set":
                    if (reverseDirection)
                        dictionary.TrySetReverse("X", "B");
                    else
                        dictionary.TrySetForward("A", "Y");
                    break;
                case "Remove":
                    if (reverseDirection)
                        dictionary.RemoveReverse("X");
                    else
                        dictionary.RemoveForward("A");
                    break;
                case "RemovePair":
                    if (reverseDirection)
                        dictionary.RemoveReverse(new KeyValuePair<string, string>("X", "A"));
                    else
                        dictionary.RemoveForward(new KeyValuePair<string, string>("A", "X"));
                    break;
                case "Clear":
                    dictionary.Clear();
                    break;
            }
        });
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual("X", dictionary.GetForward("A"));
        StringInvariant(dictionary);
        Assert.IsTrue(dictionary.TryAddForward("C", "Z"));
        StringInvariant(dictionary);
    }

    [TestMethod]
    [DataRow("Add", false)]
    [DataRow("Add", true)]
    [DataRow("Remove", false)]
    [DataRow("Remove", true)]
    public void ReplacementFailuresKeepOriginalPair(string operation, bool reverseDirection)
    {
        var forward = new FaultDictionary<string, string>();
        var reverse = new FaultDictionary<string, string>();
        forward.Add("A", "X");
        reverse.Add("X", "A");
        var dictionary = new BidirectionalDictionaryWrapper<
            string,
            string,
            FaultDictionary<string, string>,
            FaultDictionary<string, string>
        >(forward, reverse, StringComparer.Ordinal, StringComparer.Ordinal);
        (reverseDirection ? forward : reverse).FailNext = operation;
        Assert.Throws<InvalidOperationException>(() =>
        {
            if (reverseDirection)
                dictionary.TrySetReverse("X", "B");
            else
                dictionary.TrySetForward("A", "Y");
        });
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual("X", dictionary.GetForward("A"));
        StringInvariant(dictionary);
    }

    [TestMethod]
    public void RollbackFailureReportsBothErrorsWithoutRetrying()
    {
        var forward = new FaultDictionary<string, string>();
        var reverse = new FaultDictionary<string, string>();
        var dictionary = new BidirectionalDictionaryWrapper<
            string,
            string,
            FaultDictionary<string, string>,
            FaultDictionary<string, string>
        >(forward, reverse, StringComparer.Ordinal, StringComparer.Ordinal);
        forward.FailNext = "Remove";
        reverse.FailNext = "Add";
        var error = Assert.Throws<AggregateException>(() => dictionary.TryAddForward("A", "X"));
        Assert.AreEqual(2, error.InnerExceptions.Count);
        Assert.IsTrue(
            error.InnerExceptions.All(exception => exception is InvalidOperationException)
        );
    }

    [TestMethod]
    public void ComparerReentryReleasesConcurrentLock()
    {
        var comparer = new ReentrantComparer();
        using var dictionary = new ConcurrentBidirectionalDictionary<string, string>(
            StringComparer.Ordinal,
            comparer
        );
        comparer.Read = () => dictionary.Count;
        Assert.Throws<LockRecursionException>(() => dictionary.TryAddForward("A", "X"));
        comparer.Read = null;
        Assert.AreEqual(0, dictionary.Count);
        Assert.IsTrue(dictionary.TryAddForward("A", "X"));
        StringInvariant(dictionary);
    }

    [TestMethod]
    public void WrapperValidatesCapabilitiesAndComparerContract()
    {
        var first = new FaultDictionary<string, string>();
        var second = new FaultDictionary<string, string>();
        second.ReadOnly = true;
        Assert.Throws<NotSupportedException>(() =>
            new BidirectionalDictionaryWrapper<
                string,
                string,
                FaultDictionary<string, string>,
                FaultDictionary<string, string>
            >(first, second, StringComparer.Ordinal, StringComparer.Ordinal)
        );
        second.ReadOnly = false;
        Assert.Throws<ArgumentException>(() =>
            new BidirectionalDictionaryWrapper<
                string,
                string,
                FaultDictionary<string, string>,
                FaultDictionary<string, string>
            >(first, second, null, null)
        );
        Assert.Throws<ArgumentException>(() =>
            new BidirectionalDictionaryWrapper<
                string,
                string,
                Dictionary<string, string>,
                Dictionary<string, string>
            >(new(StringComparer.OrdinalIgnoreCase), new(), StringComparer.Ordinal, null)
        );
        first.Add("A", "X");
        second.Add("X", "B");
        Assert.Throws<ArgumentException>(() =>
            new BidirectionalDictionaryWrapper<
                string,
                string,
                FaultDictionary<string, string>,
                FaultDictionary<string, string>
            >(first, second, StringComparer.Ordinal, StringComparer.Ordinal)
        );
        second["X"] = "A";
        var dictionary = new BidirectionalDictionaryWrapper<
            string,
            string,
            FaultDictionary<string, string>,
            FaultDictionary<string, string>
        >(first, second, StringComparer.Ordinal, StringComparer.Ordinal);
        second.ReadOnly = true;
        Assert.IsTrue(dictionary.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => dictionary.TryAddForward("B", "Y"));
        Assert.Throws<NotSupportedException>(() => dictionary.TrySetForward("A", "Y"));
        Assert.Throws<NotSupportedException>(() => dictionary.RemoveForward("A"));
        Assert.Throws<NotSupportedException>(() => dictionary.Clear());
        StringInvariant(dictionary);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(1, false)]
    [DataRow(0, true)]
    [DataRow(1, true)]
    public void OneShotComparerFailureDoesNotLeavePartialAdd(int kind, bool reverseDirection)
    {
        var comparer = new OneShotComparer();
        IBidirectionalDictionary<string, string> dictionary =
            kind == 0
                ? new BidirectionalDictionary<string, string>(
                    reverseDirection ? comparer : StringComparer.Ordinal,
                    reverseDirection ? StringComparer.Ordinal : comparer
                )
                : new ConcurrentBidirectionalDictionary<string, string>(
                    reverseDirection ? comparer : StringComparer.Ordinal,
                    reverseDirection ? StringComparer.Ordinal : comparer
                );
        try
        {
            dictionary.TryAddForward("A", "X");
            comparer.Remaining = 2;
            Assert.Throws<InvalidOperationException>(() =>
            {
                if (reverseDirection)
                    dictionary.TryAddReverse("Y", "B");
                else
                    dictionary.TryAddForward("B", "Y");
            });
            Assert.AreEqual(1, dictionary.Count);
            StringInvariant(dictionary);
            Assert.IsTrue(dictionary.TryAddForward("B", "Y"));
            StringInvariant(dictionary);
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    [DataRow(0, false, "SetPrimary")]
    [DataRow(0, true, "SetPrimary")]
    [DataRow(1, false, "SetPrimary")]
    [DataRow(1, true, "SetPrimary")]
    [DataRow(0, false, "SetSecondary")]
    [DataRow(0, true, "SetSecondary")]
    [DataRow(1, false, "SetSecondary")]
    [DataRow(1, true, "SetSecondary")]
    [DataRow(0, false, "Remove")]
    [DataRow(0, true, "Remove")]
    [DataRow(1, false, "Remove")]
    [DataRow(1, true, "Remove")]
    public void OneShotComparerFailureRollsBackReplacementAndRemoval(
        int kind,
        bool reverseDirection,
        string operation
    )
    {
        var comparer = new OneShotComparer();
        var firstSide = operation != "SetSecondary";
        var comparerOnForward = firstSide != reverseDirection;
        IBidirectionalDictionary<string, string> dictionary =
            kind == 0
                ? new BidirectionalDictionary<string, string>(
                    comparerOnForward ? comparer : StringComparer.Ordinal,
                    comparerOnForward ? StringComparer.Ordinal : comparer
                )
                : new ConcurrentBidirectionalDictionary<string, string>(
                    comparerOnForward ? comparer : StringComparer.Ordinal,
                    comparerOnForward ? StringComparer.Ordinal : comparer
                );
        try
        {
            dictionary.TryAddForward("A", "X");
            comparer.Remaining = firstSide ? 2 : 4;
            Assert.Throws<InvalidOperationException>(() =>
            {
                if (operation == "Remove")
                {
                    if (reverseDirection)
                        dictionary.RemoveReverse("X");
                    else
                        dictionary.RemoveForward("A");
                }
                else
                {
                    if (reverseDirection)
                        dictionary.TrySetReverse("X", "B");
                    else
                        dictionary.TrySetForward("A", "Y");
                }
            });
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("X", dictionary.GetForward("A"));
            StringInvariant(dictionary);
            Assert.IsTrue(dictionary.TryAddForward("C", "Z"));
            StringInvariant(dictionary);
        }
        finally
        {
            (dictionary as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    public void ConcreteSameTypeApisAreUnambiguous()
    {
        var ordinary = new BidirectionalDictionary<string, string>();
        Assert.IsTrue(ordinary.TryAddForward("A", "X"));
        Assert.AreEqual("A", ordinary.GetReverse("X"));
        using var concurrent = new ConcurrentBidirectionalDictionary<string, string>();
        Assert.IsTrue(concurrent.TryAddReverse("X", "A"));
        Assert.AreEqual("X", concurrent.GetForward("A"));
        var frozen = new FrozenBidirectionalDictionary<string, string>([new("A", "X")]);
        Assert.AreEqual("A", frozen.GetReverse("X"));
        Assert.Throws<NotSupportedException>(() => frozen.TrySetForward("A", "Y"));
    }

    [TestMethod]
    public void ConcurrentSnapshotsAndFiniteBarrierOperations()
    {
        using var dictionary = new ConcurrentBidirectionalDictionary<int, string>();
        dictionary.TryAdd(1000, "seed");
        var forward = dictionary.Dictionary1;
        var reverse = dictionary.Dictionary2;
        var keys = dictionary.Keys;
        var values = dictionary.Values;
        using var enumerator = dictionary.GetEnumerator();
        dictionary.Clear();
        using var barrier = new Barrier(4);
        var tasks = Enumerable
            .Range(0, 4)
            .Select(worker =>
                Task.Run(() =>
                {
                    Assert.IsTrue(barrier.SignalAndWait(TimeSpan.FromSeconds(10)));
                    for (var round = 0; round < 100; round++)
                    {
                        var key = worker * 100 + round;
                        Assert.IsTrue(dictionary.TryAddForward(key, key.ToString()));
                        Assert.IsTrue(dictionary.TrySetForward(key, $"value-{key}"));
                        var snapshot = dictionary.GetSnapshot();
                        Assert.AreEqual(
                            snapshot.Length,
                            snapshot.Select(pair => pair.Key).Distinct().Count()
                        );
                        Assert.AreEqual(
                            snapshot.Length,
                            snapshot.Select(pair => pair.Value).Distinct().Count()
                        );
                        foreach (var pair in snapshot)
                            Assert.IsTrue(
                                pair.Value == pair.Key.ToString()
                                    || pair.Value == $"value-{pair.Key}"
                            );
                        Assert.IsTrue(dictionary.RemoveReverse($"value-{key}"));
                    }
                    Assert.IsTrue(barrier.SignalAndWait(TimeSpan.FromSeconds(10)));
                })
            )
            .ToArray();
        Assert.IsTrue(Task.WaitAll(tasks, TimeSpan.FromSeconds(30)));
        Invariant(dictionary);
        Assert.AreEqual(0, dictionary.Count);
        Assert.AreEqual("seed", forward[1000]);
        Assert.AreEqual(1000, reverse["seed"]);
        CollectionAssert.AreEqual(new[] { 1000 }, keys.ToArray());
        CollectionAssert.AreEqual(new[] { "seed" }, values.ToArray());
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(new KeyValuePair<int, string>(1000, "seed"), enumerator.Current);
        Assert.IsFalse(enumerator.MoveNext());
        dictionary.Dispose();
        dictionary.Dispose();
        Assert.Throws<ObjectDisposedException>(() => dictionary.GetSnapshot());
        Assert.Throws<ObjectDisposedException>(() => dictionary.TryAdd(1, "one"));
        Assert.Throws<ObjectDisposedException>(() => dictionary.GetForward(1));
        Assert.AreEqual("seed", forward[1000]);
    }

    [TestMethod]
    public void ConcurrentConflictingWritesHaveOneWinner()
    {
        using var dictionary = new ConcurrentBidirectionalDictionary<int, string>();
        using var barrier = new Barrier(4);
        var wins = 0;
        var tasks = Enumerable
            .Range(0, 4)
            .Select(worker =>
                Task.Run(() =>
                {
                    for (var round = 0; round < 50; round++)
                    {
                        Assert.IsTrue(barrier.SignalAndWait(TimeSpan.FromSeconds(10)));
                        if (dictionary.TryAddForward(round, $"{round}-{worker}"))
                            Interlocked.Increment(ref wins);
                        Assert.IsTrue(barrier.SignalAndWait(TimeSpan.FromSeconds(10)));
                    }
                })
            )
            .ToArray();
        Assert.IsTrue(Task.WaitAll(tasks, TimeSpan.FromSeconds(30)));
        Assert.AreEqual(50, wins);
        Assert.AreEqual(50, dictionary.Count);
        Invariant(dictionary);
    }

    private sealed class ReentrantComparer : IEqualityComparer<string>
    {
        public Func<int>? Read { get; set; }

        public bool Equals(string? x, string? y) => StringComparer.Ordinal.Equals(x, y);

        public int GetHashCode(string value)
        {
            Read?.Invoke();
            return StringComparer.Ordinal.GetHashCode(value);
        }
    }

    private sealed class OneShotComparer : IEqualityComparer<string>
    {
        public int Remaining { get; set; }

        public bool Equals(string? x, string? y) => StringComparer.Ordinal.Equals(x, y);

        public int GetHashCode(string value)
        {
            if (Remaining > 0 && --Remaining == 0)
                throw new InvalidOperationException("Controlled comparer failure.");
            return StringComparer.Ordinal.GetHashCode(value);
        }
    }

    private sealed class FaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _items = new();
        public string? FailNext { get; set; }
        public bool ReadOnly { get; set; }

        private void Check(string operation)
        {
            if (ReadOnly)
                throw new NotSupportedException("The backend is read-only.");
            if (FailNext == operation)
            {
                FailNext = null;
                throw new InvalidOperationException("Controlled backend failure.");
            }
        }

        public TValue this[TKey key]
        {
            get => _items[key];
            set
            {
                Check("Set");
                _items[key] = value;
            }
        }
        public ICollection<TKey> Keys => _items.Keys;
        public ICollection<TValue> Values => _items.Values;
        public int Count => _items.Count;
        public bool IsReadOnly => ReadOnly;

        public void Add(TKey key, TValue value)
        {
            Check("Add");
            _items.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            Check("Clear");
            _items.Clear();
        }

        public bool Remove(TKey key)
        {
            Check("Remove");
            return _items.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            Check("Remove");
            return ((ICollection<KeyValuePair<TKey, TValue>>)_items).Remove(item);
        }

        public bool ContainsKey(TKey key) => _items.ContainsKey(key);

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_items).Contains(item);

        public bool TryGetValue(
            TKey key,
            [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value
        ) => _items.TryGetValue(key, out value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_items).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class Once<T>(IEnumerable<T> source) : IEnumerable<T>
    {
        private bool _enumerated;

        public IEnumerator<T> GetEnumerator()
        {
            if (_enumerated)
                throw new InvalidOperationException("The source can only be enumerated once.");
            _enumerated = true;
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
