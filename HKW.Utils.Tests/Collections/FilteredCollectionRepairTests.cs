using System.Collections;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class FilteredCollectionRepairTests
{
    private static Adapter Create(
        int kind,
        ICollection<int>? source = null,
        ICollection<int>? target = null,
        Predicate<int>? filter = null
    )
    {
        source ??= kind switch
        {
            1 => new ObservableList<int>([1, 2, 1]),
            2 => new HashSet<int>([1, 2]),
            3 => new ObservableSet<int>([1, 2]),
            _ => new List<int>([1, 2, 1]),
        };
        target ??= kind >= 2 ? new HashSet<int>() : new List<int>();
        return new Adapter(kind, source, target, filter ?? (x => x > 0));
    }

    private sealed class Adapter
        : IFilteredCollectionWrapper<int, ICollection<int>, ICollection<int>>,
            IDisposable
    {
        private readonly dynamic _inner;

        public Adapter(
            int kind,
            ICollection<int> source,
            ICollection<int> target,
            Predicate<int> filter
        )
        {
            _inner = kind switch
            {
                0 => (object)
                    new FilteredListWrapper<int, IList<int>, IList<int>>(
                        (IList<int>)source,
                        (IList<int>)target,
                        filter
                    ),
                1 => new ObservableFilteredListWrapper<int, ObservableList<int>, IList<int>>(
                    (ObservableList<int>)source,
                    (IList<int>)target,
                    filter
                ),
                2 => new FilteredSetWrapper<int, ISet<int>, ISet<int>>(
                    (ISet<int>)source,
                    (ISet<int>)target,
                    filter
                ),
                _ => new ObservableFilteredSetWrapper<int, ObservableSet<int>, ISet<int>>(
                    (ObservableSet<int>)source,
                    (ISet<int>)target,
                    filter
                ),
            };
        }

        public ICollection<int> SourceCollection =>
            _inner is ISet<int> ? _inner.SourceSet : _inner.SourceList;
        public ICollection<int> FilteredCollection =>
            _inner is ISet<int> ? _inner.FilteredSet : _inner.FilteredList;
        public bool AutoFilter
        {
            get => _inner.AutoFilter;
            set => _inner.AutoFilter = value;
        }
        public Predicate<int> Filter
        {
            get => _inner.Filter;
            set => _inner.Filter = value;
        }
        public bool IsFilterValid { get; set; }
        public int Count => SourceCollection.Count;
        public bool IsReadOnly => false;

        public void BatchUpdate(Action<ICollection<int>> action)
        {
            if (_inner is FilteredListWrapper<int, IList<int>, IList<int>> list)
                list.BatchUpdate(x => action(x));
            else if (
                _inner
                is ObservableFilteredListWrapper<
                    int,
                    ObservableList<int>,
                    IList<int>
                > observableList
            )
                observableList.BatchUpdate(x => action(x));
            else if (_inner is FilteredSetWrapper<int, ISet<int>, ISet<int>> set)
                set.BatchUpdate(x => action(x));
            else
                (
                    (ObservableFilteredSetWrapper<int, ObservableSet<int>, ISet<int>>)_inner
                ).BatchUpdate(x => action(x));
        }

        public void Refresh() => _inner.Refresh();

        public void Add(int item) => ((ICollection<int>)_inner).Add(item);

        public void Clear() => _inner.Clear();

        public bool Contains(int item) => SourceCollection.Contains(item);

        public void CopyTo(int[] array, int index) => SourceCollection.CopyTo(array, index);

        public bool Remove(int item) => _inner.Remove(item);

        public IEnumerator<int> GetEnumerator() => SourceCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            if (_inner is IDisposable disposable)
                disposable.Dispose();
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void BatchFailureRestoresViewAndSwitch(int kind)
    {
        var wrapper = Create(kind);
        var error = new ApplicationException("Batch failed.");
        Assert.AreSame(
            error,
            Assert.ThrowsExactly<ApplicationException>(() =>
                wrapper.BatchUpdate(source =>
                {
                    source.Add(3);
                    throw error;
                })
            )
        );
        Assert.IsTrue(wrapper.AutoFilter);
        CollectionAssert.AreEqual(
            wrapper.SourceCollection.ToArray(),
            wrapper.FilteredCollection.ToArray()
        );
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void PredicateFailurePreservesPublishedContents(int kind)
    {
        var fail = false;
        var wrapper = Create(
            kind,
            filter: value =>
                fail && value == 2 ? throw new ApplicationException("Predicate failed.") : true
        );
        var target = wrapper.FilteredCollection;
        var before = target.ToArray();
        fail = true;
        Assert.ThrowsExactly<ApplicationException>(wrapper.Refresh);
        CollectionAssert.AreEqual(before, target.ToArray());
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = wrapper.FilteredCollection);
        fail = false;
        wrapper.Refresh();
        CollectionAssert.AreEqual(before, wrapper.FilteredCollection.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void AutoFilterFreezesViewExceptClear(int kind)
    {
        var wrapper = Create(kind);
        var before = wrapper.FilteredCollection.ToArray();
        wrapper.AutoFilter = false;
        wrapper.Remove(1);
        wrapper.Add(4);
        CollectionAssert.AreEqual(before, wrapper.FilteredCollection.ToArray());
        wrapper.Clear();
        Assert.AreEqual(0, wrapper.FilteredCollection.Count);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ListDuplicatesFollowSourcePosition(bool observable)
    {
        FilteredListWrapper<int, IList<int>, IList<int>>? plain = observable
            ? null
            : new(new List<int> { 1, 2, 1 }, new List<int>(), _ => true);
        var watched = observable
            ? new ObservableFilteredListWrapper<int, ObservableList<int>, List<int>>(
                new([1, 2, 1]),
                new(),
                _ => true
            )
            : null;
        IList<int> wrapper = observable ? watched! : plain!;
        wrapper[2] = 3;
        CollectionAssert.AreEqual(
            new[] { 1, 2, 3 },
            observable ? watched!.FilteredList.ToArray() : plain!.FilteredList.ToArray()
        );
        wrapper[2] = 1;
        wrapper.RemoveAt(2);
        CollectionAssert.AreEqual(
            new[] { 1, 2 },
            observable ? watched!.FilteredList.ToArray() : plain!.FilteredList.ToArray()
        );
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void SetOperationsUseActualRepresentativesAndSingleEnumeration(bool observable)
    {
        var source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "A" };
        var watchedSource = new ObservableSetWrapper<string, HashSet<string>>(
            source,
            StringComparer.OrdinalIgnoreCase
        );
        ISet<string> wrapper;
        Func<ISet<string>> target;
        if (observable)
        {
            var value = new ObservableFilteredSetWrapper<
                string,
                ObservableSetWrapper<string, HashSet<string>>,
                HashSet<string>
            >(watchedSource, new(StringComparer.OrdinalIgnoreCase), x => x == "A");
            wrapper = value;
            target = () => value.FilteredSet;
        }
        else
        {
            var value = new FilteredSetWrapper<string, HashSet<string>, HashSet<string>>(
                source,
                new(StringComparer.OrdinalIgnoreCase),
                x => x == "A"
            );
            wrapper = value;
            target = () => value.FilteredSet;
        }
        wrapper.ExceptWith(new Once<string>(["a"]));
        Assert.AreEqual(0, target().Count);
        wrapper.UnionWith(new Once<string>(["A", "B"]));
        CollectionAssert.AreEquivalent(new[] { "A" }, target().ToArray());
        wrapper.IntersectWith(new Once<string>(["a"]));
        CollectionAssert.AreEquivalent(new[] { "A" }, target().ToArray());
        wrapper.SymmetricExceptWith(new Once<string>(["a"]));
        Assert.AreEqual(0, target().Count);
        wrapper.Add("A");
        wrapper.ExceptWith(wrapper);
        Assert.AreEqual(0, target().Count);
    }

    private sealed class Once<T>(IEnumerable<T> items) : IEnumerable<T>
    {
        private bool _used;

        public IEnumerator<T> GetEnumerator()
        {
            if (_used)
                throw new InvalidOperationException("Input was enumerated twice.");
            _used = true;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void DictionaryBatchFailureAndPredicateFailure(bool observable)
    {
        var source = new ObservableDictionary<int, int> { [1] = 1 };
        FilteredDictionaryWrapper<
            int,
            int,
            ObservableDictionary<int, int>,
            Dictionary<int, int>
        > wrapper = observable
            ? new ObservableFilteredDictionaryWrapper<
                int,
                int,
                ObservableDictionary<int, int>,
                Dictionary<int, int>
            >(source, new(), _ => true)
            : new(source, new(), _ => true);
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            wrapper.BatchUpdate(x =>
            {
                x.Add(2, 2);
                throw new OperationCanceledException();
            })
        );
        Assert.AreEqual(2, wrapper.FilteredDictionary.Count);
        var before = wrapper.FilteredDictionary;
        Assert.ThrowsExactly<ApplicationException>(() =>
            wrapper.Filter = _ => throw new ApplicationException()
        );
        Assert.AreEqual(2, before.Count);
    }
}
