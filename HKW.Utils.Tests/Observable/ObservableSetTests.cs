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
        Test(
            new ObservableSet<int>(),
            Enumerable.Range(1, 10).ToHashSet(),
            Enumerable.Range(5, 10).ToHashSet()
        );
    }

    public static void Test<T>(IObservableSet<T> set, ISet<T> comparisonSet, ISet<T> otherSet)
    {
        // TODO: 单独设置CollectionChanged测试项
        //ObservableCollectionTests.Test(set, comparisonSet, createNewItem);

        SetChangingOnAdd(set, comparisonSet, otherSet.Last());
        SetChangingOnRemove(set, comparisonSet);
        SetChangingOnClear(set, comparisonSet);
        SetChangingOnIntersectWith(set, comparisonSet, otherSet);
        SetChangingOnExceptWith(set, comparisonSet, otherSet);
        SetChangingOnSymmetricExceptWith(set, comparisonSet, otherSet);
        SetChangingOnUnionWith(set, comparisonSet, otherSet);

        SetChangedOnAdd(set, comparisonSet, otherSet.Last());
        SetChangedOnRemove(set, comparisonSet);
        SetChangedOnClear(set, comparisonSet);
        SetChangedOnIntersectWith(set, comparisonSet, otherSet);
        SetChangedOnExceptWith(set, comparisonSet, otherSet);
        SetChangedOnSymmetricExceptWith(set, comparisonSet, otherSet);
        SetChangedOnUnionWith(set, comparisonSet, otherSet);
    }

    #region Changing
    public static void SetChangingOnAdd<T>(IObservableSet<T> set, ISet<T> comparisonSet, T newItem)
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

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Add);
            Assert.IsTrue(e.NewItems!.First()!.Equals(newItem));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnRemove<T>(IObservableSet<T> set, ISet<T> comparisonSet)
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

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnClear<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.Clear();
        cSet.Clear();

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnIntersectWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.IntersectWith(otherSet);
        cSet.IntersectWith(otherSet);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Intersect);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.SequenceEqual(set.Except(otherSet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnExceptWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.ExceptWith(otherSet);
        cSet.ExceptWith(otherSet);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Except);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.SequenceEqual(set.Intersect(otherSet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnSymmetricExceptWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.SymmetricExceptWith(otherSet);
        cSet.SymmetricExceptWith(otherSet);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.SymmetricExcept);
            Assert.IsTrue(e.NewItems!.SequenceEqual(otherSet.Except(otherSet.Intersect(set))));
            Assert.IsTrue(e.OldItems!.SequenceEqual(otherSet.Intersect(set)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangingOnUnionWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanging += Set_SetChanging;
        set.UnionWith(otherSet);
        cSet.UnionWith(otherSet);

        Assert.IsTrue(set.SequenceEqual(cSet));
        Assert.IsTrue(triggered);
        set.SetChanging -= Set_SetChanging;
        set.Clear();

        void Set_SetChanging(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Union);
            Assert.IsTrue(e.NewItems!.SequenceEqual(set.Except(otherSet)));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }
    #endregion

    #region Changed
    public static void SetChangedOnAdd<T>(IObservableSet<T> set, ISet<T> comparisonSet, T newItem)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.Add(newItem);
        set.Add(newItem);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Add);
            Assert.IsTrue(e.NewItems!.First()!.Equals(newItem));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnRemove<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        var removeItem = cSet.Random();
        set.SetChanged += Set_SetChanged;
        cSet.Remove(removeItem);
        set.Remove(removeItem);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Remove);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.First()!.Equals(removeItem));
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnClear<T>(IObservableSet<T> set, ISet<T> comparisonSet)
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.Clear();
        set.Clear();

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Clear);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems is null);
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnIntersectWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.IntersectWith(otherSet);
        set.IntersectWith(otherSet);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Intersect);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.SequenceEqual(comparisonSet.Except(otherSet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnExceptWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.ExceptWith(otherSet);
        set.ExceptWith(otherSet);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Except);
            Assert.IsTrue(e.NewItems is null);
            Assert.IsTrue(e.OldItems!.SequenceEqual(comparisonSet.Intersect(otherSet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnSymmetricExceptWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.SymmetricExceptWith(otherSet);
        set.SymmetricExceptWith(otherSet);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.SymmetricExcept);
            Assert.IsTrue(
                e.NewItems!.SequenceEqual(otherSet.Except(otherSet.Intersect(comparisonSet)))
            );
            Assert.IsTrue(e.OldItems!.SequenceEqual(otherSet.Intersect(comparisonSet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }

    public static void SetChangedOnUnionWith<T>(
        IObservableSet<T> set,
        ISet<T> comparisonSet,
        ISet<T> otherSet
    )
    {
        set.Clear();
        var triggered = false;
        var cSet = comparisonSet.ToHashSet();
        set.AddRange(cSet);

        set.SetChanged += Set_SetChanged;
        cSet.UnionWith(otherSet);
        set.UnionWith(otherSet);

        Assert.IsTrue(triggered);
        set.SetChanged -= Set_SetChanged;
        set.Clear();

        void Set_SetChanged(IObservableSet<T> sender, NotifySetChangeEventArgs<T> e)
        {
            triggered = true;
            Assert.IsTrue(e.Action is SetChangeAction.Union);
            Assert.IsTrue(e.NewItems!.SequenceEqual(comparisonSet.Except(otherSet)));
            Assert.IsTrue(e.OldItems is null);
            Assert.IsTrue(e.OtherItems!.SequenceEqual(otherSet));
            Assert.IsTrue(set.SequenceEqual(cSet));
        }
    }
    #endregion
}
