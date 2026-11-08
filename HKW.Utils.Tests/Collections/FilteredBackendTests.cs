using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class FilteredBackendTests
{
    private sealed class ReadOnlyListSource()
        : ObservableListWrapper<int, List<int>>(new()),
            ICollection<int>
    {
        bool ICollection<int>.IsReadOnly => true;
    }

    private sealed class ReadOnlySetSource()
        : ObservableSetWrapper<int, HashSet<int>>(new(), EqualityComparer<int>.Default),
            ICollection<int>
    {
        bool ICollection<int>.IsReadOnly => true;
    }

    private sealed class ReadOnlyDictionarySource()
        : ObservableDictionaryWrapper<int, int, Dictionary<int, int>>(
            new(),
            EqualityComparer<int>.Default
        ),
            ICollection<KeyValuePair<int, int>>
    {
        bool ICollection<KeyValuePair<int, int>>.IsReadOnly => true;
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ReadOnlyReplacementRejectedAcrossAllSix(bool observable)
    {
        IObservableList<int> list = new ObservableList<int>([1]);
        IObservableSet<int> set = new ObservableSet<int>([1]);
        IObservableDictionary<int, int> dictionary = new ObservableDictionary<int, int> { [1] = 1 };
        var listWrapper = observable
            ? new ObservableFilteredListWrapper<int, IObservableList<int>, List<int>>(
                list,
                new(),
                _ => true
            )
            : (FilteredListWrapper<int, IObservableList<int>, List<int>>)
                new(list, new(), _ => true);
        var setWrapper = observable
            ? new ObservableFilteredSetWrapper<int, IObservableSet<int>, HashSet<int>>(
                set,
                new(),
                _ => true
            )
            : (FilteredSetWrapper<int, IObservableSet<int>, HashSet<int>>)
                new(set, new(), _ => true);
        var dictWrapper = observable
            ? new ObservableFilteredDictionaryWrapper<
                int,
                int,
                IObservableDictionary<int, int>,
                Dictionary<int, int>
            >(dictionary, new(), _ => true)
            : (FilteredDictionaryWrapper<
                int,
                int,
                IObservableDictionary<int, int>,
                Dictionary<int, int>
            >)
                new(dictionary, new(), _ => true);
        Assert.ThrowsExactly<ArgumentException>(() =>
            listWrapper.SourceList = new ReadOnlyListSource()
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            setWrapper.SourceSet = new ReadOnlySetSource()
        );
        Assert.ThrowsExactly<ArgumentException>(() =>
            dictWrapper.SourceDictionary = new ReadOnlyDictionarySource()
        );
        Assert.AreSame(list, listWrapper.SourceList);
        Assert.AreSame(set, setWrapper.SourceSet);
        Assert.AreSame(dictionary, dictWrapper.SourceDictionary);
        listWrapper.Add(2);
        setWrapper.Add(2);
        dictWrapper.Add(2, 2);
        Assert.AreEqual(2, listWrapper.FilteredList.Count);
        Assert.AreEqual(2, setWrapper.FilteredSet.Count);
        Assert.AreEqual(2, dictWrapper.FilteredDictionary.Count);
    }

    [TestMethod]
    [DataRow(false, 0)]
    [DataRow(true, 0)]
    [DataRow(false, 1)]
    [DataRow(true, 1)]
    [DataRow(false, 2)]
    [DataRow(true, 2)]
    [DataRow(false, 3)]
    [DataRow(true, 3)]
    public void SetAliasedLazyInputsFollowFinalSource(bool observable, int operation)
    {
        var source = new ObservableSet<int>([1, 2, 3, 4]);
        FilteredSetWrapper<int, ObservableSet<int>, SortedSet<int>> wrapper = observable
            ? new ObservableFilteredSetWrapper<int, ObservableSet<int>, SortedSet<int>>(
                source,
                new(),
                x => x % 2 == 0
            )
            : new(source, new(), x => x % 2 == 0);
        var input = source.Where(x => x >= 2);
        var expected = new HashSet<int>(source);
        Apply(expected, expected.Where(x => x >= 2).ToArray(), operation);
        Apply(wrapper, input, operation);
        Assert.IsTrue(expected.SetEquals(source));
        CollectionAssert.AreEquivalent(
            source.Where(x => x % 2 == 0).ToArray(),
            wrapper.FilteredSet.ToArray()
        );
        wrapper.SymmetricExceptWith(wrapper.FilteredSet);
        Assert.AreEqual(0, wrapper.FilteredSet.Count);
    }

    private static void Apply(ISet<int> set, IEnumerable<int> input, int operation)
    {
        switch (operation)
        {
            case 0:
                set.ExceptWith(input);
                break;
            case 1:
                set.IntersectWith(input);
                break;
            case 2:
                set.UnionWith(input);
                break;
            case 3:
                set.SymmetricExceptWith(input);
                break;
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ComparerEquivalentInputsCannotReplaceSourceRepresentatives(bool observable)
    {
        var source = new ObservableSet<string>(new[] { "A" }, StringComparer.OrdinalIgnoreCase);
        FilteredSetWrapper<string, ObservableSet<string>, HashSet<string>> wrapper = observable
            ? new ObservableFilteredSetWrapper<string, ObservableSet<string>, HashSet<string>>(
                source,
                new(StringComparer.OrdinalIgnoreCase),
                x => x == "a"
            )
            : new(source, new(StringComparer.OrdinalIgnoreCase), x => x == "a");
        wrapper.UnionWith(["a"]);
        Assert.AreEqual(0, wrapper.FilteredSet.Count);
        Assert.IsFalse(wrapper.Add("a"));
        Assert.AreEqual(0, wrapper.FilteredSet.Count);
        wrapper.Filter = x => x == "A";
        wrapper.ExceptWith(["a"]);
        Assert.AreEqual(0, wrapper.FilteredSet.Count);

        var dictSource = new ObservableDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["A"] = 1,
        };
        FilteredDictionaryWrapper<
            string,
            int,
            ObservableDictionary<string, int>,
            Dictionary<string, int>
        > dictionary = observable
            ? new ObservableFilteredDictionaryWrapper<
                string,
                int,
                ObservableDictionary<string, int>,
                Dictionary<string, int>
            >(dictSource, new(StringComparer.Ordinal), x => x.Key == "A")
            : new(dictSource, new(StringComparer.Ordinal), x => x.Key == "A");
        dictionary["a"] = 2;
        Assert.AreEqual(2, dictionary.FilteredDictionary["A"]);
        Assert.IsFalse(dictionary.FilteredDictionary.ContainsKey("a"));
        dictionary.Remove("a");
        Assert.AreEqual(0, dictionary.FilteredDictionary.Count);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void IncompatibleComparersInvalidateInsteadOfSilentlyMerging(bool observable)
    {
        var source = new ObservableSet<string>(new[] { "A" }, StringComparer.Ordinal);
        FilteredSetWrapper<string, ObservableSet<string>, HashSet<string>> wrapper = observable
            ? new ObservableFilteredSetWrapper<string, ObservableSet<string>, HashSet<string>>(
                source,
                new(StringComparer.OrdinalIgnoreCase),
                _ => true
            )
            : new(source, new(StringComparer.OrdinalIgnoreCase), _ => true);
        Assert.ThrowsExactly<InvalidOperationException>(() => wrapper.Add("a"));
        Assert.IsFalse(wrapper.IsFilterValid);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = wrapper.FilteredSet);
        source.Remove("a");
        wrapper.Refresh();
        Assert.IsTrue(wrapper.IsFilterValid);

        var dictSource = new ObservableDictionary<string, int>(StringComparer.Ordinal)
        {
            ["A"] = 1,
        };
        FilteredDictionaryWrapper<
            string,
            int,
            ObservableDictionary<string, int>,
            Dictionary<string, int>
        > dictionary = observable
            ? new ObservableFilteredDictionaryWrapper<
                string,
                int,
                ObservableDictionary<string, int>,
                Dictionary<string, int>
            >(dictSource, new(StringComparer.OrdinalIgnoreCase), _ => true)
            : new(dictSource, new(StringComparer.OrdinalIgnoreCase), _ => true);
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.Add("a", 2));
        Assert.IsFalse(dictionary.IsFilterValid);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = dictionary.FilteredDictionary);
        dictSource.Remove("a");
        dictionary.Refresh();
        Assert.IsTrue(dictionary.IsFilterValid);
    }

    private sealed class TrackedList()
        : ObservableListWrapper<int, List<int>>(new()),
            System.Collections.Specialized.INotifyCollectionChanged
    {
        public int Subscriptions;
        event System.Collections.Specialized.NotifyCollectionChangedEventHandler? System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                Subscriptions++;
                CollectionChanged += value;
            }
            remove
            {
                Subscriptions--;
                CollectionChanged -= value;
            }
        }
    }

    private sealed class TrackedSet()
        : ObservableSetWrapper<int, HashSet<int>>(new(), EqualityComparer<int>.Default),
            System.Collections.Specialized.INotifyCollectionChanged
    {
        public int Subscriptions;
        event System.Collections.Specialized.NotifyCollectionChangedEventHandler? System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                Subscriptions++;
                CollectionChanged += value;
            }
            remove
            {
                Subscriptions--;
                CollectionChanged -= value;
            }
        }
    }

    private sealed class TrackedDictionary()
        : ObservableDictionaryWrapper<int, int, Dictionary<int, int>>(
            new(),
            EqualityComparer<int>.Default
        ),
            System.Collections.Specialized.INotifyCollectionChanged
    {
        public int Subscriptions;
        event System.Collections.Specialized.NotifyCollectionChangedEventHandler? System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                Subscriptions++;
                CollectionChanged += value;
            }
            remove
            {
                Subscriptions--;
                CollectionChanged -= value;
            }
        }
    }

    [TestMethod]
    public void ObservableLifetimesRemoveEverySubscription()
    {
        var oldList = new TrackedList();
        var newList = new TrackedList();
        var oldSet = new TrackedSet();
        var newSet = new TrackedSet();
        var oldDictionary = new TrackedDictionary();
        var newDictionary = new TrackedDictionary();
        var list = new ObservableFilteredListWrapper<int, TrackedList, List<int>>(
            oldList,
            new(),
            _ => true
        );
        var set = new ObservableFilteredSetWrapper<int, TrackedSet, HashSet<int>>(
            oldSet,
            new(),
            _ => true
        );
        var dictionary = new ObservableFilteredDictionaryWrapper<
            int,
            int,
            TrackedDictionary,
            Dictionary<int, int>
        >(oldDictionary, new(), _ => true);
        Assert.AreEqual(1, oldList.Subscriptions);
        Assert.AreEqual(1, oldSet.Subscriptions);
        Assert.AreEqual(1, oldDictionary.Subscriptions);
        list.SourceList = newList;
        set.SourceSet = newSet;
        dictionary.SourceDictionary = newDictionary;
        Assert.AreEqual(0, oldList.Subscriptions);
        Assert.AreEqual(0, oldSet.Subscriptions);
        Assert.AreEqual(0, oldDictionary.Subscriptions);
        Assert.AreEqual(1, newList.Subscriptions);
        Assert.AreEqual(1, newSet.Subscriptions);
        Assert.AreEqual(1, newDictionary.Subscriptions);
        list.Dispose();
        set.Dispose();
        dictionary.Dispose();
        list.Dispose();
        set.Dispose();
        dictionary.Dispose();
        Assert.AreEqual(0, newList.Subscriptions);
        Assert.AreEqual(0, newSet.Subscriptions);
        Assert.AreEqual(0, newDictionary.Subscriptions);
    }

    private sealed class RejectingList : System.Collections.ObjectModel.Collection<int>
    {
        public bool RejectRemoval;

        protected override void RemoveItem(int index)
        {
            if (RejectRemoval is false)
                base.RemoveItem(index);
        }
    }

    [TestMethod]
    public void BackendThatSilentlyRejectsRemovalCannotHangPublication()
    {
        var target = new RejectingList();
        var wrapper = new FilteredListWrapper<int, List<int>, RejectingList>(
            new() { 1, 2 },
            target,
            _ => true
        );
        target.RejectRemoval = true;
        Assert.ThrowsExactly<InvalidOperationException>(wrapper.Clear);
        Assert.IsFalse(wrapper.IsFilterValid);
        target.RejectRemoval = false;
        wrapper.Refresh();
        Assert.AreEqual(0, wrapper.FilteredList.Count);
    }

    private sealed record Value(int Identity, int Payload)
    {
        public bool Equals(Value? other) => other?.Identity == Identity;

        public override int GetHashCode() => Identity;
    }

    [TestMethod]
    public void EqualReferencesKeepActualSourceInstances()
    {
        var old = new Value(1, 1);
        var replacement = new Value(1, 2);
        var list = new FilteredListWrapper<Value, List<Value>, List<Value>>(
            new() { old },
            new(),
            _ => true
        );
        list[0] = replacement;
        Assert.AreSame(replacement, list.FilteredList[0]);
        var set = new FilteredSetWrapper<Value, HashSet<Value>, HashSet<Value>>(
            new() { old },
            new(),
            _ => true
        );
        set.BatchUpdate(source =>
        {
            source.Remove(old);
            source.Add(replacement);
        });
        Assert.AreSame(replacement, set.FilteredSet.Single());
        var dictionary = new FilteredDictionaryWrapper<
            int,
            Value,
            Dictionary<int, Value>,
            Dictionary<int, Value>
        >(new() { [1] = old }, new(), _ => true);
        dictionary[1] = replacement;
        Assert.AreSame(replacement, dictionary.FilteredDictionary[1]);
    }
}
