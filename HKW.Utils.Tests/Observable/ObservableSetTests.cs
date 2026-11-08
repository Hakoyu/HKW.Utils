using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public sealed class ObservableSetTests : ObservableSetTestsBase
{
    protected override IObservableSet<string> CreateSet() =>
        new ObservableSet<string>(Enumerable.Range(1, 3).Select(i => i.ToString()));
}

[TestClass]
public sealed class ObservableSetWrapperTests : ObservableSetTestsBase
{
    protected override IObservableSet<string> CreateSet() =>
        new ObservableSetWrapper<string, OrderedHashSet<string>>(
            new OrderedHashSet<string>(Enumerable.Range(1, 3).Select(i => i.ToString())),
            null
        );
}

public abstract class ObservableSetTestsBase
{
    protected abstract IObservableSet<string> CreateSet();

    static IReadOnlyCollection<string> _newItems = new ReadOnlyCollection<string>(
        Enumerable.Range(100, 5).Select(i => i.ToString()).ToList()
    );

    [TestMethod]
    public void ISetTest()
    {
        ISetTTestUtils.Test(CreateSet, _newItems);
    }

    [TestMethod]
    public void ObservableCollectionTest()
    {
        ObservableCollectionUtils.Test(CreateSet, _newItems);
    }

    [TestMethod]
    public void Add_WithDuplicateAndOverlappingItems()
    {
        var set = CreateSet();
        var expected = set.ToHashSet();
        var changing = 0;
        var changed = 0;
        set.SetChanging += (sender, e) =>
        {
            Assert.AreSame(set, sender);
            Assert.AreEqual(SetChangeAction.Add, e.Action);
            changing++;
        };
        set.SetChanged += (sender, e) =>
        {
            Assert.AreSame(set, sender);
            Assert.AreEqual(SetChangeAction.Add, e.Action);
            changed++;
        };
        var successfulAdds = 0;
        foreach (var item in new[] { "1", "100", "100", "101", "102" })
        {
            var added = expected.Add(item);
            Assert.AreEqual(added, set.Add(item));
            successfulAdds += added ? 1 : 0;
            Assert.AreEqual(successfulAdds, changing);
            Assert.AreEqual(successfulAdds, changed);
            Assert.IsTrue(expected.SetEquals(set));
        }
        Assert.AreEqual(3, successfulAdds);
    }

