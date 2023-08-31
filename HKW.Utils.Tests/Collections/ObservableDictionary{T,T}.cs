using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKWTests.CollectionsTests;

[TestClass]
public class ObservableDictionaryTT
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var observableDictionary = new ObservableDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        observableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Add);
            Assert.AreEqual(e.NewEntry?.Key, 10);
            Assert.AreEqual(e.NewEntry?.Value, 10);
            Assert.IsNull(e.OldEntry);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Add);
            Assert.AreEqual(e.NewEntry?.Key, 10);
            Assert.AreEqual(e.NewEntry?.Value, 10);
            Assert.IsNull(e.OldEntry);
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
        observableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Remove);
            Assert.AreEqual(e.OldEntry?.Key, 0);
            Assert.AreEqual(e.OldEntry?.Value, 0);
            Assert.IsNull(e.NewEntry);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Remove);
            Assert.AreEqual(e.OldEntry?.Key, 0);
            Assert.AreEqual(e.OldEntry?.Value, 0);
            Assert.IsNull(e.NewEntry);
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
        observableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Clear);
            Assert.IsNull(e.OldEntry);
            Assert.IsNull(e.NewEntry);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.Clear);
            Assert.IsNull(e.NewEntry);
            Assert.IsNull(e.OldEntry);
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
        observableDictionary.DictionaryChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.ValueChange);
            Assert.AreEqual(e.NewEntry?.Key, 0);
            Assert.AreEqual(e.NewEntry?.Value, 10);
            Assert.AreEqual(e.OldEntry?.Key, 0);
            Assert.AreEqual(e.OldEntry?.Value, 0);
            e.Cancel = true;
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
        observableDictionary.DictionaryChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, DictionaryChangeMode.ValueChange);
            Assert.AreEqual(e.NewEntry?.Key, 0);
            Assert.AreEqual(e.NewEntry?.Value, 10);
            Assert.AreEqual(e.OldEntry?.Key, 0);
            Assert.AreEqual(e.OldEntry?.Value, 0);
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
        var entry = new KeyValuePair<int, int>(10, 10);
        observableDictionary.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], entry);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableDictionary.Add(entry);
        Assert.AreEqual(observableDictionary.Last(), entry);
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
        var entry = new KeyValuePair<int, int>(0, 0);
        observableDictionary.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.AreEqual(e.OldItems?[0], entry);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableDictionary.Remove(entry);
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
}
