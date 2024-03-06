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
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.IsTrue(e.NewStartingIndex == 10);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Add(10);
        Assert.IsTrue(readOnlyObservableList[^1] == 10);
        Assert.IsTrue(readOnlyObservableList.Count == 11);
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
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == 10);
            Assert.IsTrue(e.NewStartingIndex == 5);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Insert(5, 10);
        Assert.IsTrue(readOnlyObservableList[5] == 10);
        Assert.IsTrue(readOnlyObservableList.Count == 11);
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
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            );
            Assert.IsTrue(e.OldItems?[0] is int i && i == 1);
            Assert.IsTrue(e.NewItems?[0] == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == 0);
        };
        observableList.RemoveAt(0);
        Assert.IsTrue(readOnlyObservableList[0] == 2);
        Assert.IsTrue(readOnlyObservableList.Count == 9);
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
            Assert.IsTrue(
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            );
            Assert.IsTrue(e.OldItems?[0] == null);
            Assert.IsTrue(e.NewItems?[0] == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableList.Clear();
        Assert.IsTrue(readOnlyObservableList.Count == 0);
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

        Assert.IsTrue(readOnlyObservableList.Find(x => x == 1) == 1);
        Assert.IsTrue(readOnlyObservableList.Find(1, x => x == 2) == (1, 2));
        Assert.IsTrue(readOnlyObservableList.Find(1, 3, x => x == 3) == (2, 3));

        Assert.IsTrue(readOnlyObservableList.Find(x => x == -1) == default);
        Assert.IsTrue(readOnlyObservableList.Find(1, x => x == -1) == (-1, default));
        Assert.IsTrue(readOnlyObservableList.Find(1, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void FindIndex()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.IsTrue(readOnlyObservableList.FindIndex(x => x == 1) == 0);
        Assert.IsTrue(readOnlyObservableList.FindIndex(1, x => x == 2) == 1);
        Assert.IsTrue(readOnlyObservableList.FindIndex(1, 3, x => x == 3) == 2);

        Assert.IsTrue(readOnlyObservableList.FindIndex(x => x == -1) == -1);
        Assert.IsTrue(readOnlyObservableList.FindIndex(1, x => x == -1) == -1);
        Assert.IsTrue(readOnlyObservableList.FindIndex(1, 3, x => x == -1) == -1);
    }

    [TestMethod]
    public void FindLast()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.IsTrue(readOnlyObservableList.FindLast(x => x == 1) == 1);
        Assert.IsTrue(readOnlyObservableList.FindLast(1, x => x == 2) == (1, 2));
        Assert.IsTrue(readOnlyObservableList.FindLast(4, 3, x => x == 3) == (2, 3));

        Assert.IsTrue(readOnlyObservableList.FindLast(x => x == -1) == default);
        Assert.IsTrue(readOnlyObservableList.FindLast(1, x => x == -1) == (-1, default));
        Assert.IsTrue(readOnlyObservableList.FindLast(4, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void FindLastIndex()
    {
        var readOnlyObservableList = new ReadOnlyObservableList<int>(
            new ObservableList<int>(Enumerable.Range(1, 10))
        );

        Assert.IsTrue(readOnlyObservableList.FindLastIndex(x => x == 1) == 0);
        Assert.IsTrue(readOnlyObservableList.FindLastIndex(1, x => x == 2) == 1);
        Assert.IsTrue(readOnlyObservableList.FindLastIndex(4, 3, x => x == 3) == 2);

        Assert.IsTrue(readOnlyObservableList.FindLastIndex(x => x == -1) == -1);
        Assert.IsTrue(readOnlyObservableList.FindLastIndex(1, x => x == -1) == -1);
        Assert.IsTrue(readOnlyObservableList.FindLastIndex(4, 3, x => x == -1) == -1);
    }
    #endregion
}
