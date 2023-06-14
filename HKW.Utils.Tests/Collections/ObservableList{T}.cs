using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKWTests.CollectionsTests;

[TestClass]
public class ObservableListT
{
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Add);
            Assert.AreEqual(e.Item, 10);
            Assert.AreEqual(e.Index, 10);
            Assert.IsTrue(e.NewItem == default);
            e.Cancel = true;
        };
        observableList.Add(10);
        Assert.AreEqual(observableList.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Added()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Add);
            Assert.AreEqual(e.Item, 10);
            Assert.AreEqual(e.Index, 10);
            Assert.IsTrue(e.OldItem == default);
        };
        observableList.Add(10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Inserting()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Add);
            Assert.AreEqual(e.Item, 10);
            Assert.AreEqual(e.Index, 5);
            Assert.IsTrue(e.NewItem == default);
            e.Cancel = true;
        };
        observableList.Insert(5, 10);
        Assert.AreEqual(observableList.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Inserted()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Add);
            Assert.AreEqual(e.Item, 10);
            Assert.AreEqual(e.Index, 5);
            Assert.IsTrue(e.OldItem == default);
        };
        observableList.Insert(5, 10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Remove);
            Assert.AreEqual(e.Item, 0);
            Assert.AreEqual(e.Index, 9);
            Assert.IsTrue(e.NewItem == default);
            e.Cancel = true;
        };
        observableList.Remove(0);
        Assert.AreEqual(observableList.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Remove);
            Assert.AreEqual(e.Item, 0);
            Assert.AreEqual(e.Index, 0);
            Assert.IsTrue(e.OldItem == default);
        };
        observableList.Remove(0);
        Assert.AreEqual(observableList.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Clear);
            Assert.IsTrue(e.Item == default);
            Assert.AreEqual(e.Index, -1);
            Assert.IsTrue(e.NewItem == default);
            e.Cancel = true;
        };
        observableList.Clear();
        Assert.AreEqual(observableList.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Cleared()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.Clear);
            Assert.IsTrue(e.Item == default);
            Assert.AreEqual(e.Index, -1);
            Assert.IsTrue(e.OldItem == default);
        };
        observableList.Clear();
        Assert.AreEqual(observableList.Count, 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.ValueChange);
            Assert.AreEqual(e.Item, 0);
            Assert.AreEqual(e.Index, 0);
            Assert.AreEqual(e.NewItem, 10);
            e.Cancel = true;
        };
        observableList[0] = 10;
        Assert.AreEqual(observableList.Count, 10);
        Assert.AreEqual(observableList[0], 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanged()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.Action, NotifyListChangeAction.ValueChange);
            Assert.AreEqual(e.Item, 10);
            Assert.AreEqual(e.Index, 0);
            Assert.AreEqual(e.OldItem, 0);
        };
        observableList[0] = 10;
        Assert.AreEqual(observableList.Count, 10);
        Assert.AreEqual(observableList[0], 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], 10);
            Assert.AreEqual(e.NewStartingIndex, 10);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableList.Add(10);
        Assert.AreEqual(observableList[^1], 10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Insert()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], null);
            Assert.AreEqual(e.NewItems?[0], 10);
            Assert.AreEqual(e.NewStartingIndex, 5);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableList.Insert(5, 10);
        Assert.AreEqual(observableList[5], 10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.AreEqual(e.OldItems?[0], 0);
            Assert.AreEqual(e.NewItems?[0], null);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, 0);
        };
        observableList.Remove(0);
        Assert.AreEqual(observableList[0], 1);
        Assert.AreEqual(observableList.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(0, 10));
        observableList.CollectionChanged += (s, e) =>
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
        observableList.Clear();
        Assert.AreEqual(observableList.Count, 0);
        Assert.IsTrue(triggered);
    }
}
