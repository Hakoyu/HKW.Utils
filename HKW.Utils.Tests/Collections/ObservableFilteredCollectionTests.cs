using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class ObservableFilteredCollectionTests
{
    private sealed class Harness
    {
        public dynamic Wrapper { get; }
        public dynamic Source { get; private set; }
        public dynamic Target { get; }
        private readonly int _kind;
        public bool PredicateFails;
        public Action? InPredicate;
        public Action? BeforeSource;
        public Action? AfterSource;
        public Action? BeforeTarget;
        public Action? AfterTarget;

        public Harness(int kind)
        {
            _kind = kind;
            if (kind < 2)
            {
                var source = new ObservableList<int>([1, 2]);
                var target = new ObservableList<int>();
                Source = source;
                Target = target;
                Wrapper =
                    kind == 0
                        ? (object)
                            new FilteredListWrapper<int, ObservableList<int>, ObservableList<int>>(
                                source,
                                target,
                                Predicate
                            )
                        : new ObservableFilteredListWrapper<
                            int,
                            ObservableList<int>,
                            ObservableList<int>
                        >(source, target, Predicate);
                source.ListChanging += (_, _) => BeforeSource?.Invoke();
                source.ListChanged += (_, _) => AfterSource?.Invoke();
                target.ListChanging += (_, _) => BeforeTarget?.Invoke();
            }
            else if (kind < 4)
            {
                var source = new ObservableSet<int>([1, 2]);
                var target = new ObservableSet<int>();
                Source = source;
                Target = target;
                Wrapper =
                    kind == 2
                        ? (object)
                            new FilteredSetWrapper<int, ObservableSet<int>, ObservableSet<int>>(
                                source,
                                target,
                                Predicate
                            )
                        : new ObservableFilteredSetWrapper<
                            int,
                            ObservableSet<int>,
                            ObservableSet<int>
                        >(source, target, Predicate);
                source.SetChanging += (_, _) => BeforeSource?.Invoke();
                source.SetChanged += (_, _) => AfterSource?.Invoke();
                target.SetChanging += (_, _) => BeforeTarget?.Invoke();
            }
            else
            {
                var source = new ObservableDictionary<int, int> { [1] = 1, [2] = 2 };
                var target = new ObservableDictionary<int, int>();
                Source = source;
                Target = target;
                Wrapper =
                    kind == 4
                        ? (object)
                            new FilteredDictionaryWrapper<
                                int,
                                int,
                                ObservableDictionary<int, int>,
                                ObservableDictionary<int, int>
                            >(source, target, x => Predicate(x.Value))
                        : new ObservableFilteredDictionaryWrapper<
                            int,
                            int,
                            ObservableDictionary<int, int>,
                            ObservableDictionary<int, int>
                        >(source, target, x => Predicate(x.Value));
                source.DictionaryChanging += (_, _) => BeforeSource?.Invoke();
                source.DictionaryChanged += (_, _) => AfterSource?.Invoke();
                target.DictionaryChanging += (_, _) => BeforeTarget?.Invoke();
            }
            ((INotifyCollectionChanged)Target).CollectionChanged += (_, _) => AfterTarget?.Invoke();
        }

        private bool Predicate(int value)
        {
            InPredicate?.Invoke();
            if (PredicateFails && value == 2)
                throw new ApplicationException("Predicate failed.");
            return value > 0;
        }

        public int[] Filtered() =>
            Items(
                _kind < 2 ? Wrapper.FilteredList
                : _kind < 4 ? Wrapper.FilteredSet
                : Wrapper.FilteredDictionary
            );

        public int[] RawTarget() => Items(Target);

        public int[] SourceItems() => Items(Source);

        private int[] Items(object collection) =>
            _kind < 4
                ? ((IEnumerable<int>)collection).ToArray()
                : ((IEnumerable<KeyValuePair<int, int>>)collection).Select(x => x.Value).ToArray();

        public void Add(int value)
        {
            if (_kind < 4)
                Wrapper.Add(value);
            else
                Wrapper.Add(value, value);
        }

        public void AddSource(int value)
        {
            if (_kind < 4)
                Source.Add(value);
            else
                Source.Add(value, value);
        }

        public void Replace(object source)
        {
            try
            {
                if (_kind < 2)
                    Wrapper.SourceList = (ObservableList<int>)source;
                else if (_kind < 4)
                    Wrapper.SourceSet = (ObservableSet<int>)source;
                else
                    Wrapper.SourceDictionary = (ObservableDictionary<int, int>)source;
            }
            finally
            {
                Source =
                    _kind < 2 ? Wrapper.SourceList
                    : _kind < 4 ? Wrapper.SourceSet
                    : Wrapper.SourceDictionary;
            }
        }

        public object NewSource() =>
            _kind < 2 ? new ObservableList<int>([4])
            : _kind < 4 ? new ObservableSet<int>([4])
            : new ObservableDictionary<int, int> { [4] = 4 };

        public void Batch(Action action)
        {
            if (_kind < 2)
                Wrapper.BatchUpdate((Action<ObservableList<int>>)(_ => action()));
            else if (_kind < 4)
                Wrapper.BatchUpdate((Action<ObservableSet<int>>)(_ => action()));
            else
                Wrapper.BatchUpdate((Action<ObservableDictionary<int, int>>)(_ => action()));
        }

        public void Refresh() => Wrapper.Refresh();
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixRejectReentrantWritesAndRecover(int kind)
    {
        var h = new Harness(kind);
        h.InPredicate = () =>
        {
            Assert.AreEqual(3, (int)h.Wrapper.Count);
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Add(7));
            Assert.ThrowsExactly<InvalidOperationException>(h.Refresh);
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Wrapper.AutoFilter = false);
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Replace(h.NewSource()));
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Batch(() => { }));
            if (h.Wrapper is IDisposable disposable)
                Assert.ThrowsExactly<InvalidOperationException>(disposable.Dispose);
        };
        h.Add(3);
        h.InPredicate = null;
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        h.Add(4);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixChangingFailureDoesNotCommitAndChangedFailureRepairsView(int kind)
    {
        var h = new Harness(kind);
        var before = h.Filtered();
        var failure = new OperationCanceledException("Change canceled.");
        h.BeforeSource = () => throw failure;
        Assert.AreSame(failure, Assert.ThrowsExactly<OperationCanceledException>(() => h.Add(3)));
        CollectionAssert.AreEquivalent(before, h.SourceItems());
        CollectionAssert.AreEquivalent(before, h.Filtered());
        h.BeforeSource = null;
        h.AfterSource = () => throw failure;
        Assert.AreSame(failure, Assert.ThrowsExactly<OperationCanceledException>(() => h.Add(3)));
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, h.SourceItems());
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        Assert.IsTrue((bool)h.Wrapper.IsFilterValid);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixFailedCommitRequiresExplicitRefresh(int kind)
    {
        var h = new Harness(kind);
        h.BeforeTarget = () => throw new ApplicationException("Target rejected commit.");
        Assert.ThrowsExactly<ApplicationException>(() => h.Add(3));
        Assert.IsFalse((bool)h.Wrapper.IsFilterValid);
        Assert.ThrowsExactly<InvalidOperationException>(() => h.Filtered());
        Assert.ThrowsExactly<InvalidOperationException>(() => h.Add(4));
        h.BeforeTarget = null;
        h.Refresh();
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        Assert.IsTrue((bool)h.Wrapper.IsFilterValid);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixFinalTargetChangedFailureKeepsCompleteState(int kind)
    {
        var h = new Harness(kind);
        h.AfterTarget = () =>
        {
            if (h.RawTarget().Length == 3)
                throw new ApplicationException("Changed handler failed.");
        };
        Assert.ThrowsExactly<ApplicationException>(() => h.Add(3));
        Assert.IsTrue((bool)h.Wrapper.IsFilterValid);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixBatchPreservesBothFailuresAndRestoresSwitch(int kind)
    {
        var h = new Harness(kind);
        h.Wrapper.AutoFilter = false;
        var original = new OperationCanceledException("Batch canceled.");
        var failure = Assert.ThrowsExactly<AggregateException>(() =>
            h.Batch(() =>
            {
                h.AddSource(3);
                h.PredicateFails = true;
                throw original;
            })
        );
        Assert.AreSame(original, failure.InnerExceptions[0]);
        Assert.IsInstanceOfType<ApplicationException>(failure.InnerExceptions[1]);
        Assert.IsFalse((bool)h.Wrapper.AutoFilter);
        Assert.IsFalse((bool)h.Wrapper.IsFilterValid);
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, h.RawTarget());
        h.PredicateFails = false;
        h.Refresh();
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(5)]
    public void ObservableSwitchDetachesOldSourceAndDisposeDetachesCurrent(int kind)
    {
        var h = new Harness(kind);
        dynamic old = h.Source;
        h.Replace(h.NewSource());
        if (kind < 4)
            old.Add(9);
        else
            old.Add(9, 9);
        CollectionAssert.AreEqual(new[] { 4 }, h.Filtered());
        h.AddSource(5);
        CollectionAssert.AreEquivalent(new[] { 4, 5 }, h.Filtered());
        h.Wrapper.AutoFilter = false;
        h.AddSource(6);
        CollectionAssert.AreEquivalent(new[] { 4, 5 }, h.Filtered());
        h.Source.Clear();
        Assert.AreEqual(0, h.Filtered().Length);
        var disposable = (IDisposable)h.Wrapper;
        disposable.Dispose();
        disposable.Dispose();
        h.AddSource(7);
        Assert.AreEqual(0, h.RawTarget().Length);
        Assert.ThrowsExactly<ObjectDisposedException>(() => h.Add(8));
        Assert.ThrowsExactly<ObjectDisposedException>(h.Refresh);
        Assert.ThrowsExactly<ObjectDisposedException>(() => h.Replace(h.NewSource()));
        Assert.ThrowsExactly<ObjectDisposedException>(() => h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixAliasAndNullSwitchPreserveSource(int kind)
    {
        var h = new Harness(kind);
        Assert.ThrowsExactly<ArgumentException>(() => h.Replace((object)h.Target));
        Assert.ThrowsExactly<ArgumentNullException>(() => h.Replace(null!));
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, h.SourceItems());
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        h.Add(3);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixSwitchFailurePreservesSubscriptionOrRequiresRefresh(int kind)
    {
        var h = new Harness(kind);
        object old = h.Source;
        dynamic candidate = h.NewSource();
        if (kind < 4)
            candidate.Add(2);
        else
            candidate.Add(2, 2);
        h.PredicateFails = true;
        Assert.ThrowsExactly<ApplicationException>(() => h.Replace((object)candidate));
        Assert.AreSame(old, (object)h.Source);
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, h.Filtered());
        h.PredicateFails = false;
        h.BeforeTarget = () => throw new ApplicationException("Commit failed.");
        Assert.ThrowsExactly<ApplicationException>(() => h.Replace((object)candidate));
        Assert.AreSame((object)candidate, (object)h.Source);
        Assert.IsFalse((bool)h.Wrapper.IsFilterValid);
        h.BeforeTarget = null;
        h.Refresh();
        h.AddSource(5);
        if (kind % 2 == 0)
            h.Refresh();
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        if (kind < 4)
            ((dynamic)old).Add(8);
        else
            ((dynamic)old).Add(8, 8);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixNestedBatchRejectsWrapperWritesButAcceptsSourceWrites(int kind)
    {
        var h = new Harness(kind);
        h.Batch(() =>
        {
            Assert.IsFalse((bool)h.Wrapper.AutoFilter);
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Batch(() => { }));
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Add(7));
            h.AddSource(3);
        });
        Assert.IsTrue((bool)h.Wrapper.AutoFilter);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
        h.Refresh();
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixNoChangeRefreshComputesOnceAndDoesNotWriteTarget(int kind)
    {
        var h = new Harness(kind);
        var evaluations = 0;
        var writes = 0;
        h.InPredicate = () => evaluations++;
        h.BeforeTarget = () => writes++;
        h.Refresh();
        Assert.AreEqual(h.SourceItems().Length, evaluations);
        Assert.AreEqual(0, writes);
        h.Wrapper.AutoFilter = false;
        h.Wrapper.Remove(1);
        h.Add(4);
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, h.Filtered());
        h.Wrapper.Clear();
        Assert.AreEqual(0, h.Filtered().Length);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixSourceUsesWholeOperationSubscriberSnapshot(int kind)
    {
        var h = new Harness(kind);
        var source = (INotifyCollectionChanged)h.Source;
        var oldCalls = 0;
        var newCalls = 0;
        NotifyCollectionChangedEventHandler oldHandler = (_, _) => oldCalls++;
        NotifyCollectionChangedEventHandler newHandler = (_, _) => newCalls++;
        source.CollectionChanged += oldHandler;
        h.BeforeSource = () =>
        {
            source.CollectionChanged -= oldHandler;
            source.CollectionChanged += newHandler;
            Assert.ThrowsExactly<InvalidOperationException>(() => h.Add(9));
            Assert.AreEqual(2, (int)h.Wrapper.Count);
        };
        h.Add(3);
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, newCalls);
        h.BeforeSource = null;
        h.Add(4);
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(1, newCalls);
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void AllSixPartialTargetChangedFailureIsExplicitlyInvalid(int kind)
    {
        var h = new Harness(kind);
        h.AfterTarget = () => throw new ApplicationException("Partial publication failed.");
        Assert.ThrowsExactly<ApplicationException>(() =>
            h.Batch(() =>
            {
                h.AddSource(3);
                h.AddSource(4);
            })
        );
        Assert.IsFalse((bool)h.Wrapper.IsFilterValid);
        Assert.ThrowsExactly<InvalidOperationException>(() => h.Filtered());
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, h.SourceItems());
        h.AfterTarget = null;
        h.Refresh();
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(5)]
    public void DirectSourceMutationDuringPublicationCannotRevalidateStaleSnapshot(int kind)
    {
        var h = new Harness(kind);
        h.AfterTarget = () =>
        {
            if (h.RawTarget().Length == 3)
            {
                h.AfterTarget = null;
                h.AddSource(9);
            }
        };
        Assert.ThrowsExactly<InvalidOperationException>(() => h.Add(3));
        Assert.IsFalse((bool)h.Wrapper.IsFilterValid);
        Assert.ThrowsExactly<InvalidOperationException>(() => h.Filtered());
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 9 }, h.SourceItems());
        h.Refresh();
        CollectionAssert.AreEquivalent(h.SourceItems(), h.Filtered());
    }

    [TestMethod]
    public void SourceStorageRejectsSelfAndNestedFilteredAliases()
    {
        var list = new List<int> { 1 };
        var wrapper = new FilteredListWrapper<int, IList<int>, IList<int>>(
            list,
            new List<int>(),
            _ => true
        );
        Assert.ThrowsExactly<ArgumentException>(() => wrapper.SourceList = wrapper);
        var outer = new FilteredListWrapper<int, IList<int>, IList<int>>(
            wrapper,
            new List<int>(),
            _ => true
        );
        Assert.ThrowsExactly<ArgumentException>(() => wrapper.SourceList = outer);
        Assert.ThrowsExactly<ArgumentException>(() =>
            new FilteredListWrapper<int, IList<int>, IList<int>>(wrapper, list, _ => true)
        );
        CollectionAssert.AreEqual(new[] { 1 }, list);
        var set = new FilteredSetWrapper<int, ISet<int>, ISet<int>>(
            new HashSet<int> { 1 },
            new HashSet<int>(),
            _ => true
        );
        Assert.ThrowsExactly<ArgumentException>(() => set.SourceSet = set);
        var dictionary = new FilteredDictionaryWrapper<
            int,
            int,
            IDictionary<int, int>,
            IDictionary<int, int>
        >(new Dictionary<int, int> { [1] = 1 }, new Dictionary<int, int>(), _ => true);
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.SourceDictionary = dictionary);
    }

    [TestMethod]
    public void ConstructorsRejectAliasesAndReadOnlySourcesBeforeWriting()
    {
        var list = new ObservableList<int>([1]);
        var set = new ObservableSet<int>([1]);
        var dictionary = new ObservableDictionary<int, int> { [1] = 1 };
        Assert.ThrowsExactly<ArgumentException>(() =>
            new FilteredListWrapper<int, ObservableList<int>, ObservableList<int>>(
                list,
                list,
                _ => true
            )
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ObservableFilteredListWrapper<int, ObservableList<int>, ObservableList<int>>(
                list,
                list,
                _ => true
            )
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            new FilteredSetWrapper<int, ObservableSet<int>, ObservableSet<int>>(set, set, _ => true)
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ObservableFilteredSetWrapper<int, ObservableSet<int>, ObservableSet<int>>(
                set,
                set,
                _ => true
            )
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            new FilteredDictionaryWrapper<
                int,
                int,
                ObservableDictionary<int, int>,
                ObservableDictionary<int, int>
            >(dictionary, dictionary, _ => true)
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ObservableFilteredDictionaryWrapper<
                int,
                int,
                ObservableDictionary<int, int>,
                ObservableDictionary<int, int>
            >(dictionary, dictionary, _ => true)
        );
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(1, set.Count);
        Assert.AreEqual(1, dictionary.Count);
        var wrapper = new FilteredListWrapper<int, IList<int>, IList<int>>(
            new List<int> { 1 },
            new List<int>(),
            _ => true
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            wrapper.SourceList = new ReadOnlyCollection<int>(new List<int> { 2 })
        );
        CollectionAssert.AreEqual(new[] { 1 }, wrapper.FilteredList.ToArray());
        var dictWrapper = new FilteredDictionaryWrapper<
            int,
            int,
            IDictionary<int, int>,
            IDictionary<int, int>
        >(new Dictionary<int, int> { [1] = 1 }, new Dictionary<int, int>(), _ => true);
        Assert.ThrowsExactly<ArgumentException>(() =>
            dictWrapper.SourceDictionary = new ReadOnlyDictionary<int, int>(
                new Dictionary<int, int>()
            )
        );
        Assert.AreEqual(1, dictWrapper.FilteredDictionary.Count);
    }
}
