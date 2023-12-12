using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableListXTests
{
    #region Add
    [TestMethod]
    public void Adding()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
        };
        observableList.Add(10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Adding_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
            e.Cancel = true;
        };
        observableList.Add(10);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Added()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
        };
        observableList.Add(10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    #region AddRange
    [TestMethod]
    public void AddingRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(list));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
        };
        observableList.AddRange(list);
        Assert.IsTrue(observableList.Count == 20);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddingRange_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(list));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
            e.Cancel = true;
        };
        observableList.AddRange(list);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void AddedRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(list));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 10);
        };
        observableList.AddRange(list);
        Assert.IsTrue(observableList.Count == 20);
        Assert.IsTrue(triggered);
    }
    #endregion
    #endregion

    #region Insert
    [TestMethod]
    public void Inserting()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
        };
        observableList.Insert(5, 10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Inserting_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
            e.Cancel = true;
        };
        observableList.Insert(5, 10);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Inserted()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
        };
        observableList.Insert(5, 10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    #region InsertRange
    [TestMethod]
    public void InsertingRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var insertItems = Enumerable.Range(10, 10).ToList();
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(insertItems));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
        };
        observableList.InsertRange(5, insertItems);
        Assert.IsTrue(observableList.Count == 20);
        Assert.IsTrue(triggered);
        var temp = list.ToList();
        temp.InsertRange(5, insertItems);
        Assert.IsTrue(observableList.SequenceEqual(temp));
    }

    [TestMethod]
    public void InsertingRange_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var insertItems = Enumerable.Range(10, 10).ToList();
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(insertItems));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
            e.Cancel = true;
        };
        observableList.InsertRange(5, insertItems);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void InsertedRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var insertItems = Enumerable.Range(10, 10).ToList();
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Add);
            Assert.IsTrue(e.NewItems!.Count == 10);
            Assert.IsTrue(e.NewItems!.SequenceEqual(insertItems));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == 5);
        };
        observableList.InsertRange(5, insertItems);
        Assert.IsTrue(observableList.Count == 20);
        Assert.IsTrue(triggered);
        var temp = list.ToList();
        temp.InsertRange(5, insertItems);
        Assert.IsTrue(observableList.SequenceEqual(temp));
    }
    #endregion
    #endregion

    #region Remove
    [TestMethod]
    public void Removing()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
        };
        observableList.RemoveAt(0);
        Assert.IsTrue(observableList.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removing_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
            e.Cancel = true;
        };
        observableList.RemoveAt(0);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Removed()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
        };
        observableList.RemoveAt(0);
        Assert.IsTrue(observableList.Count == 9);
        Assert.IsTrue(triggered);
    }

    #region RemoveRange
    [TestMethod]
    public void RemovingRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var items = Enumerable.Range(6, 5).ToList();
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems!.Count == 5);
            Assert.IsTrue(e.OldItems!.SequenceEqual(items));
            Assert.IsTrue(e.Index == 5);
        };
        observableList.RemoveRange(5, 5);
        Assert.IsTrue(observableList.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void RemovingRange_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var items = Enumerable.Range(6, 5).ToList();
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems!.Count == 5);
            Assert.IsTrue(e.OldItems!.SequenceEqual(items));
            Assert.IsTrue(e.Index == 5);
            e.Cancel = true;
        };
        observableList.RemoveRange(5, 5);
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void RemovedRange()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        var items = Enumerable.Range(6, 5).ToList();
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems!.Count == 5);
            Assert.IsTrue(e.OldItems!.SequenceEqual(items));
            Assert.IsTrue(e.Index == 5);
        };
        observableList.RemoveRange(5, 5);
        Assert.IsTrue(observableList.Count == 5);
        Assert.IsTrue(triggered);
    }
    #endregion
    #endregion

    #region Clear
    [TestMethod]
    public void Clearing()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == -1);
        };
        observableList.Clear();
        Assert.IsTrue(observableList.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Clearing_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == -1);
            e.Cancel = true;
        };
        observableList.Clear();
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Cleared()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Clear);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.Index == -1);
        };
        observableList.Clear();
        Assert.IsTrue(observableList.Count == 0);
        Assert.IsTrue(triggered);
    }
    #endregion

    #region Replace
    [TestMethod]
    public void Replacing()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Replace);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
        };
        observableList[0] = 10;
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(observableList[0] == 10);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Replacing_Cancel()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanging += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Replace);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
            e.Cancel = true;
        };
        observableList[0] = 10;
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(observableList[0] == 1);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void Replaced()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.ListChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == ListChangeAction.Replace);
            Assert.IsTrue(e.NewItems![0] == 10);
            Assert.IsTrue(e.OldItems![0] == 1);
            Assert.IsTrue(e.Index == 0);
        };
        observableList[0] = 10;
        Assert.IsTrue(observableList.Count == 10);
        Assert.IsTrue(observableList[0] == 10);
        Assert.IsTrue(triggered);
    }
    #endregion

    #region CollectionChanged
    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.IsTrue(e.NewStartingIndex == 10);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Add(10);
        Assert.IsTrue(observableList[^1] == 10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Insert()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.IsTrue(e.NewStartingIndex == 5);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Insert(5, 10);
        Assert.IsTrue(observableList[5] == 10);
        Assert.IsTrue(observableList.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.IsTrue(e.OldItems?[0] is int i && i == 1);
            Assert.IsTrue(e.NewItems?[0] == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == 0);
        };
        observableList.RemoveAt(0);
        Assert.IsTrue(observableList[0] == 2);
        Assert.IsTrue(observableList.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableListX<int>(list);
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Clear();
        Assert.IsTrue(observableList.Count == 0);
        Assert.IsTrue(triggered);
    }
    #endregion
}
