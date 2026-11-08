using System.Collections;
using System.Collections.ObjectModel;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class CollectionInterfaceContractTests
{
    private static IList CreateList<T>(int kind, params T[] items)
    {
        IList<T> source = new GenericOnlyList<T>(items.ToList());
        return kind switch
        {
            0 => new HKW.HKWUtils.Collections.ReadOnlyList<T>(items.ToList()),
            1 => new HKW.HKWUtils.Collections.ReadOnlyList<T>(source),
            2 => new SingleItemReadOnlyList<T>(items[0]),
            3 => new ReadOnlyObservableList<T>(new ObservableList<T>(items)),
            4 => new ReadOnlyObservableList<T>(new GenericOnlyObservableList<T>(source)),
            5 => new ObservableListWrapper<T, IList<T>>(items.ToList()),
            6 => new ObservableListWrapper<T, IList<T>>(source),
            7 => new FilteredListWrapper<T, IList<T>, List<T>>(source, [], _ => true),
            8 => new ObservableFilteredListWrapper<T, IObservableList<T>, ObservableList<T>>(
                new ObservableListWrapper<T, IList<T>>(source),
                new(),
                _ => true
            ),
            9 => items.ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    [DataRow(8)]
    public void ListQueriesRespectNullAndType(int kind)
    {
        CollectionInterfaceContract.Queries(CreateList(kind, 1), 1);
        CollectionInterfaceContract.Queries(
            CreateList<string?>(kind, (string?)null),
            (string?)null
        );
        CollectionInterfaceContract.Queries(CreateList<int?>(kind, (int?)null), (int?)null);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    [DataRow(8)]
    public void ListCopyArrayMatrix(int kind)
    {
        CollectionInterfaceContract.Copy(CreateList(kind, 7), [7], typeof(int));
        CollectionInterfaceContract.Copy(
            CreateList<string?>(kind, (string?)null),
            [null],
            typeof(string)
        );
        CollectionInterfaceContract.Copy(CreateList<int?>(kind, (int?)null), [null], typeof(int?));
        var referenceList = CreateList<object>(kind, "value");
        var strings = new string[1];
        referenceList.CopyTo(strings, 0);
        Assert.AreEqual("value", strings[0]);
        Assert.ThrowsExactly<ArgumentException>(() =>
            CreateList<object>(kind, new object()).CopyTo(new string[1], 0)
        );
        Assert.ThrowsExactly<ArgumentException>(() => CreateList(kind, 7).CopyTo(new string[1], 0));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void ReadOnlyListsRejectAllWrites(int kind)
    {
        var list = CreateList(kind, 7);
        CollectionInterfaceContract.ReadOnly((ICollection<int>)list, 7);
        Assert.IsTrue(list.IsReadOnly);
        Assert.IsTrue(list.IsFixedSize);
        foreach (var value in new object?[] { 7, null, "wrong" })
        {
            Assert.ThrowsExactly<NotSupportedException>(() => list.Add(value));
            Assert.ThrowsExactly<NotSupportedException>(() => list.Insert(0, value));
            Assert.ThrowsExactly<NotSupportedException>(() => list[0] = value);
            Assert.ThrowsExactly<NotSupportedException>(() => list.Remove(value));
        }
        Assert.ThrowsExactly<NotSupportedException>(() => list.RemoveAt(0));
        Assert.ThrowsExactly<NotSupportedException>(list.Clear);
        Assert.AreEqual(7, list[0]);
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    [DataRow(8)]
    public void WritableListsValidateAndMutateThroughGenericPath(int kind)
    {
        var list = CreateList(kind, 7);
        Assert.IsFalse(list.IsReadOnly);
        Assert.IsFalse(list.IsFixedSize);
        list.Remove("wrong");
        list.Remove(null);
        Assert.AreEqual(1, list.Count);
        Assert.ThrowsExactly<ArgumentException>(() => list.Add("wrong"));
        Assert.ThrowsExactly<ArgumentException>(() => list.Insert(0, "wrong"));
        Assert.ThrowsExactly<ArgumentException>(() => list[0] = "wrong");
        Assert.ThrowsExactly<ArgumentNullException>(() => list.Add(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => list.Insert(0, null));
        Assert.ThrowsExactly<ArgumentNullException>(() => list[0] = null);
        Assert.AreEqual(1, list.Add(8));
        list.Insert(0, 6);
        list[1] = 9;
        list.Remove(8);
        CollectionAssert.AreEqual(new[] { 6, 9 }, list.Cast<int>().ToArray());
        var nullable = CreateList<int?>(kind, 1);
        Assert.AreEqual(1, nullable.Add(null));
        nullable.Remove(null);
        Assert.AreEqual(1, nullable.Count);
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    [DataRow(8)]
    [DataRow(9)]
    public void SharedWritableListSuite(int kind)
    {
        IListTestUtils.Test(() => CreateList(kind, 1, 2, 3), new[] { 4, 5, 6 });
    }

    [TestMethod]
    public void SingleItemQueriesUseGenericEquality()
    {
        var value = new TypedEquality(1);
        IList list = new SingleItemReadOnlyList<TypedEquality>(value);
        Assert.IsTrue(list.Contains(new TypedEquality(1)));
        Assert.AreEqual(0, list.IndexOf(new TypedEquality(1)));
        Assert.IsFalse(list.Contains("wrong"));
        Assert.AreEqual(-1, list.IndexOf(null));
    }

    private sealed class TypedEquality(int value) : IEquatable<TypedEquality>
    {
        private readonly int _value = value;

        public bool Equals(TypedEquality? other) => other is not null && other._value == _value;
    }

    [TestMethod]
    public void MutableBridgePreservesNotificationsAndFiltering()
    {
        var source = new ObservableList<int>([1]);
        IList list = source;
        var changes = 0;
        source.ListChanged += (_, _) => changes++;
        list.Remove("wrong");
        Assert.ThrowsExactly<ArgumentException>(() => list.Add("wrong"));
        Assert.AreEqual(0, changes);
        source.ListChanging += (_, _) =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => list.Remove("wrong"));
            Assert.ThrowsExactly<InvalidOperationException>(() => list.Add("wrong"));
            Assert.ThrowsExactly<InvalidOperationException>(() => list.Insert(0, null));
            Assert.ThrowsExactly<InvalidOperationException>(() => list[0] = "wrong");
        };
        list.Add(2);
        Assert.AreEqual(1, changes);
        var target = new List<int>();
        var filter = new FilteredListWrapper<int, IList<int>, List<int>>(
            new GenericOnlyList<int>(new List<int>()),
            target,
            x => x % 2 == 0
        );
        IList bridge = filter;
        bridge.Add(1);
        bridge.Add(2);
        bridge[0] = 4;
        bridge.Remove(2);
        CollectionAssert.AreEqual(new[] { 4 }, target.ToArray());
    }

    [TestMethod]
    public void ReadOnlyViewIsLiveButEnumerableInputIsSnapshot()
    {
        var source = new GenericOnlyList<int>(new List<int> { 1 });
        IList view = new HKW.HKWUtils.Collections.ReadOnlyList<int>(source);
        IList snapshot = new HKW.HKWUtils.Collections.ReadOnlyList<int>(source.Where(_ => true));
        source.Add(2);
        Assert.AreEqual(2, view.Count);
        Assert.AreEqual(1, snapshot.Count);
        Assert.IsTrue(view.IsFixedSize);
        Assert.AreSame(
            ((ICollection)view).SyncRoot,
            ((ICollection)new HKW.HKWUtils.Collections.ReadOnlyList<int>(source)).SyncRoot
        );
    }

    [TestMethod]
    public void GenericValueBackendHasStableSyncRoot()
    {
        var source = new GenericValueList<int>(new List<int> { 1 });
        IList observable = new ObservableListWrapper<int, GenericValueList<int>>(source);
        CollectionInterfaceContract.Copy(observable, [1], typeof(int));
        Assert.AreSame(observable, observable.SyncRoot);
        IList filter = new FilteredListWrapper<int, GenericValueList<int>, List<int>>(
            source,
            [],
            _ => true
        );
        CollectionInterfaceContract.Copy(filter, [1], typeof(int));
        Assert.AreSame(filter, filter.SyncRoot);
    }

    [TestMethod]
    public void BackendMetadataAndUndoableContracts()
    {
        IList<int>[] backends =
        [
            new List<int> { 1 },
            new GenericOnlyList<int>(new List<int> { 1 }),
            new ReadOnlyCollection<int>(new List<int> { 1 }),
            new GenericOnlyList<int>(new ReadOnlyCollection<int>(new List<int> { 1 })),
            new[] { 1 },
        ];
        foreach (var source in backends)
        {
            var observable = new ObservableListWrapper<int, IList<int>>(source);
            var undoable = new UndoableListWrapper<int, IList<int>>(source);
            Assert.AreEqual(source.IsReadOnly, observable.IsReadOnly);
            Assert.AreEqual(source.IsReadOnly, undoable.IsReadOnly);
            Assert.AreEqual(
                source is IList list ? list.IsFixedSize : source.IsReadOnly,
                ((IList)observable).IsFixedSize
            );
            CollectionInterfaceContract.Copy((ICollection)observable, [1], typeof(int));
            Assert.AreEqual(0, undoable.IndexOf(1));
            var copy = new int[1];
            undoable.CopyTo(copy, 0);
            CollectionAssert.AreEqual(new[] { 1 }, copy);
            if (source.IsReadOnly)
            {
                Assert.ThrowsExactly<NotSupportedException>(() => observable.Add(2));
                Assert.ThrowsExactly<NotSupportedException>(() => undoable.Add(2));
            }
        }
        var readOnlySet = new ReadOnlyObservableSet<int>(new ObservableSet<int>([1]));
        var set = new ObservableSetWrapper<int, ISet<int>>(readOnlySet, null);
        Assert.IsTrue(set.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => set.Add(2));
        CollectionInterfaceContract.ReadOnly((ICollection<int>)readOnlySet, 1);
        var dictionary = new ReadOnlyObservableDictionary<int, int>(
            new ObservableDictionary<int, int> { [1] = 2 }
        );
        var wrapped = new ObservableDictionaryWrapper<int, int, IDictionary<int, int>>(
            dictionary,
            null
        );
        Assert.IsTrue(wrapped.IsReadOnly);
        CollectionInterfaceContract.ReadOnly(dictionary, new KeyValuePair<int, int>(1, 2));
        CollectionInterfaceContract.ReadOnly(dictionary.ObservableKeys, 1);
        CollectionInterfaceContract.ReadOnly(dictionary.ObservableValues, 2);
    }

    private static IDictionary CreateDictionary<TValue>(bool genericOnly, TValue value)
    {
        IDictionary<string, TValue> source = new Dictionary<string, TValue>(
            StringComparer.OrdinalIgnoreCase
        )
        {
            ["Key"] = value,
        };
        if (genericOnly)
            source = new GenericOnlyDictionary<string, TValue>(source);
        return new FilteredDictionaryWrapper<
            string,
            TValue,
            IDictionary<string, TValue>,
            Dictionary<string, TValue>
        >(source, new(), _ => true);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void DictionaryQueryWriteAndEnumerationMatrix(bool genericOnly)
    {
        var dictionary = CreateDictionary(genericOnly, 1);
        Assert.IsFalse(dictionary.IsReadOnly);
        Assert.IsFalse(dictionary.IsFixedSize);
        Assert.AreEqual(1, dictionary["KEY"]);
        Assert.IsNull(dictionary["missing"]);
        Assert.IsNull(dictionary[123]);
        Assert.IsFalse(dictionary.Contains(123));
        Assert.IsTrue(dictionary.Contains("KEY"));
        dictionary.Remove(123);
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Contains(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => _ = dictionary[null!]);
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Remove(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Add(null!, 2));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Add("null", null));
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.Add(123, 2));
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.Add("other", "wrong"));
        Assert.ThrowsExactly<ArgumentException>(() => dictionary[123] = 2);
        Assert.ThrowsExactly<ArgumentException>(() => dictionary["other"] = "wrong");
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary[null!] = 2);
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary["other"] = null);
        Assert.AreEqual(1, dictionary.Count);
        var keys = dictionary.Keys;
        var values = dictionary.Values;
        CollectionInterfaceContract.Copy(keys, ["Key"], typeof(string));
        CollectionInterfaceContract.Copy(values, [1], typeof(int));
        var enumerator = dictionary.GetEnumerator();
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = enumerator.Entry);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(new DictionaryEntry("Key", 1), enumerator.Entry);
        Assert.AreEqual("Key", enumerator.Key);
        Assert.AreEqual(1, enumerator.Value);
        Assert.AreEqual(enumerator.Entry, enumerator.Current);
        Assert.IsFalse(enumerator.MoveNext());
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = enumerator.Current);
        enumerator.Reset();
        Assert.IsTrue(enumerator.MoveNext());
        (enumerator as IDisposable)?.Dispose();
        dictionary.Add("other", 2);
        Assert.AreEqual(2, keys.Count);
        Assert.AreEqual(2, values.Count);
        dictionary["KEY"] = 3;
        dictionary.Remove("OTHER");
        Assert.AreEqual(3, dictionary["Key"]);
        var nullable = CreateDictionary<int?>(genericOnly, null);
        nullable.Add("other", null);
        Assert.IsTrue(nullable.Contains("other"));
        Assert.IsNull(nullable["other"]);
        var reference = CreateDictionary<string?>(genericOnly, null);
        reference["other"] = null;
        Assert.IsTrue(reference.Contains("other"));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void DictionaryCopyMatrix(bool genericOnly)
    {
        var dictionary = CreateDictionary(genericOnly, 1);
        CollectionInterfaceContract.Copy(
            dictionary,
            [new KeyValuePair<string, int>("Key", 1)],
            typeof(KeyValuePair<string, int>)
        );
        var entries = new DictionaryEntry[2];
        dictionary.CopyTo(entries, 1);
        Assert.AreEqual(new DictionaryEntry("Key", 1), entries[1]);
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.CopyTo(new string[1], 0));
    }

    [TestMethod]
    public void ObservableFilteredDictionaryUsesGenericBridgeAndPreservesScope()
    {
        var backend = new GenericOnlyDictionary<string, int>(
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Key"] = 2 }
        );
        var source = new ObservableDictionaryWrapper<string, int, IDictionary<string, int>>(
            backend,
            StringComparer.OrdinalIgnoreCase
        );
        var target = new ObservableDictionary<string, int>();
        using var wrapper = new ObservableFilteredDictionaryWrapper<
            string,
            int,
            IObservableDictionary<string, int>,
            ObservableDictionary<string, int>
        >(source, target, x => x.Value % 2 == 0);
        IDictionary dictionary = wrapper;
        Assert.IsNull(dictionary[1]);
        Assert.IsNull(dictionary["missing"]);
        Assert.AreEqual(2, dictionary["KEY"]);
        Assert.IsFalse(dictionary.IsFixedSize);
        CollectionInterfaceContract.Copy(
            dictionary,
            [new KeyValuePair<string, int>("Key", 2)],
            typeof(KeyValuePair<string, int>)
        );
        CollectionInterfaceContract.Copy(dictionary.Keys, ["Key"], typeof(string));
        CollectionInterfaceContract.Copy(dictionary.Values, [2], typeof(int));
        source.DictionaryChanging += (_, _) =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => dictionary.Add(1, "wrong"));
            Assert.ThrowsExactly<InvalidOperationException>(() => dictionary[null!] = 1);
            Assert.ThrowsExactly<InvalidOperationException>(() => dictionary.Remove(1));
        };
        dictionary.Add("other", 4);
        dictionary["KEY"] = 3;
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(4, target["other"]);
        dictionary.Remove("OTHER");
        Assert.AreEqual(0, target.Count);
        wrapper.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => dictionary.Add(1, "wrong"));
    }

    [TestMethod]
    public void GenericReadOnlyBackendsPreserveTypedQueries()
    {
        var dictionary = new GenericOnlyDictionary<string, int>(
            new ReadOnlyDictionary<string, int>(
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Key"] = 2 }
            )
        );
        var wrapper = new ObservableDictionaryWrapper<string, int, IDictionary<string, int>>(
            dictionary,
            StringComparer.OrdinalIgnoreCase
        );
        using var view = new ReadOnlyObservableDictionary<string, int>(wrapper);
        CollectionInterfaceContract.ReadOnly(view, new KeyValuePair<string, int>("KEY", 2));
        CollectionInterfaceContract.ReadOnly(view.ObservableKeys, "KEY");
        CollectionInterfaceContract.ReadOnly(view.ObservableValues, 2);
        Assert.IsFalse(((ICollection<KeyValuePair<string, int>>)view).Contains(new("KEY", 3)));
        var writableSet = new ObservableSetWrapper<int, ISet<int>>(new HashSet<int> { 1 }, null);
        Assert.IsFalse(writableSet.IsReadOnly);
        using var setView = new ReadOnlyObservableSet<int>(writableSet);
        CollectionInterfaceContract.ReadOnly(setView, 1);
    }

    [TestMethod]
    public void StackAndSingleItemTypedCopies()
    {
        CollectionInterfaceContract.Copy(
            new ReadOnlyStack<int>(new Stack<int>([1, 2])),
            [2, 1],
            typeof(int)
        );
        var single = new SingleItemReadOnlyList<int>(7);
        Assert.ThrowsExactly<ArgumentNullException>(() => single.CopyTo(null!, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => single.CopyTo(new int[1], -1));
        Assert.ThrowsExactly<ArgumentException>(() => single.CopyTo([], 0));
        var typed = new int[2];
        single.CopyTo(typed, 1);
        CollectionAssert.AreEqual(new[] { 0, 7 }, typed);
    }

    [TestMethod]
    public void EmptyCopiesValidateArrayTypeAndEndIndex()
    {
        foreach (var kind in new[] { 0, 1, 3, 4, 5, 6, 7, 8 })
        {
            var list = CreateList<int>(kind);
            list.CopyTo(Array.Empty<object>(), 0);
            list.CopyTo(new object[1], 1);
            Assert.ThrowsExactly<ArgumentException>(() => list.CopyTo(Array.Empty<string>(), 0));
        }
    }
}
