using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
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
            Assert.IsTrue(e.Action == SetChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems == null);
            e.Cancel = true;
        };
        observableSet.Add(10);
        Assert.IsTrue(observableSet.Count == 10);
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
            Assert.IsTrue(e.Action == SetChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems == null);
        };
        observableSet.Add(10);
        Assert.IsTrue(observableSet.Count == 11);
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
            Assert.IsTrue(e.Action == SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems![0] == 0);
            Assert.IsTrue(e.OtherItems == null);
            e.Cancel = true;
        };
        observableSet.Remove(0);
        Assert.IsTrue(observableSet.Count == 10);
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
            Assert.IsTrue(e.Action == SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems![0] == 0);
            Assert.IsTrue(e.OtherItems == null);
        };
        observableSet.Remove(0);
        Assert.IsTrue(observableSet.Count == 9);
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
            Assert.IsTrue(e.Action == SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems == null);
            e.Cancel = true;
        };
        observableSet.Clear();
        Assert.IsTrue(observableSet.Count == 10);
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
            Assert.IsTrue(e.Action == SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems == null);
        };
        observableSet.Clear();
        Assert.IsTrue(observableSet.Count == 0);
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
            Assert.IsTrue(e.Action == SetChangeAction.Intersect);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems != null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
        };
        observableSet.IntersectWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
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
            Assert.IsTrue(e.Action == SetChangeAction.Except);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems != null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
        };
        observableSet.ExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
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
            Assert.IsTrue(e.Action == SetChangeAction.SymmetricExcept);
            Assert.IsTrue(e.NewItems != null);
            Assert.IsTrue(e.OldItems != null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
            // set1.Union(set2).Except(set1.Intersect(set2))
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 6);
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
            Assert.IsTrue(e.Action == SetChangeAction.Union);
            Assert.IsTrue(e.NewItems != null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
        };
        observableSet.UnionWith(ints);
        Assert.IsTrue(observableSet.Count == 11);
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
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems?[0] is 10);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Add(10);
        Assert.IsTrue(observableSet.Last() == 10);
        Assert.IsTrue(observableSet.Count == 11);
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
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.IsTrue(e.OldItems?[0] is 0);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Remove(0);
        Assert.IsTrue(observableSet.ElementAt(0) == 1);
        Assert.IsTrue(observableSet.Count == 9);
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
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Reset);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Clear();
        Assert.IsTrue(observableSet.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_IntersectWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.AreNotEqual(e.OldItems, null);
        };
        observableSet.IntersectWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_ExceptWith()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.AreNotEqual(e.OldItems, null);
        };
        observableSet.ExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_SymmetricExceptWith()
    {
        var triggered = true;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 6);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_UnionWith()
    {
        var triggered = true;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.NotifySetModifies = true;
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
            Assert.AreNotEqual(e.NewItems, null);
        };
        observableSet.UnionWith(ints);
        Assert.IsTrue(observableSet.Count == 11);
        Assert.IsTrue(triggered);
    }
}
