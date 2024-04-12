using System.Collections.Specialized;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableSetTests
{
    [TestMethod]
    public void Test()
    {
        //Test(new ObservableSet<int>());
    }

    public static void Test<T>(IObservableSet<T> set, ISet<T> comparisonSet, Func<T> createNewItem)
    {
        ObservableCollectionTests.Test(set, comparisonSet, createNewItem);
    }

    [TestMethod]
    public void SetChangingOnAdd<T>(IObservableSet<T> set, ISet<T> comparisonSet, T newItem)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.Add(newItem);
        cSet.Add(newItem);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Add);
            Assert.IsTrue(e.NewItems!.First()!.Equals(newItem));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    [TestMethod]
    public void SetChangingOnAddCancel<T>(IObservableSet<T> set, ISet<T> comparisonSet, T newItem)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.Add(newItem);
        //cSet.Add(newItem);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Add);
            Assert.IsTrue(e.NewItems!.First()!.Equals(newItem));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
            e.Cancel = true;
        }
    }

    [TestMethod]
    public void SetChangingOnRemove<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanging += Set_SetChanging;
        set.Remove(removeItem);
        cSet.Remove(removeItem);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    [TestMethod]
    public void SetChangingOnRemoveCancel<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanging += Set_SetChanging;
        set.Remove(removeItem);
        //cSet.Remove(removeItem);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
            e.Cancel = true;
        }
    }

    [TestMethod]
    public void SetChangingOnClear<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanging += Set_SetChanging;
        set.Clear();
        cSet.Clear();

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    [TestMethod]
    public void SetChangingOnClearCancel<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanging += Set_SetChanging;
        set.Clear();
        //cSet.Clear();

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
            e.Cancel = true;
        }
    }

    [TestMethod]
    public void SetChangingOnIntersectWith<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        var otherSet = cSet.RandomOrder().Skip(cSet.Count / 2);
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanging += Set_SetChanging;
        set.IntersectWith(otherSet);
        cSet.IntersectWith(otherSet);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangingEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Intersect);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.SequenceEqual(set.Intersect(otherSet)));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    [TestMethod]
    public void ExceptWith()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == SetChangeAction.Except);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems != null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
        };
        observableSet.ExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void SymmetricExceptWith()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == SetChangeAction.SymmetricExcept);
            Assert.IsTrue(e.NewItems != null);
            Assert.IsTrue(e.OldItems != null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
            // set1.Union(set2).Except(set1.Intersect(set2))
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 6);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void UnionWith()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.SetChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == SetChangeAction.Union);
            Assert.IsTrue(e.NewItems != null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
        };
        observableSet.UnionWith(ints);
        Assert.IsTrue(observableSet.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Add()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
            Assert.IsTrue(e.NewItems?[0] is 10);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Add(10);
        Assert.IsTrue(observableSet.Last() == 10);
        Assert.IsTrue(observableSet.Count == 11);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Remove()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?[0] is 0);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Remove(0);
        Assert.IsTrue(observableSet.ElementAt(0) == 1);
        Assert.IsTrue(observableSet.Count == 9);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_Reset()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Reset);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.Clear();
        Assert.IsTrue(observableSet.Count == 0);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_IntersectWith()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?.ItemsEqual(Enumerable.Range(0, 10).Except(ints)));
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.IntersectWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_ExceptWith()
    {
        var triggered = false;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
            Assert.IsTrue(e.NewItems == null);
            Assert.IsTrue(e.OldItems?.ItemsEqual(Enumerable.Range(0, 10).Intersect(ints)));
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.ExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 5);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_SymmetricExceptWith()
    {
        var triggered = true;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
        };
        observableSet.SymmetricExceptWith(ints);
        Assert.IsTrue(observableSet.Count == 6);
        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void CollectionChanged_UnionWith()
    {
        var triggered = true;
        var set = Enumerable.Range(0, 10).ToHashSet();
        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
        observableSet.CollectionChanged += (s, e) =>
        {
            triggered = true;
            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
            Assert.IsTrue(e.NewItems?.ItemsEqual(observableSet.Union(ints)));
            Assert.IsTrue(e.OldItems == null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
        };
        observableSet.UnionWith(ints);
        Assert.IsTrue(observableSet.Count == 11);
        Assert.IsTrue(triggered);
    }
}
