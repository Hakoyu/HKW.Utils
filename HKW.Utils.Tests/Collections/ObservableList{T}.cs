﻿using System;
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Add);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, 10);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Add);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, 10);
        };
        observableList.Add(10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Inserting()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Add);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, 5);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Add);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, 5);
        };
        observableList.Insert(5, 10);
        Assert.AreEqual(observableList.Count, 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Remove);
            Assert.AreEqual(e.NewItem, default);
            Assert.AreEqual(e.OldItem, 1);
            Assert.AreEqual(e.Index, 0);
            e.Cancel = true;
        };
        observableList.RemoveAt(0);
        Assert.AreEqual(observableList.Count, 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Remove);
            Assert.AreEqual(e.NewItem, default);
            Assert.AreEqual(e.OldItem, 1);
            Assert.AreEqual(e.Index, 0);
        };
        observableList.RemoveAt(0);
        Assert.AreEqual(observableList.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Clear);
            Assert.AreEqual(e.NewItem, default);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, -1);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.Clear);
            Assert.AreEqual(e.NewItem, default);
            Assert.AreEqual(e.OldItem, default);
            Assert.AreEqual(e.Index, -1);
        };
        observableList.Clear();
        Assert.AreEqual(observableList.Count, 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanging()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.ValueChange);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, 1);
            Assert.AreEqual(e.Index, 0);
            e.Cancel = true;
        };
        observableList[0] = 10;
        Assert.AreEqual(observableList.Count, 10);
        Assert.AreEqual(observableList[0], 1);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void ValueChanged()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(e.ChangeMode, ListChangeMode.ValueChange);
            Assert.AreEqual(e.NewItem, 10);
            Assert.AreEqual(e.OldItem, 1);
            Assert.AreEqual(e.Index, 0);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], default);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.AreEqual(e.OldItems?[0], default);
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
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.AreEqual(e.OldItems?[0], 1);
            Assert.AreEqual(e.NewItems?[0], default);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, 0);
        };
        observableList.RemoveAt(0);
        Assert.AreEqual(observableList[0], 2);
        Assert.AreEqual(observableList.Count, 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var observableList = new ObservableList<int>(Enumerable.Range(1, 10));
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                e.Action,
                System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.AreEqual(e.OldItems?[0], default);
            Assert.AreEqual(e.NewItems?[0], default);
            Assert.AreEqual(e.NewStartingIndex, -1);
            Assert.AreEqual(e.OldStartingIndex, -1);
        };
        observableList.Clear();
        Assert.AreEqual(observableList.Count, 0);
        Assert.IsTrue(triggered);
    }
}
