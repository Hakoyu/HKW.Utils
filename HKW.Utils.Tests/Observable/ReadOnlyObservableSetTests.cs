using System.Collections.Specialized;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public class ReadOnlyObservableSetTests
{
    [TestMethod]
    public void ReadOnlyInterfaces()
    {
        var source = new ObservableSet<int>([1, 2, 3]);
        using var view = new ReadOnlyObservableSet<int>(source);
        var notifications = 0;
        view.SetChanging += (_, _) => notifications++;
        view.SetChanged += (_, _) => notifications++;
        view.CollectionChanged += (_, _) => notifications++;
        view.PropertyChanged += (_, _) => notifications++;
        IReadOnlySetTTestUtils.Test<int>(() => view, new[] { 4, 5 });
        Assert.ThrowsExactly<NotSupportedException>(() => ((ISet<int>)view).Add(4));
        Assert.AreEqual(0, notifications);
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, source.ToArray());
        Assert.IsTrue(view.SetEquals(new[] { 3, 2, 1, 1 }));
        Assert.IsFalse(view.SetEquals(new[] { 1, 2 }));
    }

    [TestMethod]
    [DataRow(SetChangeAction.Add)]
    [DataRow(SetChangeAction.Remove)]
    [DataRow(SetChangeAction.Clear)]
    public void ForwardsChanges(SetChangeAction action)
    {
        var source = new ObservableSet<int>([1, 2, 3]);
        var expected = new HashSet<int> { 1, 2, 3 };
        using var view = new ReadOnlyObservableSet<int>(source);
        var changing = new List<NotifySetChangeEventArgs<int>>();
        var changed = new List<NotifySetChangeEventArgs<int>>();
        var sourceCollection = new List<NotifyCollectionChangedEventArgs>();
        var viewCollection = new List<NotifyCollectionChangedEventArgs>();
        var sourceProperties = new List<string?>();
        var viewProperties = new List<string?>();
        var order = new List<string>();
        source.CollectionChanged += (_, e) => sourceCollection.Add(e);
        source.PropertyChanged += (_, e) => sourceProperties.Add(e.PropertyName);
        view.SetChanging += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, view.ToArray());
            changing.Add(e);
            order.Add("Changing");
        };
        view.SetChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            Assert.IsTrue(expected.SetEquals(view));
            changed.Add(e);
            order.Add("Changed");
        };
        view.CollectionChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            viewCollection.Add(e);
        };
        view.PropertyChanged += (sender, e) =>
        {
            Assert.AreSame(view, sender);
            viewProperties.Add(e.PropertyName);
        };
        switch (action)
        {
            case SetChangeAction.Add:
                expected.Add(4);
                Assert.IsTrue(source.Add(4));
                break;
            case SetChangeAction.Remove:
                expected.Remove(1);
                Assert.IsTrue(source.Remove(1));
                break;
            case SetChangeAction.Clear:
                expected.Clear();
                source.Clear();
                break;
            default:
                Assert.Fail("Unsupported test action.");
                break;
        }
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        CollectionAssert.AreEqual(new[] { "Changing", "Changed" }, order);
        foreach (var e in changing.Concat(changed))
        {
            Assert.AreEqual(action, e.Action);
            Assert.IsNull(e.OtherItems);
            if (action == SetChangeAction.Add)
                CollectionAssert.AreEqual(new[] { 4 }, e.NewItems!.ToArray());
            else
                Assert.IsNull(e.NewItems);
            if (action == SetChangeAction.Remove)
                CollectionAssert.AreEqual(new[] { 1 }, e.OldItems!.ToArray());
            else
                Assert.IsNull(e.OldItems);
        }
        Assert.AreEqual(1, sourceCollection.Count);
        Assert.AreEqual(1, viewCollection.Count);
        Assert.AreSame(sourceCollection[0], viewCollection[0]);
        CollectionAssert.AreEqual(new[] { "Count" }, sourceProperties);
        CollectionAssert.AreEqual(new[] { "Count" }, viewProperties);
        Assert.IsTrue(expected.SetEquals(view));
        Assert.IsFalse(source.Remove(-1));
        Assert.AreEqual(1, changing.Count);
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(1, viewCollection.Count);
        CollectionAssert.AreEqual(new[] { "Count" }, sourceProperties);
        CollectionAssert.AreEqual(new[] { "Count" }, viewProperties);
    }

    [TestMethod]
    public void DuplicateAdd_DoesNotNotify()
    {
        var source = new ObservableSet<int>([1, 2, 3]);
        using var view = new ReadOnlyObservableSet<int>(source);
        var count = 0;
        view.SetChanging += (_, _) => count++;
        view.SetChanged += (_, _) => count++;
        view.CollectionChanged += (_, _) => count++;
        view.PropertyChanged += (_, _) => count++;
        Assert.IsFalse(source.Add(2));
        Assert.AreEqual(0, count);
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, view.ToArray());
    }

    [TestMethod]
    public void Dispose_StopsAllForwardingWithoutDisposingSource()
    {
        var source = new ObservableSet<int>([1, 2]);
        using var view = new ReadOnlyObservableSet<int>(source);
        var count = 0;
        view.SetChanging += (_, _) => count++;
        view.SetChanged += (_, _) => count++;
        view.CollectionChanged += (_, _) => count++;
        view.PropertyChanged += (_, _) => count++;
        view.Dispose();
        view.Dispose();
        Assert.IsTrue(source.Add(3));
        Assert.AreEqual(0, count);
        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, source.ToArray());
    }
}

