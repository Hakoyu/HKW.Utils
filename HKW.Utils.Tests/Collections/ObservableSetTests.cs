using HKW.HKWUtils.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HKWTests.Collections;

[TestClass]
public class ObservableSetTests
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanging += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Add);
            Assert.IsNull(e.OtherItems);
            Assert.AreEqual(e.NewItem, 10);
            e.Cancel = true;
        };
        observableSet.Add(10);
        Assert.AreEqual(observableSet.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Added()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Add);
            Assert.IsNull(e.OtherItems);
            Assert.AreEqual(e.NewItem, 10);
        };
        observableSet.Add(10);
        Assert.AreEqual(observableSet.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanging += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Remove);
            Assert.IsNull(e.OtherItems);
            Assert.AreEqual(e.NewItem, 0);
            e.Cancel = true;
        };
        observableSet.Remove(0);
        Assert.AreEqual(observableSet.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Remove);
            Assert.IsNull(e.OtherItems);
            Assert.AreEqual(e.NewItem, 0);
        };
        observableSet.Remove(0);
        Assert.AreEqual(observableSet.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanging += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Clear);
            Assert.IsNull(e.OtherItems);
            Assert.IsTrue(e.NewItem == default);
            e.Cancel = true;
        };
        observableSet.Clear();
        Assert.AreEqual(observableSet.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Cleared()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Clear);
            Assert.IsNull(e.OtherItems);
            Assert.IsTrue(e.NewItem == default);
        };
        observableSet.Clear();
        Assert.AreEqual(observableSet.Count, 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void IntersectWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Intersect);
            Assert.AreEqual(e.OtherItems, ints);
            Assert.IsTrue(e.NewItem == default);
        };
        observableSet.IntersectWith(ints);
        Assert.AreEqual(observableSet.Count, 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ExceptWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Except);
            Assert.AreEqual(e.OtherItems, ints);
            Assert.IsTrue(e.NewItem == default);
        };
        observableSet.ExceptWith(ints);
        Assert.AreEqual(observableSet.Count, 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void SymmetricExceptWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.SymmetricExcept);
            Assert.AreEqual(e.OtherItems, ints);
            Assert.IsTrue(e.NewItem == default);
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.AreEqual(observableSet.Count, 6);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void UnionWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, SetChangeAction.Union);
            Assert.AreEqual(e.OtherItems, ints);
            Assert.IsTrue(e.NewItem == default);
        };
        observableSet.UnionWith(ints);
        Assert.AreEqual(observableSet.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Add);
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], 10);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableSet.Add(10);
        Assert.AreEqual(observableSet.Last(), 10);
        Assert.AreEqual(observableSet.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Remove);
            Assert.AreEqual(e.OldItems?[0], 0);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableSet.Remove(0);
        Assert.AreEqual(observableSet.ElementAt(0), 1);
        Assert.AreEqual(observableSet.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Reset);
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableSet.Clear();
        Assert.AreEqual(observableSet.Count, 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_IntersectWith()
    {
        var triggeredCount = 0;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggeredCount++;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Remove);
            Assert.AreNotEqual(e.OldItems, null);
        };
        observableSet.IntersectWith(ints);
        Assert.AreEqual(observableSet.Count, 5);
        Assert.AreEqual(triggeredCount, 5);
    }

    [TestMethod]
    public void CollectionChanged_ExceptWith()
    {
        var triggeredCount = 0;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggeredCount++;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Remove);
            Assert.AreNotEqual(e.OldItems, null);
        };
        observableSet.ExceptWith(ints);
        Assert.AreEqual(observableSet.Count, 5);
        Assert.AreEqual(triggeredCount, 5);
    }

    [TestMethod]
    public void CollectionChanged_SymmetricExceptWith()
    {
        var triggeredCount = 0;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggeredCount++;
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.AreEqual(observableSet.Count, 6);
        Assert.AreEqual(triggeredCount, 6);
    }

    [TestMethod]
    public void CollectionChanged_UnionWith()
    {
        var triggeredCount = 0;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggeredCount++;
            Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Add);
            Assert.AreNotEqual(e.NewItems, null);
        };
        observableSet.UnionWith(ints);
        Assert.AreEqual(observableSet.Count, 11);
        Assert.AreEqual(triggeredCount, 1);
    }
}
