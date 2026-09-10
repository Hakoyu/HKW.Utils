using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using HKW.HKWUtilsTests.Collections;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public sealed class ObservableListTests : ObservableListTestsBase
{
    protected override IObservableList<string> CreateList() =>
        new ObservableList<string>(Enumerable.StringRange(1, 10));
}

[TestClass]
public sealed class ObservableListWrapperTests : ObservableListTestsBase
{
    protected override IObservableList<string> CreateList() =>
        new ObservableListWrapper<string, List<string>>(Enumerable.StringRange(1, 10).ToList());
}

public abstract class ObservableListTestsBase
{
    protected abstract IObservableList<string> CreateList();

    static IReadOnlyCollection<string> _newItems = new ReadOnlyCollection<string>(
        Enumerable.Range(100, 10).Select(i => i.ToString()).ToList()
    );

    [TestMethod]
    public void IListTTest()
    {
        IListTTestUtils.Test(CreateList, _newItems);
    }

    [TestMethod]
    public void ObservableCollectionTest()
    {
        ObservableCollectionUtils.Test(CreateList, _newItems);
    }

    #region ListChanging
    [TestMethod]
    public void ChangingOnAdd()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;
        var newItem = default(string);

        list.ListChanging += List_ListChanging;
        foreach (var item in _newItems)
        {
            newItem = item;
            list.Add(item);
            cList.Add(item);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(list, sender);
            Assert.AreEqual(ListChangeAction.Add, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(list.Count, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangingOnInsert()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;
        var newItem = default(string);
        var newIndex = -1;

        list.ListChanging += List_ListChanging;
        foreach (var (e, i) in _newItems.WithIndex())
        {
            newItem = e;
            newIndex = i;
            list.Insert(i, e);
            cList.Insert(i, e);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Add, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(newIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangingOnInsertFail()
    {
        var list = CreateList();

        list.ListChanging += List_ListChanging;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.Insert(list.Count + 1, default!);
        });
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnReplace()
    {
        var list = CreateList();
        var cList = list.ToList();
        var replaceList = list.Reverse().ToList();

        var triggerCount = 0;
        var newItem = default(string);
        var oldItem = default(string);
        var replaceIndex = -1;

        list.ListChanging += List_ListChanging;
        foreach (var (e, i) in replaceList.WithIndex())
        {
            newItem = e;
            oldItem = list[i];
            replaceIndex = i;
            list[i] = newItem;
            cList[i] = newItem;
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;

        Assert.HasCount(triggerCount, replaceList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Replace, e.Action);
            Assert.AreEqual(oldItem, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(replaceIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangingOnReplaceFail()
    {
        var list = CreateList();

        list.ListChanging += List_ListChanging;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list[list.Count + 100] = default!;
        });
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnRemove()
    {
        var list = CreateList();
        var cList = list.ToList();
        var removeList = list.ToList();

        var triggerCount = 0;
        var removeIndex = -1;
        var removeItem = default(string);

        list.ListChanging += List_ListChanging;
        removeIndex = 0;
        foreach (var item in removeList)
        {
            removeItem = item;
            Assert.AreEqual(true, [list.Remove(removeItem), cList.Remove(removeItem)]);
        }
        list.ListChanging -= List_ListChanging;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        list = CreateList();
        cList = list.ToList();
        triggerCount = 0;
        list.ListChanging += List_ListChanging;
        foreach (var (e, i) in removeList.ReverseWithIndex())
        {
            removeIndex = i;
            removeItem = e;
            Assert.AreEqual(true, [list.Remove(removeItem), cList.Remove(removeItem)]);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;

            Assert.AreEqual(ListChangeAction.Remove, e.Action);
            Assert.AreEqual(removeItem, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(removeIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangingOnRemoveFail()
    {
        var list = CreateList();
        var cList = list.ToList();

        var removeItem = default(string);

        list.ListChanging += List_ListChanging;

        foreach (var item in _newItems)
        {
            removeItem = item;
            Assert.AreEqual(false, [list.Remove(removeItem), cList.Remove(removeItem)]);
        }
        list.ListChanging -= List_ListChanging;

        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnRemoveAt()
    {
        var list = CreateList();
        var cList = list.ToList();
        var removeList = list.ToList();

        var triggerCount = 0;
        var removeIndex = -1;
        var removeItem = default(string);

        removeIndex = 0;
        list.ListChanging += List_ListChanging;
        foreach (var item in removeList)
        {
            removeItem = item;
            list.RemoveAt(0);
            cList.RemoveAt(0);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        list = CreateList();
        cList = list.ToList();
        triggerCount = 0;
        list.ListChanging += List_ListChanging;
        foreach (var (e, i) in removeList.ReverseWithIndex())
        {
            removeIndex = i;
            removeItem = e;
            list.RemoveAt(removeIndex);
            cList.RemoveAt(removeIndex);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanging -= List_ListChanging;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;

            Assert.AreEqual(ListChangeAction.Remove, e.Action);
            Assert.AreEqual(removeItem, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(removeIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangingOnRemoveAtFail()
    {
        var list = CreateList();

        list.ListChanging += List_ListChanging;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.RemoveAt(list.Count);
        });
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnClear()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;

        list.ListChanging += List_ListChanging;
        list.Clear();
        cList.Clear();
        list.ListChanging -= List_ListChanging;

        Assert.AreEqual(1, triggerCount);
        Assert.HasCount(0, list);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanging(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Clear, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(-1, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    #endregion

    #region ListChanged
    [TestMethod]
    public void ChangedOnAdd()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;
        var newItem = default(string);

        list.ListChanged += List_ListChanged;
        foreach (var item in _newItems)
        {
            newItem = item;
            cList.Add(item);
            list.Add(item);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;

            Assert.AreEqual(ListChangeAction.Add, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(cList.Count - 1, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangedOnInsert()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;
        var newItem = default(string);
        var newIndex = -1;

        list.ListChanged += List_ListChanged;
        foreach (var (e, i) in _newItems.WithIndex())
        {
            newItem = e;
            newIndex = i;
            cList.Insert(i, e);
            list.Insert(i, e);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Add, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(newIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangedOnInsertFail()
    {
        var list = CreateList();

        list.ListChanged += List_ListChanged;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.Insert(list.Count + 1, default!);
        });
        list.ListChanged -= List_ListChanged;

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnReplace()
    {
        var list = CreateList();
        var cList = list.ToList();
        var replaceList = list.Reverse().ToList();

        var triggerCount = 0;
        var newItem = default(string);
        var oldItem = default(string);
        var replaceIndex = -1;

        list.ListChanged += List_ListChanged;
        foreach (var (e, i) in replaceList.WithIndex())
        {
            newItem = e;
            oldItem = list[i];
            replaceIndex = i;
            cList[i] = newItem;
            list[i] = newItem;
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;

        Assert.HasCount(triggerCount, replaceList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Replace, e.Action);
            Assert.AreEqual(oldItem, e.OldItem);
            Assert.AreEqual(newItem, e.NewItem);
            Assert.AreEqual(replaceIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangedOnReplaceFail()
    {
        var list = CreateList();

        list.ListChanged += List_ListChanged;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list[list.Count + 100] = default!;
        });
        list.ListChanged -= List_ListChanged;

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnRemove()
    {
        var list = CreateList();
        var cList = list.ToList();
        var removeList = list.ToList();

        var triggerCount = 0;
        var removeIndex = -1;
        var removeItem = default(string);

        removeIndex = 0;
        list.ListChanged += List_ListChanged;
        foreach (var item in removeList)
        {
            removeItem = item;
            Assert.AreEqual(true, [cList.Remove(removeItem), list.Remove(removeItem)]);
        }
        list.ListChanged -= List_ListChanged;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        list = CreateList();
        cList = list.ToList();
        triggerCount = 0;
        list.ListChanged += List_ListChanged;
        foreach (var (e, i) in removeList.ReverseWithIndex())
        {
            removeIndex = i;
            removeItem = e;
            Assert.AreEqual(true, [cList.Remove(removeItem), list.Remove(removeItem)]);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;

            Assert.AreEqual(ListChangeAction.Remove, e.Action);
            Assert.AreEqual(removeItem, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(removeIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangedOnRemoveFail()
    {
        var list = CreateList();
        var cList = list.ToList();

        var removeItem = default(string);

        list.ListChanged += List_ListChanged;
        foreach (var item in _newItems)
        {
            removeItem = item;
            Assert.AreEqual(false, [list.Remove(removeItem), cList.Remove(removeItem)]);
        }
        list.ListChanged -= List_ListChanged;

        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnRemoveAt()
    {
        var list = CreateList();
        var cList = list.ToList();
        var removeList = list.ToList();

        var triggerCount = 0;
        var removeIndex = -1;
        var removeItem = default(string);

        removeIndex = 0;
        list.ListChanged += List_ListChanged;
        foreach (var item in removeList)
        {
            removeItem = item;
            cList.RemoveAt(0);
            list.RemoveAt(0);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        list = CreateList();
        cList = list.ToList();
        triggerCount = 0;
        list.ListChanged += List_ListChanged;
        foreach (var (e, i) in removeList.ReverseWithIndex())
        {
            removeIndex = i;
            removeItem = e;
            cList.RemoveAt(removeIndex);
            list.RemoveAt(removeIndex);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
        list.ListChanged -= List_ListChanged;
        Assert.HasCount(triggerCount, removeList);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;

            Assert.AreEqual(ListChangeAction.Remove, e.Action);
            Assert.AreEqual(removeItem, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(removeIndex, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    [TestMethod]
    public void ChangedOnRemoveAtFail()
    {
        var list = CreateList();

        list.ListChanged += List_ListChanged;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.RemoveAt(list.Count);
        });
        list.ListChanged -= List_ListChanged;

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnClear()
    {
        var list = CreateList();
        var cList = list.ToList();

        var triggerCount = 0;

        list.ListChanged += List_ListChanged;
        cList.Clear();
        list.Clear();
        list.ListChanged -= List_ListChanged;

        Assert.AreEqual(1, triggerCount);
        Assert.HasCount(0, list);
        Assert.IsTrue(list.SequenceEqual(cList));

        void List_ListChanged(IObservableList<string> sender, NotifyListChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(ListChangeAction.Clear, e.Action);
            Assert.AreEqual(default, e.OldItem);
            Assert.AreEqual(default, e.NewItem);
            Assert.AreEqual(-1, e.Index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    #endregion
}