// 以下保留历史草稿；当前可执行契约测试位于上方。
//using System.Collections.Specialized;
//using HKW.HKWUtils.Extensions;
//using HKW.HKWUtils.Observable;

//namespace HKW.HKWUtilsTests.Observable;

//[TestClass]
//public class ReadOnlyObservableSetTests
//{
//    [TestMethod]
//    public void Adding()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanging += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Add);
//            Assert.IsTrue(e.NewItems![0] == 10);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Add(10);
//        Assert.IsTrue(readOnlyObservableSet.Count == 11);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void Added()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Add);
//            Assert.IsTrue(e.NewItems![0] == 10);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Add(10);
//        Assert.IsTrue(readOnlyObservableSet.Count == 11);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void Removing()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanging += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Remove);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems![0] == 0);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Remove(0);
//        Assert.IsTrue(readOnlyObservableSet.Count == 9);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void Removed()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Remove);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems![0] == 0);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Remove(0);
//        Assert.IsTrue(readOnlyObservableSet.Count == 9);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void Clearing()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanging += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Clear);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Clear();
//        Assert.IsTrue(readOnlyObservableSet.Count == 0);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void Cleared()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Clear);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.OtherItems == null);
//        };
//        observableSet.Clear();
//        Assert.IsTrue(readOnlyObservableSet.Count == 0);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void IntersectWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Intersect);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems != null);
//            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
//        };
//        observableSet.IntersectWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 5);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void ExceptWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Except);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems != null);
//            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
//        };
//        observableSet.ExceptWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 5);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void SymmetricExceptWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.SymmetricExcept);
//            Assert.IsTrue(e.NewItems != null);
//            Assert.IsTrue(e.OldItems != null);
//            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
//            // set1.Union(set2).Except(set1.Intersect(set2))
//        };
//        observableSet.SymmetricExceptWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 6);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void UnionWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.SetChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == SetChangeAction.Union);
//            Assert.IsTrue(e.NewItems != null);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.OtherItems?.SequenceEqual(ints));
//        };
//        observableSet.UnionWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 11);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_Add()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
//            Assert.IsTrue(e.NewItems?[0] is 10);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.Add(10);
//        Assert.IsTrue(readOnlyObservableSet.Last() == 10);
//        Assert.IsTrue(readOnlyObservableSet.Count == 11);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_Remove()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems?[0] is 0);
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.Remove(0);
//        Assert.IsTrue(readOnlyObservableSet.First() == 1);
//        Assert.IsTrue(readOnlyObservableSet.Count == 9);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_Reset()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Reset);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.Clear();
//        Assert.IsTrue(readOnlyObservableSet.Count == 0);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_IntersectWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems?.ItemsEqual(Enumerable.Range(0, 10).Except(ints)));
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.IntersectWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 5);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_ExceptWith()
//    {
//        var triggered = false;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Remove);
//            Assert.IsTrue(e.NewItems == null);
//            Assert.IsTrue(e.OldItems?.ItemsEqual(Enumerable.Range(0, 10).Intersect(ints)));
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.ExceptWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 5);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_SymmetricExceptWith()
//    {
//        var triggered = true;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//        };
//        observableSet.SymmetricExceptWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 6);
//        Assert.IsTrue(triggered);
//    }

//    [TestMethod]
//    public void CollectionChanged_UnionWith()
//    {
//        var triggered = true;
//        var set = Enumerable.Range(0, 10).ToHashSet();
//        var observableSet = new ObservableSet<int>(Enumerable.Range(0, 10));
//        var readOnlyObservableSet = new ReadOnlyObservableSet<int>(observableSet);
//        var ints = new int[] { 1, 3, 5, 7, 9, 11 };
//        readOnlyObservableSet.CollectionChanged += (s, e) =>
//        {
//            triggered = true;
//            Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);
//            Assert.IsTrue(e.NewItems?.ItemsEqual(readOnlyObservableSet.Union(ints)));
//            Assert.IsTrue(e.OldItems == null);
//            Assert.IsTrue(e.NewStartingIndex == -1);
//            Assert.IsTrue(e.OldStartingIndex == -1);
//        };
//        observableSet.UnionWith(ints);
//        Assert.IsTrue(readOnlyObservableSet.Count == 11);
//        Assert.IsTrue(triggered);
//    }
//}
