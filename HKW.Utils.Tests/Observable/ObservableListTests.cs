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
        Test(new ObservableList<int>());
    }

    public static void Test(IObservableList<int> list)
    {
        IListFindTests.Test(list);
        ObservableCollectionTests.Test(list);

        ListChangingOnInsert(list, 0);
        ListChangingOnInsert(list, 4);
        ListChangingOnInsert(list, 9);
        ListChangingOnRemoveAt(list, 0);
        ListChangingOnRemoveAt(list, 4);
        ListChangingOnRemoveAt(list, 9);
        ListChangingOnRemoveAtFailed(list);

        ListChangedOnInsert(list, 0);
        ListChangedOnInsert(list, 4);
        ListChangedOnInsert(list, 9);
        ListChangedOnRemoveAt(list, 0);
        ListChangedOnRemoveAt(list, 4);
        ListChangedOnRemoveAt(list, 9);
        ListChangedOnRemoveAtFailed(list);
    }

    #region ListChanging

    public static void ListChangingOnAdd(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var addeditem = int.MaxValue;
        list.ListChanging += List_ListChanging;
        list.Add(addeditem);
        comparisonList.Add(addeditem);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == addeditem);
            Assert.IsTrue(e.Index == comparisonList.Count - 1);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangingOnAdd_Cancel(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var addeditem = int.MaxValue;
        list.ListChanging += List_ListChanging;
        list.Add(addeditem);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == addeditem);
            Assert.IsTrue(e.Index == comparisonList.Count - 1);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
            e.Cancel = true;
        }
    }

    public static void ListChangingOnInsert(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var addeditem = int.MaxValue;
        list.ListChanging += List_ListChanging;
        list.Insert(index, addeditem);
        comparisonList.Insert(index, addeditem);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == addeditem);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangingOnInsert_Cancel(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var addeditem = int.MaxValue;
        list.ListChanging += List_ListChanging;
        list.Insert(index, addeditem);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == addeditem);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
            e.Cancel = true;
        }
    }

    public static void ListChangingOnRemoveAt(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var removedItem = comparisonList[index];
        list.ListChanging += List_ListChanging;
        list.RemoveAt(index);
        comparisonList.RemoveAt(index);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0] is int i && i == removedItem);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangingOnRemoveAt_Cancel(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var removedItem = comparisonList[index];
        list.ListChanging += List_ListChanging;
        list.RemoveAt(index);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0] is int i && i == removedItem);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
            e.Cancel = true;
        }
    }

    public static void ListChangingOnRemoveAtFailed(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.ListChanging += List_ListChanging;

        Assert.IsTrue(list.Remove(-1) == comparisonList.Remove(-1));
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        Assert.IsTrue(list.Remove(int.MaxValue) == comparisonList.Remove(int.MaxValue));
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.ListChanging -= List_ListChanging;
        list.Clear();

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            Assert.Fail();
        }
    }

    public static void ListChangingOnClear(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        list.ListChanging += List_ListChanging;
        comparisonList.Clear();
        list.Clear();

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Clear);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.Index == -1);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangingOnClear_Cancel(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        list.ListChanging += List_ListChanging;
        list.Clear();

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanging -= List_ListChanging;

        void List_ListChanging(IObservableList<int> sender, NotifyListChangingEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Clear);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.Index == -1);
            e.Cancel = true;
        }
    }
    #endregion

    #region ListChanged

    public static void ListChangedOnInsert(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var addedItem = int.MaxValue;
        list.ListChanged += List_ListChanged;
        comparisonList.Insert(index, addedItem);
        list.Insert(index, addedItem);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<int> sender, NotifyListChangedEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == addedItem);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangedOnRemoveAt(IObservableList<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        var triggered = false;
        var removedItem = comparisonList[index];
        list.ListChanged += List_ListChanged;
        comparisonList.RemoveAt(index);
        list.RemoveAt(index);

        Assert.IsTrue(list.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<int> sender, NotifyListChangedEventArgs<int> e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(list));
            Assert.IsTrue(e.Action is ListChangeAction.Remove);
            Assert.IsTrue(e.OldItems?[0] is int i && i == removedItem);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.Index == index);
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }
    }

    public static void ListChangedOnRemoveAtFailed(IObservableList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.ListChanged += List_ListChanged;
        Assert.IsTrue(list.Remove(-1) == comparisonList.Remove(-1));
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        Assert.IsTrue(list.Remove(int.MaxValue) == comparisonList.Remove(int.MaxValue));
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.ListChanged -= List_ListChanged;
        list.Clear();

        void List_ListChanged(IObservableList<int> sender, NotifyListChangedEventArgs<int> e)
        {
            Assert.Fail();
        }
    }
    #endregion
}
