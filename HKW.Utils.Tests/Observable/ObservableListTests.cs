using System.Collections.Specialized;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using HKW.HKWUtils.Tests.Collections;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableListTests
{
    [TestMethod]
    public void Test()
    {
        Test(
            new ObservableList<int>(),
            Enumerable.Range(1, 10).ToList(),
            () => Random.Shared.Next(100, 1000)
        );
    }

    public static void Test<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        Func<T> createNewItem
    )
    {
        IListTTestUtils.Test(list, comparisonList, createNewItem);
        ObservableCollectionTests.Test(list, comparisonList, createNewItem);

        ListChangingOnAdd(list, comparisonList, createNewItem());

        ListChangingOnInsert(list, comparisonList, 0, createNewItem());
        ListChangingOnInsert(list, comparisonList, comparisonList.Count / 2, createNewItem());
        ListChangingOnInsert(list, comparisonList, comparisonList.Count - 1, createNewItem());
        ListChangingOnInsertFalse(list, comparisonList, -1, createNewItem());
        ListChangingOnInsertFalse(list, comparisonList, comparisonList.Count + 1, createNewItem());

        ListChangingOnRemove(list, comparisonList);
        ListChangingOnRemoveFalse(list, comparisonList, createNewItem());

        ListChangingOnRemoveAt(list, comparisonList, 0);
        ListChangingOnRemoveAt(list, comparisonList, comparisonList.Count / 2);
        ListChangingOnRemoveAt(list, comparisonList, comparisonList.Count - 1);
        ListChangingOnRemoveAtFalse(list, comparisonList, -1);
        ListChangingOnRemoveAtFalse(list, comparisonList, comparisonList.Count);

        ListChangingOnClear(list, comparisonList);

        ListChangingOnReplace(list, comparisonList, 0, createNewItem());
        ListChangingOnReplace(list, comparisonList, comparisonList.Count / 2, createNewItem());
        ListChangingOnReplace(list, comparisonList, comparisonList.Count - 1, createNewItem());
        ListChangingOnReplaceFalse(list, comparisonList, -1, createNewItem());
        ListChangingOnReplaceFalse(list, comparisonList, comparisonList.Count, createNewItem());

        ListChangedOnAdd(list, comparisonList, createNewItem());

        ListChangedOnInsert(list, comparisonList, 0, createNewItem());
        ListChangedOnInsert(list, comparisonList, comparisonList.Count / 2, createNewItem());
        ListChangedOnInsert(list, comparisonList, comparisonList.Count - 1, createNewItem());
        ListChangedOnInsertFalse(list, comparisonList, -1, createNewItem());
        ListChangedOnInsertFalse(list, comparisonList, comparisonList.Count + 1, createNewItem());

        ListChangedOnRemove(list, comparisonList);
        ListChangedOnRemoveFalse(list, comparisonList, createNewItem());

        ListChangedOnRemoveAt(list, comparisonList, 0);
        ListChangedOnRemoveAt(list, comparisonList, comparisonList.Count / 2);
        ListChangedOnRemoveAt(list, comparisonList, comparisonList.Count - 1);
        ListChangedOnRemoveAtFalse(list, comparisonList, -1);
        ListChangedOnRemoveAtFalse(list, comparisonList, comparisonList.Count);

        ListChangedOnClear(list, comparisonList);

        ListChangedOnReplace(list, comparisonList, 0, createNewItem());
        ListChangedOnReplace(list, comparisonList, comparisonList.Count / 2, createNewItem());
        ListChangedOnReplace(list, comparisonList, comparisonList.Count - 1, createNewItem());
        ListChangedOnReplaceFalse(list, comparisonList, -1, createNewItem());
        ListChangedOnReplaceFalse(list, comparisonList, comparisonList.Count, createNewItem());
    }

    #region ListChanging
    public static void ListChangingOnAdd<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanging += List_ListChanging;
        list.Add(newItem);
        cList.Add(newItem);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == list.Count);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnInsert<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanging += List_ListChanging;
        list.Insert(index, newItem);
        cList.Insert(index, newItem);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnInsertFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanging += List_ListChanging;
        try
        {
            list.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        try
        {
            cList.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangingOnRemove<T>(IObservableList<T> list, IList<T> comparisonList)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        var removeIndex = cList.RandomIndex();
        var removeItem = cList[removeIndex];
        list.ListChanging += List_ListChanging;
        Assert.IsTrue(list.Remove(removeItem));
        Assert.IsTrue(cList.Remove(removeItem));

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItem?.Equals(removeItem));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == removeIndex);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnRemoveFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        T nonExeistItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanging += List_ListChanging;
        Assert.IsTrue(list.Remove(nonExeistItem) is false);
        Assert.IsTrue(cList.Remove(nonExeistItem) is false);

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangingOnRemoveAt<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);
        Assert.IsTrue(list.SequenceEqual(cList));
        var triggered = false;

        var removeItem = cList[index];
        list.ListChanging += List_ListChanging;
        list.RemoveAt(index);
        cList.RemoveAt(index);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItem?.Equals(removeItem));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnRemoveAtFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanging += List_ListChanging;
        try
        {
            list.RemoveAt(outRangeIndex);
            Assert.Fail();
        }
        catch { }

        try
        {
            cList.RemoveAt(outRangeIndex);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangingOnClear<T>(IObservableList<T> list, IList<T> comparisonList)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanging += List_ListChanging;
        list.Clear();
        cList.Clear();

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Clear);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == -1);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnReplace<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        var oldItem = cList[index];
        list.ListChanging += List_ListChanging;
        list[index] = newItem;
        cList[index] = newItem;

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Replace);
            Assert.IsTrue(e.OldItem?.Equals(oldItem));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangingOnReplaceFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanging += List_ListChanging;
        try
        {
            list[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        try
        {
            cList[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    #endregion

    #region ListChanged
    public static void ListChangedOnAdd<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanged += List_ListChanged;
        cList.Add(newItem);
        list.Add(newItem);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == cList.Count - 1);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnInsert<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanged += List_ListChanged;
        cList.Insert(index, newItem);
        list.Insert(index, newItem);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnInsertFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanged += List_ListChanged;
        try
        {
            list.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        try
        {
            cList.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangedOnRemove<T>(IObservableList<T> list, IList<T> comparisonList)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        var removeIndex = cList.RandomIndex();
        var removeItem = cList[removeIndex];
        list.ListChanged += List_ListChanged;
        Assert.IsTrue(cList.Remove(removeItem));
        Assert.IsTrue(list.Remove(removeItem));

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItem?.Equals(removeItem));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == removeIndex);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnRemoveFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        T nonExeistItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanged += List_ListChanged;
        Assert.IsTrue(cList.Remove(nonExeistItem) is false);
        Assert.IsTrue(list.Remove(nonExeistItem) is false);

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangedOnRemoveAt<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);
        Assert.IsTrue(list.SequenceEqual(cList));
        var triggered = false;

        var removeItem = cList[index];
        list.ListChanged += List_ListChanged;
        cList.RemoveAt(index);
        list.RemoveAt(index);

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItem?.Equals(removeItem));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnRemoveAtFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanged += List_ListChanged;
        try
        {
            list.RemoveAt(outRangeIndex);
            Assert.Fail();
        }
        catch { }

        try
        {
            cList.RemoveAt(outRangeIndex);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangedOnClear<T>(IObservableList<T> list, IList<T> comparisonList)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        list.ListChanged += List_ListChanged;
        cList.Clear();
        list.Clear();

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Clear);
            Assert.IsTrue(e.OldItem?.Equals(default(T)));
            Assert.IsTrue(e.NewItem?.Equals(default(T)));
            Assert.IsTrue(e.Index == -1);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnReplace<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int index,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var triggered = false;
        var oldItem = cList[index];
        list.ListChanged += List_ListChanged;
        cList[index] = newItem;
        list[index] = newItem;

        Assert.IsTrue(list.SequenceEqual(cList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Replace);
            Assert.IsTrue(e.OldItem?.Equals(oldItem));
            Assert.IsTrue(e.NewItem?.Equals(newItem));
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(cList));
        }
    }

    public static void ListChangedOnReplaceFalse<T>(
        IObservableList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        list.ListChanged += List_ListChanged;
        try
        {
            list[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        try
        {
            cList[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(list.SequenceEqual(cList));
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<T> sender, NotifyListChangeEventArgs<T> e)
        {
            Assert.Fail();
        }
    }
    #endregion
}
