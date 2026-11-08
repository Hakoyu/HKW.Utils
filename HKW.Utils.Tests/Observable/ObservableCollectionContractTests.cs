using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public sealed class ObservableCollectionContractTests
{
    private sealed class Fixture : IDisposable
    {
        public required Action Add { get; init; }
        public required Action Clear { get; init; }
        public required Action NoOp { get; init; }
        public required Func<string[]> Read { get; init; }
        public required Action<Action> Before { get; init; }
        public required Action<Action> After { get; init; }
        public required INotifyCollectionChanged Collection { get; init; }
        public required INotifyPropertyChanged Properties { get; init; }
        public required IDisposable View { get; init; }
        public required Action<Action> ViewBefore { get; init; }
        public required Action<Action> ViewAfter { get; init; }

        public void Dispose() => View.Dispose();
    }

    private static Fixture Create(int kind, bool wrapper)
    {
        if (kind == 0)
        {
            IObservableList<string> source = wrapper
                ? new ObservableListWrapper<string, List<string>>(["a"])
                : new ObservableList<string>(["a"]);
            var view = new ReadOnlyObservableList<string>(source);
            return new()
            {
                Add = () => source.Add("b"),
                Clear = source.Clear,
                NoOp = () =>
                {
                    source[0] = "a";
                    source.Remove("missing");
                },
                Read = () => source.ToArray(),
                Before = a => source.ListChanging += (_, _) => a(),
                After = a => source.ListChanged += (_, _) => a(),
                Collection = source,
                Properties = source,
                View = view,
                ViewBefore = a => view.ListChanging += (_, _) => a(),
                ViewAfter = a => view.ListChanged += (_, _) => a(),
            };
        }
        if (kind == 1)
        {
            IObservableDictionary<string, string> source = wrapper
                ? new ObservableDictionaryWrapper<string, string, Dictionary<string, string>>(
                    new() { ["a"] = "a" },
                    null
                )
                : new ObservableDictionary<string, string>([new("a", "a")]);
            var view = new ReadOnlyObservableDictionary<string, string>(source);
            return new()
            {
                Add = () => source.Add("b", "b"),
                Clear = source.Clear,
                NoOp = () =>
                {
                    source["a"] = "a";
                    source.Remove("missing");
                    source.TryAdd("a", "x");
                },
                Read = () => source.Keys.ToArray(),
                Before = a => source.DictionaryChanging += (_, _) => a(),
                After = a => source.DictionaryChanged += (_, _) => a(),
                Collection = source,
                Properties = source,
                View = view,
                ViewBefore = a => view.DictionaryChanging += (_, _) => a(),
                ViewAfter = a => view.DictionaryChanged += (_, _) => a(),
            };
        }
        IObservableSet<string> set = wrapper
            ? new ObservableSetWrapper<string, OrderedHashSet<string>>(new(["a"]), null)
            : new ObservableSet<string>(["a"]);
        var readOnly = new ReadOnlyObservableSet<string>(set);
        return new()
        {
            Add = () => set.Add("b"),
            Clear = set.Clear,
            NoOp = () =>
            {
                set.Add("a");
                set.Remove("missing");
                set.UnionWith(["a"]);
                set.IntersectWith(["a"]);
                set.ExceptWith([]);
                set.SymmetricExceptWith([]);
            },
            Read = () => set.ToArray(),
            Before = a => set.SetChanging += (_, _) => a(),
            After = a => set.SetChanged += (_, _) => a(),
            Collection = set,
            Properties = set,
            View = readOnly,
            ViewBefore = a => readOnly.SetChanging += (_, _) => a(),
            ViewAfter = a => readOnly.SetChanged += (_, _) => a(),
        };
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void OperationObservesBeforeAndFinalStateAndRejectsNestedWrites(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        var before = 0;
        var after = 0;
        f.Before(() =>
        {
            before++;
            if (before > 1)
                return;
            CollectionAssert.AreEqual(new[] { "a" }, f.Read());
            Assert.ThrowsExactly<InvalidOperationException>(f.Clear);
            Assert.ThrowsExactly<InvalidOperationException>(f.NoOp);
        });
        f.After(() =>
        {
            after++;
            if (after > 1)
                return;
            CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
            Assert.ThrowsExactly<InvalidOperationException>(f.Clear);
            Assert.ThrowsExactly<InvalidOperationException>(f.NoOp);
        });
        f.Add();
        Assert.AreEqual(1, before);
        Assert.AreEqual(1, after);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void NoOpsHaveNoEventsAndEmptyClearIsSilent(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        var events = 0;
        f.Before(() => events++);
        f.After(() => events++);
        f.Collection.CollectionChanged += (_, _) => events++;
        f.Properties.PropertyChanged += (_, _) => events++;
        f.NoOp();
        Assert.AreEqual(0, events);
        f.Clear();
        events = 0;
        f.Clear();
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void ExceptionsAndCancellationNeverLeavePartialState(int kind, bool wrapper)
    {
        foreach (var cancel in new[] { false, true })
        {
            using var f = Create(kind, wrapper);
            var reject = true;
            var changed = 0;
            f.Before(() =>
            {
                if (reject)
                {
                    if (cancel)
                        throw new OperationCanceledException();
                    throw new ArgumentException("Rejected.");
                }
            });
            f.After(() => changed++);
            Assert.Throws<Exception>(f.Add);
            CollectionAssert.AreEqual(new[] { "a" }, f.Read());
            Assert.AreEqual(0, changed);
            reject = false;
            f.Add();
            CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
        }
        using var g = Create(kind, wrapper);
        var fail = true;
        g.After(() =>
        {
            if (fail)
                throw new ArgumentException("Notification failed.");
        });
        Assert.ThrowsExactly<ArgumentException>(g.Add);
        CollectionAssert.AreEqual(new[] { "a", "b" }, g.Read());
        fail = false;
        g.Clear();
        Assert.IsEmpty(g.Read());
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void EntireOperationFreezesSourceAndReadOnlySubscriptions(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        var oldCalls = 0;
        var newCalls = 0;
        var viewCalls = 0;
        var propertyCalls = 0;
        NotifyCollectionChangedEventHandler old = (_, _) => oldCalls++;
        NotifyCollectionChangedEventHandler next = (_, _) => newCalls++;
        PropertyChangedEventHandler property = (_, _) => propertyCalls++;
        f.Collection.CollectionChanged += old;
        f.Before(() =>
        {
            f.Collection.CollectionChanged -= old;
            f.Collection.CollectionChanged += next;
            f.Properties.PropertyChanged += property;
            f.ViewAfter(() => viewCalls++);
        });
        f.Add();
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, newCalls);
        Assert.AreEqual(0, viewCalls);
        Assert.AreEqual(0, propertyCalls);
        f.Clear();
        Assert.AreEqual(1, newCalls);
        Assert.AreEqual(1, viewCalls);
        Assert.IsGreaterThan(0, propertyCalls);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void ReadOnlyDisposeStopsForwardingButKeepsReadAccess(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        var count = 0;
        f.ViewBefore(() => count++);
        f.ViewAfter(() => count++);
        ((INotifyCollectionChanged)f.View).CollectionChanged += (_, _) => count++;
        ((INotifyPropertyChanged)f.View).PropertyChanged += (_, _) => count++;
        f.View.Dispose();
        f.View.Dispose();
        f.Add();
        Assert.AreEqual(0, count);
        CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
    }

    [TestMethod]
    public void ListTailInsertionAndStructuralIndexerNotifications()
    {
        var list = new ObservableList<int>();
        var names = new List<string?>();
        list.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        list.Insert(0, 1);
        list.Insert(list.Count, 2);
        list.RemoveAt(0);
        list.Clear();
        Assert.AreEqual(4, names.Count(n => n == "Item[]"));
        Assert.AreEqual(4, names.Count(n => n == "Count"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.Insert(1, 3));
    }

    [TestMethod]
    public void DictionaryUsesStoredKeysAndComparerForAllPayloads()
    {
        var source = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["A"] = 1,
            ["B"] = 2,
        };
        var dictionary = new ObservableDictionaryWrapper<string, int, Dictionary<string, int>>(
            source,
            null
        );
        using var view = new ReadOnlyObservableDictionary<string, int>(dictionary);
        Assert.IsFalse(((ICollection<KeyValuePair<string, int>>)view).Contains(new("a", 9)));
        Assert.IsTrue(((ICollection<KeyValuePair<string, int>>)view).Contains(new("a", 1)));
        var events = new List<NotifyCollectionChangedEventArgs>();
        dictionary.CollectionChanged += (_, e) => events.Add(e);
        dictionary["b"] = 3;
        dictionary.Remove("a");
        Assert.AreEqual("B", ((KeyValuePair<string, int>)events[0].NewItems![0]!).Key);
        Assert.AreEqual(1, events[0].NewStartingIndex);
        Assert.AreEqual("A", ((KeyValuePair<string, int>)events[1].OldItems![0]!).Key);
        Assert.AreEqual(0, events[1].OldStartingIndex);
    }

    [TestMethod]
    public void SetBatchFreezesInputAndReplaysIndices()
    {
        var set = new ObservableSet<string>(["A", "B", "C", "D"], StringComparer.OrdinalIgnoreCase);
        var input = new List<string> { "b", "d", "E", "e" };
        var mirror = set.ToList();
        NotifySetChangeEventArgs<string>? captured = null;
        set.SetChanging += (_, e) =>
        {
            captured = e;
            input.Clear();
        };
        set.CollectionChanged += (_, e) =>
        {
            CollectionAssert.AreEqual(new[] { "A", "C", "E" }, set.ToArray());
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Assert.AreEqual(mirror[e.OldStartingIndex], e.OldItems![0]);
                mirror.RemoveAt(e.OldStartingIndex);
            }
            else
                mirror.Insert(e.NewStartingIndex, (string)e.NewItems![0]!);
        };
        set.SymmetricExceptWith(input);
        CollectionAssert.AreEqual(new[] { "A", "C", "E" }, mirror);
        CollectionAssert.AreEqual(new[] { "b", "d", "E", "e" }, captured!.OtherItems!.ToArray());
    }

    [TestMethod]
    public void SetLateCollectionSubscriberStartsNextOperation()
    {
        var set = new ObservableSet<int>([1, 2, 3]);
        var calls = 0;
        set.SetChanged += (_, _) => set.CollectionChanged += (_, _) => calls++;
        set.ExceptWith([1, 3]);
        Assert.AreEqual(0, calls);
        set.Clear();
        Assert.AreEqual(1, calls);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void ChangedAndReadOnlyCallbacksFreezeUnsubscriptionsAndRejectWrites(
        int kind,
        bool wrapper
    )
    {
        using var f = Create(kind, wrapper);
        var sourceCalls = 0;
        var viewCalls = 0;
        var newCalls = 0;
        NotifyCollectionChangedEventHandler source = (_, _) => sourceCalls++;
        NotifyCollectionChangedEventHandler view = (_, _) => viewCalls++;
        f.Collection.CollectionChanged += source;
        var readOnly = (INotifyCollectionChanged)f.View;
        readOnly.CollectionChanged += view;
        f.ViewBefore(() =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(f.Clear);
            readOnly.CollectionChanged -= view;
        });
        f.ViewAfter(() =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(f.Clear);
            readOnly.CollectionChanged += (_, _) => newCalls++;
        });
        f.After(() => f.Collection.CollectionChanged -= source);
        f.Add();
        Assert.AreEqual(1, sourceCalls);
        Assert.AreEqual(1, viewCalls);
        Assert.AreEqual(0, newCalls);
        f.Clear();
        Assert.AreEqual(1, sourceCalls);
        Assert.AreEqual(1, viewCalls);
        Assert.AreEqual(1, newCalls);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void StandardNotificationFailuresKeepFinalStateAndReleaseGuard(int kind, bool wrapper)
    {
        foreach (var property in new[] { false, true })
        {
            using var f = Create(kind, wrapper);
            var fail = true;
            Action callback = () =>
            {
                Assert.ThrowsExactly<InvalidOperationException>(f.NoOp);
                if (fail)
                    throw new ApplicationException("Notification failed.");
            };
            if (property)
                f.Properties.PropertyChanged += (_, _) => callback();
            else
                f.Collection.CollectionChanged += (_, _) => callback();
            Assert.ThrowsExactly<ApplicationException>(f.Add);
            CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
            fail = false;
            f.Clear();
            Assert.IsEmpty(f.Read());
        }
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void ReadOnlyCallbackFailuresPreserveTransactionBoundary(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        var cancel = true;
        f.ViewBefore(() =>
        {
            if (cancel)
                throw new OperationCanceledException();
        });
        Assert.ThrowsExactly<OperationCanceledException>(f.Add);
        CollectionAssert.AreEqual(new[] { "a" }, f.Read());
        cancel = false;
        f.ViewAfter(() => throw new ApplicationException("Notification failed."));
        Assert.ThrowsExactly<ApplicationException>(f.Add);
        CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void DisposedReadOnlyViewsCanBeCollectedWhileSourceLives(int kind, bool wrapper)
    {
        var (source, weak) = CreateDisposedView(kind, wrapper);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.IsFalse(weak.IsAlive);
        GC.KeepAlive(source);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (object Source, WeakReference View) CreateDisposedView(int kind, bool wrapper)
    {
        var f = Create(kind, wrapper);
        var weak = new WeakReference(f.View);
        f.View.Dispose();
        return (f.Collection, weak);
    }

    [TestMethod]
    [DataRow(false, 0)]
    [DataRow(false, 1)]
    [DataRow(false, 2)]
    [DataRow(false, 3)]
    [DataRow(true, 0)]
    [DataRow(true, 1)]
    [DataRow(true, 2)]
    [DataRow(true, 3)]
    public void AllSetBatchesReplayAndRemainAtomicUnderNotificationFailures(
        bool wrapper,
        int action
    )
    {
        IObservableSet<string> CreateSet() =>
            wrapper
                ? new ObservableSetWrapper<string, OrderedHashSet<string>>(
                    new(["A", "B", "C", "D"], StringComparer.OrdinalIgnoreCase),
                    null
                )
                : new ObservableSet<string>(["A", "B", "C", "D"], StringComparer.OrdinalIgnoreCase);
        void Apply(IObservableSet<string> set, IEnumerable<string> input)
        {
            switch (action)
            {
                case 0:
                    set.IntersectWith(input);
                    break;
                case 1:
                    set.ExceptWith(input);
                    break;
                case 2:
                    set.SymmetricExceptWith(input);
                    break;
                default:
                    set.UnionWith(input);
                    break;
            }
        }
        var expected = action switch
        {
            0 => new[] { "B", "D" },
            1 => ["A", "C"],
            2 => ["A", "C", "E"],
            _ => ["A", "B", "C", "D", "E"],
        };
        var set = CreateSet();
        using var view = new ReadOnlyObservableSet<string>(set);
        var input = new List<string> { "b", "d", "E", "e" };
        var mirror = set.ToList();
        set.SetChanging += (_, _) =>
        {
            CollectionAssert.AreEqual(new[] { "A", "B", "C", "D" }, set.ToArray());
            Assert.ThrowsExactly<InvalidOperationException>(() => set.UnionWith([]));
            input.Clear();
        };
        view.SetChanged += (_, _) =>
        {
            CollectionAssert.AreEqual(expected, set.ToArray());
            Assert.ThrowsExactly<InvalidOperationException>(() => set.ExceptWith([]));
        };
        view.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Assert.AreEqual(mirror[e.OldStartingIndex], e.OldItems![0]);
                mirror.RemoveAt(e.OldStartingIndex);
            }
            else
                mirror.Insert(e.NewStartingIndex, (string)e.NewItems![0]!);
        };
        Apply(set, input);
        CollectionAssert.AreEqual(expected, mirror);
        foreach (var cancel in new[] { false, true })
        {
            var rejected = CreateSet();
            rejected.SetChanging += (_, _) =>
            {
                if (cancel)
                    throw new OperationCanceledException();
                throw new ApplicationException("Rejected.");
            };
            Assert.Throws<Exception>(() => Apply(rejected, ["b", "d", "E", "e"]));
            CollectionAssert.AreEqual(new[] { "A", "B", "C", "D" }, rejected.ToArray());
        }
        foreach (var standard in new[] { false, true })
        {
            var committed = CreateSet();
            if (standard)
                committed.CollectionChanged += (_, _) =>
                    throw new ApplicationException("Notification failed.");
            else
                committed.SetChanged += (_, _) =>
                    throw new ApplicationException("Notification failed.");
            Assert.ThrowsExactly<ApplicationException>(() =>
                Apply(committed, ["b", "d", "E", "e"])
            );
            CollectionAssert.AreEqual(expected, committed.ToArray());
        }
    }

    [TestMethod]
    public void DictionaryKeyValueViewsFreezeSubscriptionsAndUseCanonicalIndices()
    {
        var dictionary = new ObservableDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["A"] = 1,
            ["B"] = 2,
        };
        using var view = new ReadOnlyObservableDictionary<string, int>(dictionary);
        var keys = view.ObservableKeys;
        var values = view.ObservableValues;
        var oldCalls = 0;
        var newCalls = 0;
        NotifyCollectionChangedEventHandler existing = (_, e) =>
        {
            oldCalls++;
            Assert.AreEqual("A", e.OldItems![0]);
            Assert.AreEqual(0, e.OldStartingIndex);
            Assert.ThrowsExactly<InvalidOperationException>(() => dictionary.Clear());
        };
        keys.CollectionChanged += existing;
        dictionary.DictionaryChanging += (_, _) =>
        {
            keys.CollectionChanged -= existing;
            keys.CollectionChanged += (_, _) => newCalls++;
        };
        values.CollectionChanged += (_, e) =>
        {
            Assert.AreEqual(0, e.OldStartingIndex);
            CollectionAssert.AreEqual(dictionary.Values.ToArray(), values.ToArray());
        };
        dictionary.Remove("a");
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, newCalls);
        view.Dispose();
        dictionary.Remove("b");
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, newCalls);
    }

    [TestMethod]
    public void DictionaryStructuralChangesNotifyBothIndexerNamesAndCountOnlyWhenNeeded()
    {
        var dictionary = new ObservableDictionary<string, int>();
        var names = new List<string?>();
        dictionary.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        dictionary.Add("a", 1);
        dictionary["a"] = 2;
        dictionary["b"] = 3;
        dictionary.Remove("a");
        dictionary.Clear();
        dictionary.Clear();
        Assert.AreEqual(4, names.Count(n => n == "Count"));
        Assert.AreEqual(5, names.Count(n => n == "Item"));
        Assert.AreEqual(5, names.Count(n => n == "Item[]"));
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void EveryWriteEntryRejectsReentrancyEvenWhenItWouldBeANoOp(int kind, bool wrapper)
    {
        using var f = Create(kind, wrapper);
        Action[] writes;
        if (f.Collection is IObservableList<string> list)
        {
            var bridge = (System.Collections.IList)list;
            writes =
            [
                () => list.Add("x"),
                () => list.Insert(list.Count, "x"),
                () => list[0] = "a",
                () => list.RemoveAt(0),
                () => list.Remove("missing"),
                list.Clear,
                () => bridge.Add("x"),
                () => bridge.Insert(0, "x"),
                () => bridge[0] = "a",
                () => bridge.Remove("missing"),
                () => bridge.RemoveAt(0),
                bridge.Clear,
            ];
        }
        else if (f.Collection is IObservableDictionary<string, string> dictionary)
        {
            var pairs = (ICollection<KeyValuePair<string, string>>)dictionary;
            // 接口未声明 TryAdd；测试实例方法，而非仅执行 ContainsKey 就返回的框架扩展方法。
            Action tryAdd = dictionary is ObservableDictionary<string, string> concrete
                ? () => concrete.TryAdd("a", "x")
                : () =>
                    (
                        (ObservableDictionaryWrapper<string, string, Dictionary<string, string>>)
                            dictionary
                    ).TryAdd("a", "x");
            writes =
            [
                () => dictionary.Add("x", "x"),
                tryAdd,
                () => dictionary["a"] = "a",
                () => dictionary.Remove("missing"),
                dictionary.Clear,
                () => pairs.Add(new("x", "x")),
                () => pairs.Remove(new("a", "wrong")),
            ];
        }
        else
        {
            var set = (IObservableSet<string>)f.Collection;
            writes =
            [
                () => set.Add("a"),
                () => set.Remove("missing"),
                set.Clear,
                () => set.UnionWith([]),
                () => set.ExceptWith([]),
                () => set.IntersectWith(set),
                () => set.SymmetricExceptWith([]),
                () => ((ICollection<string>)set).Add("x"),
            ];
        }
        var other = new ObservableList<int>();
        void Probe()
        {
            foreach (var write in writes)
                Assert.ThrowsExactly<InvalidOperationException>(write);
            other.Add(1);
        }
        f.Before(Probe);
        f.After(Probe);
        f.ViewBefore(Probe);
        f.ViewAfter(Probe);
        f.Add();
        Assert.AreEqual(4, other.Count);
        CollectionAssert.AreEqual(new[] { "a", "b" }, f.Read());
    }

    [TestMethod]
    public void SingleSetRemovalUsesActualStoredInstanceAndSourceComparer()
    {
        var stored = new string(['A']);
        var set = new ObservableSetWrapper<string, OrderedHashSet<string>>(
            new([stored, "B"], StringComparer.OrdinalIgnoreCase),
            StringComparer.Ordinal
        );
        set.SetChanging += (_, e) => Assert.AreSame(stored, e.OldItems!.Single());
        set.CollectionChanged += (_, e) =>
        {
            Assert.AreSame(stored, e.OldItems![0]);
            Assert.AreEqual(0, e.OldStartingIndex);
        };
        Assert.IsTrue(set.Remove("a"));
    }

    [TestMethod]
    public void UnorderedAndSortedSetBatchesUseResetAndNullableWrappersRemainSupported()
    {
        foreach (
            ISet<string> source in new ISet<string>[]
            {
                new HashSet<string>(["B", "D"]),
                new SortedSet<string>(["B", "D"]),
            }
        )
        {
            var set = new ObservableSetWrapper<string, ISet<string>>(source, null);
            var calls = 0;
            set.CollectionChanged += (_, e) =>
            {
                calls++;
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, e.Action);
                Assert.IsTrue(set.SetEquals(["A", "D"]));
            };
            set.SymmetricExceptWith(["B", "A"]);
            Assert.AreEqual(1, calls);
        }
        var nullable = new ObservableSetWrapper<string?, HashSet<string?>>(new(), null);
        Assert.IsTrue(nullable.Add(null));
        Assert.IsTrue(nullable.Remove(null));
    }

    [TestMethod]
    public void SetInputsEnumerateOnceAndSelfOperationsRemainCorrect()
    {
        var set = new ObservableSet<int>([1, 2, 3]);
        var enumerations = 0;
        IEnumerable<int> Input()
        {
            enumerations++;
            if (enumerations > 1)
                throw new InvalidOperationException("Enumerated twice.");
            yield return 2;
            yield return 4;
            yield return 4;
        }
        set.SetChanged += (_, _) => { };
        set.SymmetricExceptWith(Input());
        CollectionAssert.AreEqual(new[] { 1, 3, 4 }, set.ToArray());
        Assert.AreEqual(1, enumerations);
        set.UnionWith(set);
        set.IntersectWith(set);
        CollectionAssert.AreEqual(new[] { 1, 3, 4 }, set.ToArray());
        set.SymmetricExceptWith(set);
        Assert.IsEmpty(set);
        set.UnionWith([1, 2]);
        set.ExceptWith(set);
        Assert.IsEmpty(set);
    }

    [TestMethod]
    public void UnobservedSingleWritesDoNotAllocateNotificationPayloads()
    {
        var list = new ObservableList<int>(4);
        var dictionary = new ObservableDictionary<int, int>();
        var set = new ObservableSet<int>(4);
        void Operate()
        {
            list.Add(1);
            list[0] = 2;
            list.RemoveAt(0);
            dictionary.Add(1, 1);
            dictionary[1] = 2;
            dictionary.Remove(1);
            set.Add(1);
            set.Remove(1);
        }
        for (var i = 0; i < 100; i++)
            Operate();
        var start = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
            Operate();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - start;
        Assert.AreEqual(0L, allocated);
    }

    [TestMethod]
    public void NestedReadOnlyWrappersFreezeSubscriptionsAndDisposeDuringOperation()
    {
        var source = new ObservableList<int>();
        using var first = new ReadOnlyObservableList<int>(source);
        using var second = new ReadOnlyObservableList<int>(first);
        var calls = 0;
        var lateCalls = 0;
        second.ListChanged += (_, _) => calls++;
        source.ListChanging += (_, _) =>
        {
            second.ListChanged += (_, _) => lateCalls++;
            second.Dispose();
        };
        source.Add(1);
        Assert.AreEqual(1, calls);
        Assert.AreEqual(0, lateCalls);
        source.Add(2);
        Assert.AreEqual(1, calls);
        Assert.AreEqual(0, lateCalls);
        CollectionAssert.AreEqual(new[] { 1, 2 }, second.ToArray());
    }

    [TestMethod]
    public void SetEventArgumentsAreIndependentOfMutableReadOnlyViews()
    {
        int[] input = [1, 2];
        var args = new NotifySetChangeEventArgs<int>(
            SetChangeAction.Union,
            Array.AsReadOnly(input),
            input,
            input
        );
        input[0] = 9;
        CollectionAssert.AreEqual(new[] { 1, 2 }, args.OtherItems!.ToArray());
        CollectionAssert.AreEqual(new[] { 1, 2 }, args.NewItems!.ToArray());
        CollectionAssert.AreEqual(new[] { 1, 2 }, args.OldItems!.ToArray());
    }
}
