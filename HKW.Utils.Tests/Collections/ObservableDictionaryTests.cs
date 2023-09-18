using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKWTests.Collections;

[TestClass]
public class ObservableDictionaryTests
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(a.NewPairs?[0].Key == 10);
            Assert.IsTrue(a.NewPairs?[0].Value == 10);
            Assert.IsNull(a.OldPairs);
            a.Cancel = true;
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Added()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanged += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(a.NewPairs?[0].Key == 10);
            Assert.IsTrue(a.NewPairs?[0].Value == 10);
            Assert.IsNull(a.OldPairs);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(observableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddRange()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        observableDictionary.DictionaryChanged += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(a.NewPairs?.Count == 5);
            Assert.IsTrue(a.NewPairs.ItemsEqual(addPairs));
            Assert.IsNull(a.OldPairs);
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(observableDictionary.Count == 15);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(a.OldPairs?[0].Key == 0);
            Assert.IsTrue(a.OldPairs?[0].Value == 0);
            Assert.IsNull(a.NewPairs);
            a.Cancel = true;
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanged += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(a.OldPairs?[0].Key == 0);
            Assert.IsTrue(a.OldPairs?[0].Value == 0);
            Assert.IsNull(a.NewPairs);
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(observableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Clear);
            Assert.IsNull(a.OldPairs);
            Assert.IsNull(a.NewPairs);
            a.Cancel = true;
        };
        observableDictionary.Clear();
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Cleared()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanged += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.Clear);
            Assert.IsNull(a.NewPairs);
            Assert.IsNull(a.OldPairs);
        };
        observableDictionary.Clear();
        Assert.IsTrue(observableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.ValueChange);
            Assert.IsTrue(a.NewPairs?[0].Key == 0);
            Assert.IsTrue(a.NewPairs?[0].Value == 10);
            Assert.IsTrue(a.OldPairs?[0].Key == 0);
            Assert.IsTrue(a.OldPairs?[0].Value == 0);
            a.Cancel = true;
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(observableDictionary[0] == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanged()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanged += (a) =>
        {
            triggered = true;
            Assert.IsTrue(a.Action == DictionaryChangeAction.ValueChange);
            Assert.IsTrue(a.NewPairs?[0].Key == 0);
            Assert.IsTrue(a.NewPairs?[0].Value == 10);
            Assert.IsTrue(a.OldPairs?[0].Key == 0);
            Assert.IsTrue(a.OldPairs?[0].Value == 0);
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(observableDictionary[0] == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var pair = new KeyValuePair<int, int>(10, 10);
        observableDictionary.CollectionChanged += (s, e) =>
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
        Assert.IsTrue(observableDictionary.Last().Equals(pair) is true);
        Assert.IsTrue(observableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var pair = new KeyValuePair<int, int>(0, 0);
        observableDictionary.CollectionChanged += (s, e) =>
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
        Assert.IsTrue(observableDictionary.First().Key == 1);
        Assert.IsTrue(observableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.CollectionChanged += (s, e) =>
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
        Assert.IsTrue(observableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ObservableKeysAndValues()
    {
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 0);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 0);

        observableDictionary.ObservableKeysAndValues = true;
        Assert.IsTrue(observableDictionary.Keys.SequenceEqual(observableDictionary.ObservableKeys));
        Assert.IsTrue(
            observableDictionary.Values.SequenceEqual(observableDictionary.ObservableValues)
        );

        observableDictionary.ObservableKeysAndValues = false;
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 0);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 0);
    }
}
