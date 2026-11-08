using System.ComponentModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ObservableSelectionRepairTests
{
    [TestMethod]
    public void DefaultValues_AreSelectable()
    {
        var list = new ObservableSelectableList<int>([0]);
        list.SelectedItem = 0;
        Assert.IsTrue(list.HasSelection);
        var set = new ObservableSelectableSet<int>([0]);
        set.SelectedItem = 0;
        Assert.IsTrue(set.HasSelection);
        Assert.AreEqual(0, set.SelectedIndex);
        var dictionary = new ObservableSelectableDictionary<int, int>([new(0, 0)]);
        dictionary.SelectedItem = default;
        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreEqual(0, dictionary.SelectedIndex);
        dictionary.SelectedIndex = -1;
        Assert.IsFalse(dictionary.HasSelection);
    }

    [TestMethod]
    public void MissingSelection_AddAndReselect_RecomputesMembership()
    {
        var set = new ObservableSelectableSet<int>();
        set.SelectedItem = 5;
        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(0, set.SelectedItem);
        set.Add(5);
        Assert.IsFalse(set.HasSelection);
        set.SelectedItem = 5;
        Assert.IsTrue(set.HasSelection);
        var dictionary = new ObservableSelectableDictionary<int, int>();
        dictionary.SelectedItem = new(5, 6);
        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(default, dictionary.SelectedItem);
        dictionary.Add(5, 6);
        Assert.IsFalse(dictionary.HasSelection);
        dictionary.SelectedItem = new(5, 6);
        Assert.IsTrue(dictionary.HasSelection);
    }

    [TestMethod]
    public void MissingKey_DoesNotSelectDefaultPair()
    {
        var dictionary = new ObservableSelectableDictionary<int, int>([new(0, 0)]);
        dictionary.SelectedKey = 9;
        Assert.IsFalse(dictionary.HasSelection);
        dictionary.SelectedKey = 0;
        Assert.IsTrue(dictionary.HasSelection);
        dictionary.SelectedItem = new(0, 1);
        Assert.IsFalse(dictionary.HasSelection);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void SetSelection_UsesActualStoredObject(int backend)
    {
        ISet<string> source = backend switch
        {
            0 => new OrderedHashSet<string>(["Actual"], StringComparer.OrdinalIgnoreCase),
            1 => new HashSet<string>(["Actual"], StringComparer.OrdinalIgnoreCase),
            2 => new SortedSet<string>(["Actual"], StringComparer.OrdinalIgnoreCase),
            _ => new DelegatingSet(["Actual"]),
        };
        var set = new ObservableSelectableSetWrapper<string, ISet<string>>(
            source,
            backend == 3 ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal
        );
        set.SelectedItem = "ACTUAL";
        Assert.IsTrue(set.HasSelection);
        Assert.AreSame(source.Single(), set.SelectedItem);
        set.Remove("actual");
        Assert.IsFalse(set.HasSelection);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void DictionarySelection_CanonicalizesKeyAndTracksUpdates(int backend)
    {
        IDictionary<string, int> source = backend switch
        {
            0 => new OrderedDictionary<string, int>(StringComparer.OrdinalIgnoreCase),
            1 => new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase),
            2 => new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase),
            _ => new SortedList<string, int>(StringComparer.OrdinalIgnoreCase),
        };
        source.Add("Actual", 1);
        var dictionary = new ObservableSelectableDictionaryWrapper<
            string,
            int,
            IDictionary<string, int>
        >(source);
        dictionary.SelectedKey = "ACTUAL";
        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreSame(source.Keys.Single(), dictionary.SelectedKey);
        dictionary.ClearSelection();
        dictionary.SelectedItem = new("aCtUaL", 1);
        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreSame(source.Keys.Single(), dictionary.SelectedKey);
        dictionary["actual"] = 2;
        Assert.IsTrue(dictionary.HasSelection);
        Assert.AreEqual(2, dictionary.SelectedValue);
        dictionary.Remove("ACTUAL");
        Assert.IsFalse(dictionary.HasSelection);
        Assert.AreEqual(default, dictionary.SelectedItem);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void SetPredecessorRemoval_NotifiesOnlyIndex(int operation)
    {
        var set = new ObservableSelectableSet<int>([1, 2, 3, 4]);
        set.SelectedItem = 3;
        List<string?> names = [];
        set.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        switch (operation)
        {
            case 0:
                set.Remove(1);
                break;
            case 1:
                set.ExceptWith([1, 2]);
                break;
            case 2:
                set.IntersectWith([3, 4]);
                break;
            default:
                set.SymmetricExceptWith([1, 2, 5]);
                break;
        }
        Assert.AreEqual(operation == 0 ? 1 : 0, set.SelectedIndex);
        Assert.AreEqual(1, names.Count(n => n == "SelectedIndex"));
        Assert.AreEqual(0, names.Count(n => n is "SelectedItem" or "HasSelection"));
    }

    [TestMethod]
    public void DictionaryPredecessorRemoval_NotifiesOnlyIndex()
    {
        var dictionary = new ObservableSelectableDictionary<int, int>([
            new(1, 1),
            new(2, 2),
            new(3, 3),
        ]);
        dictionary.SelectedKey = 2;
        List<string?> names = [];
        dictionary.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        dictionary.Remove(3);
        Assert.AreEqual(0, names.Count(n => n == "SelectedIndex"));
        dictionary.Remove(1);
        Assert.AreEqual(0, dictionary.SelectedIndex);
        Assert.AreEqual(1, names.Count(n => n == "SelectedIndex"));
        Assert.AreEqual(
            0,
            names.Count(n =>
                n is "SelectedItem" or "SelectedKey" or "SelectedValue" or "HasSelection"
            )
        );
    }

    [TestMethod]
    public void ListPredecessorChange_PreservesSelectedOccurrence()
    {
        var list = new ObservableSelectableList<string>(["same", "same"]);
        list.SelectedIndex = 1;
        List<string?> names = [];
        list.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        list.Insert(0, "new");
        Assert.AreEqual(2, list.SelectedIndex);
        list.RemoveAt(0);
        Assert.AreEqual(1, list.SelectedIndex);
        Assert.AreEqual(2, names.Count(n => n == "SelectedIndex"));
        Assert.AreEqual(0, names.Count(n => n is "SelectedItem" or "HasSelection"));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void SelectionOperation_FreezesSubscribersAndRejectsAllWrites(int kind)
    {
        var fixture = CreateFixture(kind);
        List<string?> first = [],
            second = [];
        PropertyChangedEventHandler added = (_, e) => second.Add(e.PropertyName);
        PropertyChangedEventHandler? handler = null;
        handler = (_, e) =>
        {
            first.Add(e.PropertyName);
            fixture.Observable.PropertyChanged -= handler;
            fixture.Observable.PropertyChanged += added;
            Assert.Throws<InvalidOperationException>(fixture.Select);
            Assert.Throws<InvalidOperationException>(fixture.Add);
        };
        fixture.Observable.PropertyChanged += handler;
        fixture.Select();
        Assert.IsTrue(first.Count >= 2);
        Assert.AreEqual(0, second.Count);
        Assert.AreEqual(1, fixture.Count());
        Assert.IsTrue(fixture.HasSelection());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void CollectionCallback_CannotSelectAndThrowingChangedKeepsCompleteState(int kind)
    {
        var fixture = CreateFixture(kind);
        fixture.Select();
        PropertyChangedEventHandler failure = (_, _) =>
        {
            Assert.IsFalse(fixture.HasSelection());
            Assert.AreEqual(0, fixture.Count());
            Assert.Throws<InvalidOperationException>(fixture.Select);
            throw new ApplicationException("callback failure");
        };
        fixture.Observable.PropertyChanged += failure;
        Assert.Throws<ApplicationException>(fixture.Clear);
        fixture.Observable.PropertyChanged -= failure;
        fixture.Add();
        fixture.Select();
        Assert.IsTrue(fixture.HasSelection());
    }

    [TestMethod]
    public void LiteralDefaultPair_DoesNotMeanAbsence()
    {
        Assert.IsTrue(default(KeyValuePair<int, int>).IsDefaultValue);
        Assert.IsFalse(new KeyValuePair<int, int>(0, 1).IsDefaultValue);
        Assert.IsFalse(new KeyValuePair<int, int>(1, 0).IsDefaultValue);
        Assert.IsTrue(default(KeyValuePair<Guid, string?>).IsDefaultValue);
        var dictionary = new ObservableSelectableDictionary<Guid, string?>([new(default, null)]);
        dictionary.SelectedItem = default;
        Assert.IsTrue(dictionary.HasSelection);
        Assert.IsTrue(dictionary.SelectedItem.IsDefaultValue);
        dictionary.ClearSelection();
        Assert.IsFalse(dictionary.HasSelection);
        Assert.IsTrue(dictionary.SelectedItem.IsDefaultValue);
        Assert.AreEqual(1, dictionary.Count);
    }

    [TestMethod]
    public void AllowedNulls_AreSelectedAndExplicitlyCleared()
    {
        var list = new ObservableSelectableList<string?>([null]);
        list.SelectedItem = null;
        Assert.IsTrue(list.HasSelection);
        list.ClearSelection();
        Assert.IsFalse(list.HasSelection);
        Assert.AreEqual(1, list.Count);
        var set = new ObservableSelectableSetWrapper<string, HashSet<string>>(
            new HashSet<string> { null! }
        );
        set.SelectedItem = null!;
        Assert.IsTrue(set.HasSelection);
        set.ClearSelection();
        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(1, set.Count);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void InvalidIndex_PreservesSelectionAndScopeRecovers(int kind)
    {
        if (kind == 0)
        {
            var list = new ObservableSelectableList<int>([0]);
            list.SelectedIndex = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => list.SelectedIndex = -2);
            Assert.AreEqual(0, list.SelectedIndex);
            list.ClearSelection();
            Assert.IsFalse(list.HasSelection);
        }
        else if (kind == 1)
        {
            var set = new ObservableSelectableSet<int>([0]);
            set.SelectedIndex = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => set.SelectedIndex = 1);
            Assert.AreEqual(0, set.SelectedIndex);
            set.ClearSelection();
            Assert.IsFalse(set.HasSelection);
        }
        else
        {
            var dictionary = new ObservableSelectableDictionary<int, int>([new(0, 0)]);
            dictionary.SelectedIndex = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.SelectedIndex = 1);
            Assert.AreEqual(0, dictionary.SelectedIndex);
            dictionary.ClearSelection();
            Assert.IsFalse(dictionary.HasSelection);
        }
    }

    [TestMethod]
    public void SelectionNotifications_AreAccurateForDefaultAndNoOps()
    {
        var dictionary = new ObservableSelectableDictionary<int, int>([new(0, 0)]);
        List<string?> names = [];
        dictionary.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        dictionary.SelectedIndex = 0;
        CollectionAssert.AreEquivalent(
            new[]
            {
                "SelectedItem",
                "SelectedKey",
                "SelectedValue",
                "HasSelection",
                "SelectedIndex",
            },
            names
        );
        names.Clear();
        dictionary.SelectedKey = 0;
        dictionary.SelectedItem = default;
        Assert.AreEqual(0, names.Count);
        dictionary[0] = 1;
        CollectionAssert.AreEqual(
            new[] { "SelectedItem", "SelectedValue", "Item", "Item[]" },
            names
        );
        names.Clear();
        dictionary.ClearSelection();
        CollectionAssert.AreEquivalent(
            new[]
            {
                "SelectedItem",
                "SelectedKey",
                "SelectedValue",
                "HasSelection",
                "SelectedIndex",
            },
            names
        );
        names.Clear();
        dictionary.ClearSelection();
        Assert.AreEqual(0, names.Count);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void BeforeCallback_ReentrantSelectionRejectedAndFailurePreservesState(int kind)
    {
        if (kind == 0)
        {
            var list = new ObservableSelectableList<int>([0]);
            list.SelectedIndex = 0;
            list.ListChanging += (_, _) =>
            {
                Assert.Throws<InvalidOperationException>(list.ClearSelection);
                Assert.Throws<InvalidOperationException>(() => list.SelectedIndex = -1);
                throw new ApplicationException("before failure");
            };
            Assert.Throws<ApplicationException>(() => list.RemoveAt(0));
            Assert.IsTrue(list.HasSelection);
            Assert.AreEqual(0, list.SelectedIndex);
            Assert.AreEqual(1, list.Count);
        }
        else if (kind == 1)
        {
            var set = new ObservableSelectableSet<int>([0]);
            set.SelectedIndex = 0;
            set.SetChanging += (_, _) =>
            {
                Assert.Throws<InvalidOperationException>(set.ClearSelection);
                Assert.Throws<InvalidOperationException>(() => set.SelectedIndex = -1);
                throw new ApplicationException("before failure");
            };
            Assert.Throws<ApplicationException>(() => set.ExceptWith([0]));
            Assert.IsTrue(set.HasSelection);
            Assert.AreEqual(0, set.SelectedIndex);
            Assert.AreEqual(1, set.Count);
        }
        else
        {
            var dictionary = new ObservableSelectableDictionary<int, int>([new(0, 0)]);
            dictionary.SelectedIndex = 0;
            dictionary.DictionaryChanging += (_, _) =>
            {
                Assert.Throws<InvalidOperationException>(dictionary.ClearSelection);
                Assert.Throws<InvalidOperationException>(() => dictionary.SelectedIndex = -1);
                throw new ApplicationException("before failure");
            };
            Assert.Throws<ApplicationException>(() => dictionary.Remove(0));
            Assert.IsTrue(dictionary.HasSelection);
            Assert.AreEqual(0, dictionary.SelectedIndex);
            Assert.AreEqual(1, dictionary.Count);
        }
    }

    [TestMethod]
    public void CollectionOperation_FreezesSelectionPropertySubscribersBeforeChanging()
    {
        var set = new ObservableSelectableSet<int>([1, 2]);
        set.SelectedItem = 2;
        List<string?> early = [],
            late = [];
        PropertyChangedEventHandler first = (_, e) => early.Add(e.PropertyName);
        set.PropertyChanged += first;
        set.SetChanging += (_, _) =>
        {
            set.PropertyChanged -= first;
            set.PropertyChanged += (_, e) => late.Add(e.PropertyName);
        };
        set.Remove(1);
        CollectionAssert.AreEqual(new[] { "SelectedIndex", "Count" }, early);
        Assert.AreEqual(0, late.Count);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void SelectedRemovalByBatch_ClearsOnceAndCanReselect(int operation)
    {
        var set = new ObservableSelectableSet<int>([0, 1, 2]);
        set.SelectedItem = 0;
        List<string?> names = [];
        set.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        switch (operation)
        {
            case 0:
                set.ExceptWith([0, 1]);
                break;
            case 1:
                set.IntersectWith([1, 2]);
                break;
            default:
                set.SymmetricExceptWith([0, 1]);
                break;
        }
        Assert.IsFalse(set.HasSelection);
        Assert.AreEqual(-1, set.SelectedIndex);
        Assert.AreEqual(1, names.Count(n => n == "SelectedIndex"));
        Assert.AreEqual(1, names.Count(n => n == "SelectedItem"));
        Assert.AreEqual(1, names.Count(n => n == "HasSelection"));
        set.Add(0);
        set.SelectedItem = 0;
        Assert.IsTrue(set.HasSelection);
    }

    [TestMethod]
    public void ListSelection_PreservesReferenceAcrossIndexChangesAndReplacement()
    {
        var first = new Item(1);
        var selected = new Item(1);
        var list = new ObservableSelectableList<Item>([first, selected]);
        list.SelectedIndex = 1;
        list.Insert(0, new Item(0));
        Assert.AreSame(selected, list.SelectedItem);
        list.RemoveAt(1);
        Assert.AreSame(selected, list.SelectedItem);
        var replacement = new Item(2);
        list[1] = replacement;
        Assert.AreSame(replacement, list.SelectedItem);
        list.RemoveAt(1);
        Assert.IsFalse(list.HasSelection);
    }

    private sealed record Item(int Value);

    private sealed class DelegatingSet(IEnumerable<string> items) : ISet<string>
    {
        private readonly HashSet<string> _source = new(items, StringComparer.OrdinalIgnoreCase);
        public int Count => _source.Count;
        public bool IsReadOnly => false;

        public bool Add(string item) => _source.Add(item);

        void ICollection<string>.Add(string item) => Add(item);

        public bool Remove(string item) => _source.Remove(item);

        public void Clear() => _source.Clear();

        public bool Contains(string item) => _source.Contains(item);

        public void CopyTo(string[] array, int index) => _source.CopyTo(array, index);

        public IEnumerator<string> GetEnumerator() => _source.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public void ExceptWith(IEnumerable<string> other) => _source.ExceptWith(other);

        public void IntersectWith(IEnumerable<string> other) => _source.IntersectWith(other);

        public void SymmetricExceptWith(IEnumerable<string> other) =>
            _source.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<string> other) => _source.UnionWith(other);

        public bool IsProperSubsetOf(IEnumerable<string> other) => _source.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<string> other) =>
            _source.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<string> other) => _source.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<string> other) => _source.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<string> other) => _source.Overlaps(other);

        public bool SetEquals(IEnumerable<string> other) => _source.SetEquals(other);
    }

    private static (
        INotifyPropertyChanged Observable,
        Action Select,
        Action Add,
        Action Clear,
        Func<int> Count,
        Func<bool> HasSelection
    ) CreateFixture(int kind)
    {
        if (kind == 0)
        {
            var list = new ObservableSelectableList<int>([1]);
            return (
                list,
                () => list.SelectedIndex = 0,
                () => list.Add(1),
                list.Clear,
                () => list.Count,
                () => list.HasSelection
            );
        }
        if (kind == 1)
        {
            var set = new ObservableSelectableSet<int>([1]);
            return (
                set,
                () => set.SelectedItem = 1,
                () => set.Add(1),
                set.Clear,
                () => set.Count,
                () => set.HasSelection
            );
        }
        var dictionary = new ObservableSelectableDictionary<int, int>([new(1, 1)]);
        return (
            dictionary,
            () => dictionary.SelectedKey = 1,
            () => dictionary.TryAdd(1, 1),
            dictionary.Clear,
            () => dictionary.Count,
            () => dictionary.HasSelection
        );
    }
}