    [TestMethod]
    public void ExceptWith_PartialOverlapAndNoOp()
    {
        var set = CreateSet();
        var before = set.ToHashSet();
        string[] other = ["2", "2", "100"];
        var expected = before.Except(other).ToHashSet();
        var removed = before.Intersect(other).ToArray();
        var changing = new List<NotifySetChangeEventArgs<string>>();
        var changed = new List<NotifySetChangeEventArgs<string>>();
        set.SetChanging += (sender, e) =>
        {
            Assert.AreSame(set, sender);
            Assert.IsTrue(before.SetEquals(sender));
            changing.Add(e);
        };
        set.SetChanged += (sender, e) =>
        {
            Assert.AreSame(set, sender);
            Assert.IsTrue(expected.SetEquals(sender));
            changed.Add(e);
        };
        set.ExceptWith(other);
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        foreach (var e in changing.Concat(changed))
        {
            Assert.AreEqual(SetChangeAction.Except, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsNotNull(e.OldItems);
            Assert.IsNotNull(e.OtherItems);
            CollectionAssert.AreEquivalent(removed, e.OldItems.ToArray());
            CollectionAssert.AreEqual(other, e.OtherItems.ToArray());
        }
        Assert.IsTrue(expected.SetEquals(set));
        set.ExceptWith(other);
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        Assert.IsTrue(expected.SetEquals(set));
    }

    #region Changing
    [TestMethod]
    public void ChangingOnAdd()
    {
        var set = CreateSet();
        var expectedCount = _newItems.Distinct().Except(set).Count();

        var triggerCount = 0;
        var newItem = default(string);

        set.SetChanging += Set_SetChanging;
        foreach (var item in _newItems)
        {
            newItem = item;
            Assert.IsTrue(set.Add(item));
        }

        foreach (var item in _newItems)
        {
            Assert.IsFalse(set.Add(item));
        }
        set.SetChanging -= Set_SetChanging;

        Assert.AreEqual(expectedCount, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Add, e.Action);
            Assert.HasCount(1, e.NewItems!);
            Assert.AreEqual(newItem, e.NewItems!.First());
            Assert.IsNull(e.OldItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangingOnRemove()
    {
        var set = CreateSet();
        var oldCount = set.Count;

        var triggerCount = 0;
        var removeItem = default(string);

        set.SetChanging += Set_SetChanging;
        foreach (var item in set.ToArray())
        {
            removeItem = item;
            Assert.IsTrue(set.Remove(item));
        }

        foreach (var item in _newItems)
        {
            Assert.IsFalse(set.Remove(item));
        }
        set.SetChanging -= Set_SetChanging;

        Assert.AreEqual(oldCount, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Remove, e.Action);
            Assert.HasCount(1, e.OldItems!);
            Assert.AreEqual(removeItem, e.OldItems!.First());
            Assert.IsNull(e.NewItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangingOnClear()
    {
        var set = CreateSet();

        var triggerCount = 0;

        set.SetChanging += Set_SetChanging;
        set.Clear();
        set.SetChanging -= Set_SetChanging;

        Assert.AreEqual(1, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Clear, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsNull(e.OldItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangingOnIntersectWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanging += Set_SetChanging;
        set.IntersectWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanging += Set_SetChanging;
        set.IntersectWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanging += Set_SetChanging;
        set.IntersectWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(0, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Intersect, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsTrue(copySet.Except(currentOtherSet).ToHashSet().SetEquals(e.OldItems!));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangingOnExceptWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanging += Set_SetChanging;
        set.ExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanging += Set_SetChanging;
        set.ExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanging += Set_SetChanging;
        set.ExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Except, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsTrue(copySet.Intersect(currentOtherSet).ToHashSet().SetEquals(e.OldItems!));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangingOnSymmetricExceptWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanging += Set_SetChanging;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanging += Set_SetChanging;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanging += Set_SetChanging;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.SymmetricExcept, e.Action);
            Assert.IsTrue(currentOtherSet.Except(copySet).ToHashSet().SetEquals(e.NewItems!));
            Assert.IsTrue(e.OldItems!.SequenceEqual(currentOtherSet.Intersect(copySet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangingOnUnionWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanging += Set_SetChanging;
        set.UnionWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanging += Set_SetChanging;
        set.UnionWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanging += Set_SetChanging;
        set.UnionWith(currentOtherSet);
        set.SetChanging -= Set_SetChanging;
        Assert.AreEqual(1, triggerCount);

        Assert.AreEqual(1, triggerCount);

        void Set_SetChanging(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Union, e.Action);
            Assert.IsTrue(e.NewItems!.SequenceEqual(currentOtherSet.Except(copySet)));
            Assert.IsNull(e.OldItems);
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }
    #endregion

    #region Changed
    [TestMethod]
    public void ChangedOnAdd()
    {
        var set = CreateSet();
        var expectedCount = _newItems.Distinct().Except(set).Count();

        var triggerCount = 0;
        var newItem = default(string);

        set.SetChanged += Set_SetChanged;
        foreach (var item in _newItems)
        {
            newItem = item;
            Assert.IsTrue(set.Add(item));
        }

        foreach (var item in _newItems)
        {
            Assert.IsFalse(set.Add(item));
        }
        set.SetChanged -= Set_SetChanged;

        Assert.AreEqual(expectedCount, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Add, e.Action);
            Assert.HasCount(1, e.NewItems!);
            Assert.AreEqual(newItem, e.NewItems!.First());
            Assert.IsNull(e.OldItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangedOnRemove()
    {
        var set = CreateSet();
        var oldCount = set.Count;

        var triggerCount = 0;
        var removeItem = default(string);

        set.SetChanged += Set_SetChanged;
        foreach (var item in set.ToArray())
        {
            removeItem = item;
            Assert.IsTrue(set.Remove(item));
        }

        foreach (var item in _newItems)
        {
            Assert.IsFalse(set.Remove(item));
        }
        set.SetChanged -= Set_SetChanged;

        Assert.AreEqual(oldCount, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Remove, e.Action);
            Assert.HasCount(1, e.OldItems!);
            Assert.AreEqual(removeItem, e.OldItems!.First());
            Assert.IsNull(e.NewItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangedOnClear()
    {
        var set = CreateSet();
        var triggerCount = 0;

        set.SetChanged += Set_SetChanged;
        set.Clear();
        set.SetChanged -= Set_SetChanged;

        Assert.AreEqual(1, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Clear, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsNull(e.OldItems);
            Assert.IsNull(e.OtherItems);
        }
    }

    [TestMethod]
    public void ChangedOnIntersectWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanged += Set_SetChanged;
        set.IntersectWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanged += Set_SetChanged;
        set.IntersectWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanged += Set_SetChanged;
        set.IntersectWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(0, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Intersect, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsTrue(copySet.Except(currentOtherSet).ToHashSet().SetEquals(e.OldItems!));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangedOnExceptWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanged += Set_SetChanged;
        set.ExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanged += Set_SetChanged;
        set.ExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanged += Set_SetChanged;
        set.ExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Except, e.Action);
            Assert.IsNull(e.NewItems);
            Assert.IsTrue(copySet.Intersect(currentOtherSet).ToHashSet().SetEquals(e.OldItems!));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangedOnSymmetricExceptWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanged += Set_SetChanged;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanged += Set_SetChanged;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanged += Set_SetChanged;
        set.SymmetricExceptWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.SymmetricExcept, e.Action);
            Assert.IsTrue(currentOtherSet.Except(copySet).ToHashSet().SetEquals(e.NewItems!));
            Assert.IsTrue(e.OldItems!.SequenceEqual(currentOtherSet.Intersect(copySet)));
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }

    [TestMethod]
    public void ChangedOnUnionWith()
    {
        var set = CreateSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(_newItems).ToHashSet();
        var currentOtherSet = default(IReadOnlyCollection<string>);
        var triggerCount = 0;

        triggerCount = 0;
        currentOtherSet = copySet;
        set.SetChanged += Set_SetChanged;
        set.UnionWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(0, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = _newItems;
        set.SetChanged += Set_SetChanged;
        set.UnionWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        set = CreateSet();
        triggerCount = 0;
        currentOtherSet = concatSet;
        set.SetChanged += Set_SetChanged;
        set.UnionWith(currentOtherSet);
        set.SetChanged -= Set_SetChanged;
        Assert.AreEqual(1, triggerCount);

        Assert.AreEqual(1, triggerCount);

        void Set_SetChanged(IObservableSet<string> sender, NotifySetChangeEventArgs<string> e)
        {
            triggerCount++;
            Assert.AreEqual(SetChangeAction.Union, e.Action);
            Assert.IsTrue(e.NewItems!.SequenceEqual(currentOtherSet.Except(copySet)));
            Assert.IsNull(e.OldItems);
            Assert.IsTrue(e.OtherItems!.SequenceEqual(currentOtherSet));
        }
    }
    #endregion
}
