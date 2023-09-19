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
        var newPair = KeyValuePair.Create(10, 10);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Equals(newPair));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(observableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Adding_Cancel()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var newPair = KeyValuePair.Create(10, 10);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Equals(newPair));
            Assert.IsTrue(e.OldItems == null);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0].Key == 10);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(observableDictionary.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddingRange()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(observableDictionary.Count == 15);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddingRange_Cancel()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
            e.Cancel = true;
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddedRange()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        observableDictionary.DictionaryChanged += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewItems?.Count == 5);
            Assert.IsTrue(e.NewItems.ItemsEqual(addPairs));
            Assert.IsTrue(e.OldItems == null);
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
        var newPair = KeyValuePair.Create(0, 0);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
            Assert.IsTrue(e.NewItems == null);
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(observableDictionary.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing_Cancel()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        var newPair = KeyValuePair.Create(0, 0);
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
            Assert.IsTrue(e.NewItems == null);
            e.Cancel = true;
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
        var newPair = KeyValuePair.Create(0, 0);
        observableDictionary.DictionaryChanged += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?[0].Equals(newPair));
        };
        observableDictionary.Remove(newPair);
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
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems == null);
        };
        observableDictionary.Clear();
        Assert.IsTrue(observableDictionary.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing_Cancel()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems == null);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
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
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.ValueChange);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
        };
        observableDictionary[0] = 10;
        Assert.IsTrue(observableDictionary.Count == 10);
        Assert.IsTrue(observableDictionary[0] == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging_Cancel()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.ValueChange);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == DictionaryChangeAction.ValueChange);
            Assert.IsTrue(e.NewItems?[0].Key == 0);
            Assert.IsTrue(e.NewItems?[0].Value == 10);
            Assert.IsTrue(e.OldItems?[0].Key == 0);
            Assert.IsTrue(e.OldItems?[0].Value == 0);
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
        Assert.IsTrue(observableDictionary.Keys.ItemsEqual(observableDictionary.ObservableKeys));
        Assert.IsTrue(
            observableDictionary.Values.ItemsEqual(observableDictionary.ObservableValues)
        );
        observableDictionary.ObservableKeysAndValues = false;
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 0);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 0);
    }

    [TestMethod]
    public void ObservableKeysAndValues_Add()
    {
        var triggeredKeys = false;
        var triggeredValues = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        )
        {
            ObservableKeysAndValues = true
        };
        observableDictionary.ObservableKeys.ListChanged += (e) =>
        {
            triggeredKeys = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0] == 10);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.ObservableValues.ListChanged += (e) =>
        {
            triggeredValues = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems?[0] == 10);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Add(10, 10);
        Assert.IsTrue(observableDictionary.Count == 11);
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 11);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 11);
        Assert.IsTrue(triggeredKeys);
        Assert.IsTrue(triggeredValues);
    }

    [TestMethod]
    public void ObservableKeysAndValues_AddRange()
    {
        var triggeredKeys = false;
        var triggeredValues = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        )
        {
            ObservableKeysAndValues = true
        };
        var addPairs = Enumerable.Range(10, 5).ToDictionary(i => i, i => i);
        observableDictionary.ObservableKeys.ListChanged += (e) =>
        {
            triggeredKeys = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems?.ItemsEqual(addPairs.Keys));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.ObservableValues.ListChanged += (e) =>
        {
            triggeredValues = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems?.ItemsEqual(addPairs.Values));
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.AddRange(addPairs);
        Assert.IsTrue(observableDictionary.Count == 15);
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 15);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 15);
        Assert.IsTrue(triggeredKeys);
        Assert.IsTrue(triggeredValues);
    }

    [TestMethod]
    public void ObservableKeysAndValues_Remove()
    {
        var triggeredKeys = false;
        var triggeredValues = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        )
        {
            ObservableKeysAndValues = true
        };
        observableDictionary.ObservableKeys.ListChanged += (e) =>
        {
            triggeredKeys = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?[0] == 0);
        };
        observableDictionary.ObservableValues.ListChanged += (e) =>
        {
            triggeredValues = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?[0] == 0);
        };
        observableDictionary.Remove(0);
        Assert.IsTrue(observableDictionary.Count == 9);
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 9);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 9);
        Assert.IsTrue(triggeredKeys);
        Assert.IsTrue(triggeredValues);
    }

    [TestMethod]
    public void ObservableKeysAndValues_Clear()
    {
        var triggeredKeys = false;
        var triggeredValues = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        )
        {
            ObservableKeysAndValues = true
        };
        observableDictionary.ObservableKeys.ListChanged += (e) =>
        {
            triggeredKeys = true;
            Assert.IsTrue(e.Action == ListChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.ObservableValues.ListChanged += (e) =>
        {
            triggeredValues = true;
            Assert.IsTrue(e.Action == ListChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
        };
        observableDictionary.Clear();
        Assert.IsTrue(observableDictionary.Count == 0);
        Assert.IsTrue(observableDictionary.ObservableKeys.Count == 0);
        Assert.IsTrue(observableDictionary.ObservableValues.Count == 0);
        Assert.IsTrue(triggeredKeys);
        Assert.IsTrue(triggeredValues);
    }
}
