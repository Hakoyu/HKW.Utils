using HKW.HKWUtils.Collections;

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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Add);
            Assert.AreEqual(a.NewPairs?[0].Key, 10);
            Assert.AreEqual(a.NewPairs?[0].Value, 10);
            Assert.IsNull(a.OldPairs);
            a.Cancel = true;
        };
        observableDictionary.Add(10, 10);
        Assert.AreEqual(observableDictionary.Count, 10);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Add);
            Assert.AreEqual(a.NewPairs?[0].Key, 10);
            Assert.AreEqual(a.NewPairs?[0].Value, 10);
            Assert.IsNull(a.OldPairs);
        };
        observableDictionary.Add(10, 10);
        Assert.AreEqual(observableDictionary.Count, 11);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Remove);
            Assert.AreEqual(a.OldPairs?[0].Key, 0);
            Assert.AreEqual(a.OldPairs?[0].Value, 0);
            Assert.IsNull(a.NewPairs);
            a.Cancel = true;
        };
        observableDictionary.Remove(0);
        Assert.AreEqual(observableDictionary.Count, 10);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Remove);
            Assert.AreEqual(a.OldPairs?[0].Key, 0);
            Assert.AreEqual(a.OldPairs?[0].Value, 0);
            Assert.IsNull(a.NewPairs);
        };
        observableDictionary.Remove(0);
        Assert.AreEqual(observableDictionary.Count, 9);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Clear);
            Assert.IsNull(a.OldPairs);
            Assert.IsNull(a.NewPairs);
            a.Cancel = true;
        };
        observableDictionary.Clear();
        Assert.AreEqual(observableDictionary.Count, 10);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.Clear);
            Assert.IsNull(a.NewPairs);
            Assert.IsNull(a.OldPairs);
        };
        observableDictionary.Clear();
        Assert.AreEqual(observableDictionary.Count, 0);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.ValueChange);
            Assert.AreEqual(a.NewPairs?[0].Key, 0);
            Assert.AreEqual(a.NewPairs?[0].Value, 10);
            Assert.AreEqual(a.OldPairs?[0].Key, 0);
            Assert.AreEqual(a.OldPairs?[0].Value, 0);
            a.Cancel = true;
        };
        observableDictionary[0] = 10;
        Assert.AreEqual(observableDictionary.Count, 10);
        Assert.AreEqual(observableDictionary[0], 0);
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
            Assert.AreEqual(a.Action, DictionaryChangeAction.ValueChange);
            Assert.AreEqual(a.NewPairs?[0].Key, 0);
            Assert.AreEqual(a.NewPairs?[0].Value, 10);
            Assert.AreEqual(a.OldPairs?[0].Key, 0);
            Assert.AreEqual(a.OldPairs?[0].Value, 0);
        };
        observableDictionary[0] = 10;
        Assert.AreEqual(observableDictionary.Count, 10);
        Assert.AreEqual(observableDictionary[0], 10);
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], pair);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableDictionary.Add(pair);
        Assert.AreEqual(observableDictionary.Last(), pair);
        Assert.AreEqual(observableDictionary.Count, 11);
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.AreEqual(e.OldItems?[0], pair);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableDictionary.Remove(pair);
        Assert.AreEqual(observableDictionary.First().Key, 1);
        Assert.AreEqual(observableDictionary.Count, 9);
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableDictionary.Clear();
        Assert.AreEqual(observableDictionary.Count, 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ObservableKeysAndValues()
    {
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.AreEqual(observableDictionary.ObservableKeys.Count, 0);
        Assert.AreEqual(observableDictionary.ObservableValues.Count, 0);

        observableDictionary.ObservableKeysAndValues = true;
        Assert.IsTrue(observableDictionary.Keys.SequenceEqual(observableDictionary.ObservableKeys));
        Assert.IsTrue(
            observableDictionary.Values.SequenceEqual(observableDictionary.ObservableValues)
        );

        observableDictionary.ObservableKeysAndValues = false;
        Assert.AreEqual(observableDictionary.ObservableKeys.Count, 0);
        Assert.AreEqual(observableDictionary.ObservableValues.Count, 0);
    }
}
