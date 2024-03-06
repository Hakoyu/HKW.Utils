using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ReadOnlyObservableDictionaryTests
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var newPair = KeyValuePair.Create(10, 10);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Equals(newPair));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Adding_Cancel()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var newPair = KeyValuePair.Create(10, 10);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Equals(newPair));
            Assert.IsTrue(e.OldItems == null);
            e.Cancel = true;
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Added()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Key == 10);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddingRange()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 15);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddingRange_Cancel()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
            e.Cancel = true;
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddedRange()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 15);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var newPair = KeyValuePair.Create(0, 0);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
            Assert.IsTrue(e.NewItems == null);
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing_Cancel()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var newPair = KeyValuePair.Create(0, 0);
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
            Assert.IsTrue(e.NewItems == null);
            e.Cancel = true;
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var newPair = KeyValuePair.Create(0, 0);
        readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
        };
        observableDictionary.Remove(newPair);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems == null);
        };
        observableDictionary.Clear();
        Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing_Cancel()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems == null);
            e.Cancel = true;
        };
        observableDictionary.Clear();
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Cleared()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Clear();
        Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(readOnlyObservableDictionary[0] == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging_Cancel()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
            e.Cancel = true;
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(readOnlyObservableDictionary[0] == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanged()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(readOnlyObservableDictionary.Count == 10);
        Assert.IsTrue(readOnlyObservableDictionary[0] == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var pair = new KeyValuePair<int, int>(10, 10);
        readOnlyObservableDictionary.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0]?.Equals(pair) is true);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableDictionary.Add(pair);
        Assert.IsTrue(readOnlyObservableDictionary.Last().Equals(pair) is true);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        var pair = new KeyValuePair<int, int>(0, 0);
        readOnlyObservableDictionary.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.IsTrue(e.OldItems?[0]?.Equals(pair) is true);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableDictionary.Remove(pair);
        Assert.IsTrue(readOnlyObservableDictionary.First().Key == 1);
        Assert.IsTrue(readOnlyObservableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var dictionary = Enumerable.Range(0, 10).ToDictionary(i => i, i => i);
        var observableDictionary = new ObservableDictionary<int, int>(dictionary);
        var readOnlyObservableDictionary = new ReadOnlyObservableDictionary<int, int>(
            observableDictionary
        );
        readOnlyObservableDictionary.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableDictionary.Clear();
        Assert.IsTrue(readOnlyObservableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }
}
