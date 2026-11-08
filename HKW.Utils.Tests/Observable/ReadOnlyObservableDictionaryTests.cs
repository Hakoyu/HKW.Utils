using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ReadOnlyObservableDictionaryTests
{
    [TestMethod]
    public void ReadOnlyInterfaces()
    {
        var source = new ObservableDictionary<int, int>(
            new Dictionary<int, int> { [1] = 10, [2] = 20 }
        );
        using var view = new ReadOnlyObservableDictionary<int, int>(source);
        var notifications = 0;
        view.DictionaryChanging += (_, _) => notifications++;
        view.DictionaryChanged += (_, _) => notifications++;
        view.CollectionChanged += (_, _) => notifications++;
        view.PropertyChanged += (_, _) => notifications++;
        IReadOnlyDictionaryTTestUtils.Test<int, int>(
            () => view,
            new[] { KeyValuePair.Create(3, 30), KeyValuePair.Create(4, 40) }
        );
        Assert.AreEqual(0, notifications);
        CollectionAssert.AreEquivalent(
            new[] { KeyValuePair.Create(1, 10), KeyValuePair.Create(2, 20) },
            source.ToArray()
        );
    }

    [TestMethod]
    public void ContainsPair_RequiresMatchingValue()
    {
        var source = new ObservableDictionary<int, int>(new Dictionary<int, int> { [1] = 10 });
        using var view = new ReadOnlyObservableDictionary<int, int>(source);
        ICollection<KeyValuePair<int, int>> collection = view;
        Assert.IsTrue(collection.Contains(KeyValuePair.Create(1, 10)));
        Assert.IsFalse(collection.Contains(KeyValuePair.Create(1, 99)));
    }

    [TestMethod]
    [DataRow(DictionaryChangeAction.Add)]
    [DataRow(DictionaryChangeAction.Replace)]
    [DataRow(DictionaryChangeAction.Remove)]
    [DataRow(DictionaryChangeAction.Clear)]
    public void ForwardsChanges(DictionaryChangeAction action)
    {
        var initial = new Dictionary<int, int> { [1] = 10, [2] = 20 };
        var expected = new Dictionary<int, int>(initial);
        var source = new ObservableDictionary<int, int>(initial);
        using var view = new ReadOnlyObservableDictionary<int, int>(source);
        var changing = new List<NotifyDictionaryChangeEventArgs<int, int>>();
        var changed = new List<NotifyDictionaryChangeEventArgs<int, int>>();
        var sourceCollection =
            new List<System.Collections.Specialized.NotifyCollectionChangedEventArgs>();
        var viewCollection =
            new List<System.Collections.Specialized.NotifyCollectionChangedEventArgs>();
        var sourceProperties = new List<string?>();
        var viewProperties = new List<string?>();
        var order = new List<string>();
        source.CollectionChanged += (_, e) => sourceCollection.Add(e);
        source.PropertyChanged += (_, e) => sourceProperties.Add(e.PropertyName);
        view.DictionaryChanging += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            CollectionAssert.AreEquivalent(initial.ToArray(), view.ToArray());
            changing.Add(e);
            order.Add("Changing");
        };
        view.DictionaryChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            CollectionAssert.AreEquivalent(expected.ToArray(), view.ToArray());
            changed.Add(e);
            order.Add("Changed");
        };
        view.CollectionChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            viewCollection.Add(e);
        };
        view.PropertyChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            viewProperties.Add(e.PropertyName);
        };
        KeyValuePair<int, int>? oldPair = null;
        KeyValuePair<int, int>? newPair = null;
        switch (action)
        {
            case DictionaryChangeAction.Add:
                newPair = KeyValuePair.Create(3, 30);
                expected.Add(3, 30);
                source.Add(3, 30);
                break;
            case DictionaryChangeAction.Replace:
                oldPair = KeyValuePair.Create(1, 10);
                newPair = KeyValuePair.Create(1, 99);
                expected[1] = 99;
                source[1] = 99;
                break;
            case DictionaryChangeAction.Remove:
                oldPair = KeyValuePair.Create(1, 10);
                expected.Remove(1);
                Assert.IsTrue(source.Remove(1));
                break;
            case DictionaryChangeAction.Clear:
                expected.Clear();
                source.Clear();
                break;
            default:
                Assert.Fail("Unsupported test action.");
                break;
        }
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        CollectionAssert.AreEqual(new[] { "Changing", "Changed" }, order);
        foreach (var e in changing.Concat(changed))
        {
            Assert.AreEqual(action, e.Action);
            Assert.AreEqual(oldPair, e.OldPair);
            Assert.AreEqual(newPair, e.NewPair);
        }
        Assert.AreEqual(1, sourceCollection.Count);
        Assert.AreEqual(1, viewCollection.Count);
        Assert.AreSame(sourceCollection[0], viewCollection[0]);
        var expectedProperties =
            action == DictionaryChangeAction.Replace
                ? new[] { "Item", "Item[]" }
                : new[] { "Count", "Item", "Item[]" };
        CollectionAssert.AreEqual(
            expectedProperties,
            sourceProperties,
            $"Source notifications: {string.Join(", ", sourceProperties)}"
        );
        CollectionAssert.AreEqual(
            expectedProperties,
            viewProperties,
            $"View notifications: {string.Join(", ", viewProperties)}"
        );
        CollectionAssert.AreEquivalent(expected.ToArray(), view.ToArray());
        Assert.IsFalse(source.Remove(-1));
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(1, viewCollection.Count);
        CollectionAssert.AreEqual(expectedProperties, sourceProperties);
        CollectionAssert.AreEqual(expectedProperties, viewProperties);
    }

    [TestMethod]
    public void ObservableKeysAndValues_ForwardAdd()
    {
        var source = new ObservableDictionary<int, int>();
        using var view = new ReadOnlyObservableDictionary<int, int>(source);
        var keys = view.ObservableKeys;
        var values = view.ObservableValues;
        var keyCount = 0;
        var valueCount = 0;
        keys.CollectionChanged += (sender, e) =>
        {
            Assert.AreSame(keys, sender);
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Add,
                e.Action
            );
            CollectionAssert.AreEqual(new[] { 1 }, e.NewItems!.Cast<int>().ToArray());
            keyCount++;
        };
        values.CollectionChanged += (sender, e) =>
        {
            Assert.AreSame(values, sender);
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Add,
                e.Action
            );
            CollectionAssert.AreEqual(new[] { 10 }, e.NewItems!.Cast<int>().ToArray());
            valueCount++;
        };
        source.Add(1, 10);
        Assert.AreEqual(1, keyCount);
        Assert.AreEqual(1, valueCount);
        CollectionAssert.AreEqual(new[] { 1 }, keys.ToArray());
        CollectionAssert.AreEqual(new[] { 10 }, values.ToArray());
    }

    [TestMethod]
    public void Dispose_StopsAllForwardingWithoutDisposingSource()
    {
        var source = new ObservableDictionary<int, int>();
        using var view = new ReadOnlyObservableDictionary<int, int>(source);
        var count = 0;
        view.DictionaryChanging += (_, _) => count++;
        view.DictionaryChanged += (_, _) => count++;
        view.CollectionChanged += (_, _) => count++;
        view.PropertyChanged += (_, _) => count++;
        view.ObservableKeys.CollectionChanged += (_, _) => count++;
        view.ObservableValues.CollectionChanged += (_, _) => count++;
        view.Dispose();
        view.Dispose();
        source.Add(1, 10);
        Assert.AreEqual(0, count);
        Assert.AreEqual(10, source[1]);
    }

    // 以下保留历史草稿；当前可执行契约测试位于上方。
    //[TestMethod]
    //public void Adding()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var newPair = KeyValuePair.Create(10, 10);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?[0].Equals(newPair));
    //        Assert.IsTrue(e.OldPair == null);
    //    };
    //    observableDictionary.Add(10, 10);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Adding_Cancel()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var newPair = KeyValuePair.Create(10, 10);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?[0].Equals(newPair));
    //        Assert.IsTrue(e.OldPair == null);
    //        e.Cancel = true;
    //    };
    //    observableDictionary.Add(10, 10);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Added()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?[0].Key == 10);
    //        Assert.IsTrue(e.NewPair?[0].Value == 10);
    //        Assert.IsTrue(e.NewPair == null);
    //    };
    //    observableDictionary.Add(10, 10);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void AddingRange()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?.Count == 5);
    //        Assert.IsTrue(e.NewPair.ItemsEqual(addPairs));
    //        Assert.IsTrue(e.OldPair == null);
    //    };
    //    observableDictionary.AddRange(addPairs);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 15);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void AddingRange_Cancel()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?.Count == 5);
    //        Assert.IsTrue(e.NewPair.ItemsEqual(addPairs));
    //        Assert.IsTrue(e.OldPair == null);
    //        e.Cancel = true;
    //    };
    //    observableDictionary.AddRange(addPairs);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void AddedRange()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
    //    readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
    //        Assert.IsTrue(e.NewPair?.Count == 5);
    //        Assert.IsTrue(e.NewPair.ItemsEqual(addPairs));
    //        Assert.IsTrue(e.NewPair == null);
    //    };
    //    observableDictionary.AddRange(addPairs);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 15);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Removing()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var newPair = KeyValuePair.Create(0, 0);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
    //        Assert.IsTrue(e.OldPair?[0].Equals(newPair));
    //        Assert.IsTrue(e.NewPair == null);
    //    };
    //    observableDictionary.Remove(0);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Removing_Cancel()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var newPair = KeyValuePair.Create(0, 0);
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
    //        Assert.IsTrue(e.OldPair?[0].Equals(newPair));
    //        Assert.IsTrue(e.NewPair == null);
    //        e.Cancel = true;
    //    };
    //    observableDictionary.Remove(0);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Removed()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var newPair = KeyValuePair.Create(0, 0);
    //    readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
    //        Assert.IsTrue(e.NewPair == null);
    //        Assert.IsTrue(e.NewPair?[0].Equals(newPair));
    //    };
    //    observableDictionary.Remove(newPair);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Clearing()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
    //        Assert.IsTrue(e.OldPair == null);
    //        Assert.IsTrue(e.NewPair == null);
    //    };
    //    observableDictionary.Clear();
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Clearing_Cancel()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
    //        Assert.IsTrue(e.OldPair == null);
    //        Assert.IsTrue(e.NewPair == null);
    //        e.Cancel = true;
    //    };
    //    observableDictionary.Clear();
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void Cleared()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
    //        Assert.IsTrue(e.NewPair == null);
    //        Assert.IsTrue(e.NewPair == null);
    //    };
    //    observableDictionary.Clear();
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void ValueChanging()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
    //        Assert.IsTrue(e.NewPair?[0].Key == 0);
    //        Assert.IsTrue(e.NewPair?[0].Value == 10);
    //        Assert.IsTrue(e.OldPair?[0].Key == 0);
    //        Assert.IsTrue(e.OldPair?[0].Value == 0);
    //    };
    //    observableDictionary[0] = 10;
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(readOnlyObservableDictionary[0] == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void ValueChanging_Cancel()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
    //        Assert.IsTrue(e.NewPair?[0].Key == 0);
    //        Assert.IsTrue(e.NewPair?[0].Value == 10);
    //        Assert.IsTrue(e.OldPair?[0].Key == 0);
    //        Assert.IsTrue(e.OldPair?[0].Value == 0);
    //        e.Cancel = true;
    //    };
    //    observableDictionary[0] = 10;
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(readOnlyObservableDictionary[0] == 0);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void ValueChanged()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
    //        Assert.IsTrue(e.NewPair?[0].Key == 0);
    //        Assert.IsTrue(e.NewPair?[0].Value == 10);
    //        Assert.IsTrue(e.NewPair?[0].Key == 0);
    //        Assert.IsTrue(e.NewPair?[0].Value == 0);
    //    };
    //    observableDictionary[0] = 10;
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
    //    Assert.IsTrue(readOnlyObservableDictionary[0] == 10);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void CollectionChanged_Add()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var pair = new KeyValuePair<int, int>(10, 10);
    //    readOnlyObservableDictionary.CollectionChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(
    //            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
    //        );
    //        Assert.IsTrue(e.OldItems?[0] is null);
    //        Assert.IsTrue(e.NewItems?[0]?.Equals(pair) is true);
    //        Assert.IsTrue(e.NewStartingIndex == -1);
    //        Assert.IsTrue(e.OldStartingIndex == -1);
    //    };
    //    observableDictionary.Add(pair);
    //    Assert.IsTrue(readOnlyObservableDictionary.Last().Equals(pair) is true);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void CollectionChanged_Remove()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    var pair = new KeyValuePair<int, int>(0, 0);
    //    readOnlyObservableDictionary.CollectionChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(
    //            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
    //        );
    //        Assert.IsTrue(e.OldItems?[0]?.Equals(pair) is true);
    //        Assert.IsTrue(e.NewItems?[0] is null);
    //        Assert.IsTrue(e.NewStartingIndex == -1);
    //        Assert.IsTrue(e.OldStartingIndex == -1);
    //    };
    //    observableDictionary.Remove(pair);
    //    Assert.IsTrue(readOnlyObservableDictionary.First().Key == 1);
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
    //    Assert.IsTrue(triggered);
    //}

    //[TestMethod]
    //public void CollectionChanged_Reset()
    //{
    //    var triggered = false;
    //    var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
    //    var observableDictionary = new ObservableDictionary<int, int>(dictionary);
    //    var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
    //        observableDictionary
    //    );
    //    readOnlyObservableDictionary.CollectionChanged += (s, e) =>
    //    {
    //        triggered = true;
    //        Assert.IsTrue(
    //            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
    //        );
    //        Assert.IsTrue(e.OldItems?[0] is null);
    //        Assert.IsTrue(e.NewItems?[0] is null);
    //        Assert.IsTrue(e.NewStartingIndex == -1);
    //        Assert.IsTrue(e.OldStartingIndex == -1);
    //    };
    //    observableDictionary.Clear();
    //    Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
    //    Assert.IsTrue(triggered);
    //}
}
