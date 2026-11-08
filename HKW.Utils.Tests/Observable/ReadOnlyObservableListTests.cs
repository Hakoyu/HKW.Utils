using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ReadOnlyObservableListTests
{
    [TestMethod]
    public void ReadOnlyInterfaces()
    {
        var source = new ObservableList<int>([1, 2, 3]);
        using var view = new ReadOnlyObservableList<int>(source);
        var notifications = 0;
        view.ListChanging += (_, _) => notifications++;
        view.ListChanged += (_, _) => notifications++;
        view.CollectionChanged += (_, _) => notifications++;
        view.PropertyChanged += (_, _) => notifications++;
        IReadOnlyListTTestUtils.Test<int>(() => view, new[] { 4, 5 });
        System.Collections.IList list = view;
        Assert.IsTrue(list.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => list.Add(4));
        Assert.ThrowsExactly<NotSupportedException>(() => list.Insert(0, 4));
        Assert.ThrowsExactly<NotSupportedException>(() => list[0] = 4);
        Assert.ThrowsExactly<NotSupportedException>(() => list.Remove(1));
        Assert.ThrowsExactly<NotSupportedException>(() => list.RemoveAt(0));
        Assert.ThrowsExactly<NotSupportedException>(() => list.Clear());
        Assert.AreEqual(0, notifications);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, source.ToArray());
    }

    [TestMethod]
    public void NonGenericInterface_IsFixedSize()
    {
        using var view = new ReadOnlyObservableList<int>(new ObservableList<int>([1, 2]));
        Assert.IsTrue(((System.Collections.IList)view).IsFixedSize);
    }

    [TestMethod]
    public void Dispose_StopsForwardingWithoutDisposingSource()
    {
        var source = new ObservableList<int>([1, 2]);
        using var view = new ReadOnlyObservableList<int>(source);
        var count = 0;
        view.ListChanging += (_, _) => count++;
        view.ListChanged += (_, _) => count++;
        view.CollectionChanged += (_, _) => count++;
        view.PropertyChanged += (_, _) => count++;
        view.Dispose();
        view.Dispose();
        source.Add(3);
        Assert.AreEqual(0, count);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, source.ToArray());
    }

    #region CollectionChanged
    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggerCount = 0;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        using var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggerCount++;
            Assert.AreSame(readOnlyObservableList, s);
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
        Assert.AreEqual(1, triggerCount);
    }

    [TestMethod]
    public void CollectionChanged_Insert()
    {
        var triggerCount = 0;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        using var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        var sourceCount = 0;
        observableList.CollectionChanged += (sender, _) =>
        {
            sourceCount++;
            Assert.AreSame(observableList, sender);
        };
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggerCount++;
            Assert.AreSame(readOnlyObservableList, s);
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
        Assert.AreEqual(1, triggerCount);
        Assert.AreEqual(1, sourceCount);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggerCount = 0;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        using var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggerCount++;
            Assert.AreSame(readOnlyObservableList, s);
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
        Assert.AreEqual(1, triggerCount);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggerCount = 0;
        var list = Enumerable.Range(1, 10).ToList();
        var observableList = new ObservableList<int>(list);
        using var readOnlyObservableList = new ReadOnlyObservableList<int>(observableList);
        readOnlyObservableList.CollectionChanged += (s, e) =>
        {
            triggerCount++;
            Assert.AreSame(readOnlyObservableList, s);
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
        Assert.AreEqual(1, triggerCount);
    }
    #endregion
}
