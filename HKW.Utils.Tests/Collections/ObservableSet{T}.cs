using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKWTests.CollectionsTests;

[TestClass]
public class ObservableSetT
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.SetChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Add);
            Assert.IsNull(e.Items);
            Assert.AreEqual(e.Item, 10);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Add);
            Assert.IsNull(e.Items);
            Assert.AreEqual(e.Item, 10);
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
        observableSet.SetChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Remove);
            Assert.IsNull(e.Items);
            Assert.AreEqual(e.Item, 0);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Remove);
            Assert.IsNull(e.Items);
            Assert.AreEqual(e.Item, 0);
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
        observableSet.SetChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Clear);
            Assert.IsNull(e.Items);
            Assert.IsTrue(e.Item == default);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Clear);
            Assert.IsNull(e.Items);
            Assert.IsTrue(e.Item == default);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Intersect);
            Assert.AreEqual(e.Items, ints);
            Assert.IsTrue(e.Item == default);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Except);
            Assert.AreEqual(e.Items, ints);
            Assert.IsTrue(e.Item == default);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.SymmetricExcept);
            Assert.AreEqual(e.Items, ints);
            Assert.IsTrue(e.Item == default);
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
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, SetChangeMode.Union);
            Assert.AreEqual(e.Items, ints);
            Assert.IsTrue(e.Item == default);
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
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
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableSet.Clear();
        Assert.AreEqual(observableSet.Count, 0);
        Assert.IsTrue(triggered);
    }
}
