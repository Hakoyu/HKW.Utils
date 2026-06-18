using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ReadOnlyObservableListTests
{
    #region CollectionChanged
    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Add,
                e.Action
            );
            Assert.IsNull(e.OldItems?[0]);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.AreEqual(10, e.NewStartingIndex);
            Assert.AreEqual(-1, e.OldStartingIndex);
        };
        observableList.Add(10);
        Assert.AreEqual(10, readOnlyObservableList[^1]);
        Assert.AreEqual(11, readOnlyObservableList.Count);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Insert()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        observableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Add,
                e.Action
            );
            Assert.IsNull(e.OldItems?[0]);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.AreEqual(5, e.NewStartingIndex);
            Assert.AreEqual(-1, e.OldStartingIndex);
        };
        observableList.Insert(5, 10);
        Assert.AreEqual(10, readOnlyObservableList[5]);
        Assert.AreEqual(11, readOnlyObservableList.Count);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Remove,
                e.Action
            );
            Assert.IsTrue(e.OldItems?[0] is int i && i == 1);
            Assert.IsNull(e.NewItems?[0]);
            Assert.AreEqual(-1, e.NewStartingIndex);
            Assert.AreEqual(0, e.OldStartingIndex);
        };
        observableList.RemoveAt(0);
        Assert.AreEqual(2, readOnlyObservableList[0]);
        Assert.AreEqual(9, readOnlyObservableList.Count);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.AreEqual(
                System.Collections.Specialized.NotifyCollectionChangedAction.Reset,
                e.Action
            );
            Assert.IsNull(e.OldItems?[0]);
            Assert.IsNull(e.NewItems?[0]);
            Assert.AreEqual(-1, e.NewStartingIndex);
            Assert.AreEqual(-1, e.OldStartingIndex);
        };
        observableList.Clear();
        Assert.AreEqual(0, readOnlyObservableList.Count);
        Assert.IsTrue(triggered);
    }
    #endregion
    #region IListFind
    [TestMethod]
    public void Find()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.AreEqual(1, readOnlyObservableList.Find(x => x == 1));
        Assert.IsTrue(readOnlyObservableList.FindPair(1, x => x == 2) == (1, 2));
        Assert.IsTrue(readOnlyObservableList.FindPair(1, 3, x => x == 3) == (2, 3));

        Assert.AreEqual(default, readOnlyObservableList.Find(x => x == -1));
        Assert.IsTrue(readOnlyObservableList.FindPair(1, x => x == -1) == (-1, default));
        Assert.IsTrue(readOnlyObservableList.FindPair(1, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void FindIndex()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.AreEqual(0, readOnlyObservableList.FindIndex(x => x == 1));
        Assert.AreEqual(1, readOnlyObservableList.FindIndex(1, x => x == 2));
        Assert.AreEqual(2, readOnlyObservableList.FindIndex(1, 3, x => x == 3));

        Assert.AreEqual(-1, readOnlyObservableList.FindIndex(x => x == -1));
        Assert.AreEqual(-1, readOnlyObservableList.FindIndex(1, x => x == -1));
        Assert.AreEqual(-1, readOnlyObservableList.FindIndex(1, 3, x => x == -1));
    }

    [TestMethod]
    public void FindLast()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.AreEqual(1, readOnlyObservableList.FindLast(x => x == 1));
        Assert.IsTrue(readOnlyObservableList.FindLastPair(1, x => x == 2) == (1, 2));
        Assert.IsTrue(readOnlyObservableList.FindLastPair(4, 3, x => x == 3) == (2, 3));

        Assert.AreEqual(default, readOnlyObservableList.FindLast(x => x == -1));
        Assert.IsTrue(readOnlyObservableList.FindLastPair(1, x => x == -1) == (-1, default));
        Assert.IsTrue(readOnlyObservableList.FindLastPair(4, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void FindLastIndex()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.AreEqual(0, readOnlyObservableList.FindLastIndex(x => x == 1));
        Assert.AreEqual(1, readOnlyObservableList.FindLastIndex(1, x => x == 2));
        Assert.AreEqual(2, readOnlyObservableList.FindLastIndex(4, 3, x => x == 3));

        Assert.AreEqual(-1, readOnlyObservableList.FindLastIndex(x => x == -1));
        Assert.AreEqual(-1, readOnlyObservableList.FindLastIndex(1, x => x == -1));
        Assert.AreEqual(-1, readOnlyObservableList.FindLastIndex(4, 3, x => x == -1));
    }
    #endregion
}
