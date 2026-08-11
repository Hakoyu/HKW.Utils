using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

public static class ObservableCollectionUtils
{
    public static void Test<T>(
        Func<IObservableCollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        ICollectionTTestUtils.Test(createCollection, newItems);
        CollectionChangedOnAdd(createCollection, newItems);
        CollectionChangedOnRemove(createCollection);
        CollectionChangedOnRemoveFailed(createCollection, newItems);
        CollectionChangedOnClear(createCollection);

        PropertyChangedOnAdd(createCollection, newItems);
        PropertyChangedOnRemove(createCollection);
        PropertyChangedOnRemoveFailed(createCollection, newItems);
        PropertyChangedOnClear(createCollection);
    }

    #region CollectionChanged
    public static void CollectionChangedOnAdd<T>(
        Func<IObservableCollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var oldCount = collection.Count;

        var triggerCount = 0;
        var addItem = default(T);
        var addIndex = -1;
        collection.CollectionChanged += Collection_CollectionChanged;
        foreach (var item in newItems)
        {
            Assert.IsNotNull(item);
            addItem = item;
            addIndex = collection.Count;
            cCollection.Add(item);
            collection.Add(item);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
        collection.CollectionChanged -= Collection_CollectionChanged;

        Assert.HasCount(triggerCount, newItems);
        Assert.HasCount(oldCount + newItems.Count, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.IsNull(e.OldItems);
            Assert.AreEqual(addItem, [e.NewItems?[0], collection.Last(), cCollection[^1]]);
            // 如果添加至末尾, NewStartingIndex可为 Count - 1 或 -1
            if ((e.NewStartingIndex == addIndex || e.NewStartingIndex == -1) is false)
                Assert.Fail();
            Assert.AreEqual(-1, e.OldStartingIndex);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void CollectionChangedOnRemove<T>(Func<IObservableCollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var removeItems = collection.ToArray();

        var triggerCount = 0;
        var removeItem = default(T);
        var removeIndex = 0;

        collection.CollectionChanged += Collection_CollectionChanged;
        foreach (var (e, i) in removeItems.ReverseWithIndex())
        {
            removeItem = e;
            removeIndex = i;
            cCollection.Remove(e);
            collection.Remove(e);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
        collection.CollectionChanged -= Collection_CollectionChanged;

        Assert.HasCount(triggerCount, removeItems);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.AreEqual(removeItem, e.OldItems?[0]);
            Assert.IsNull(e.NewItems?[0]);
            Assert.AreEqual(-1, e.NewStartingIndex);
            Assert.AreEqual(removeIndex, e.OldStartingIndex);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void CollectionChangedOnRemoveFailed<T>(
        Func<IObservableCollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var oldCount = collection.Count;

        collection.CollectionChanged += Collection_CollectionChanged;
        foreach (var item in newItems)
        {
            Assert.AreEqual(false, [cCollection.Remove(item), collection.Remove(item)]);
        }
        collection.CollectionChanged -= Collection_CollectionChanged;

        Assert.HasCount(oldCount, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void CollectionChangedOnClear<T>(Func<IObservableCollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        var triggerCount = 0;

        collection.CollectionChanged += Collection_CollectionChanged;
        cCollection.Clear();
        collection.Clear();
        collection.CollectionChanged -= Collection_CollectionChanged;

        Assert.AreEqual(1, triggerCount);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, e.Action);
            Assert.IsNull(e.OldItems?[0]);
            Assert.IsNull(e.NewItems?[0]);
            Assert.AreEqual(-1, e.NewStartingIndex);
            Assert.AreEqual(-1, e.OldStartingIndex);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }
    #endregion

    #region PropertyChanged
    public static void PropertyChangedOnAdd<T>(
        Func<IObservableCollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        var triggerCount = 0;

        collection.PropertyChanged += Collection_PropertyChanged;
        foreach (var item in newItems)
        {
            cCollection.Add(item);
            collection.Add(item);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
        collection.PropertyChanged -= Collection_PropertyChanged;

        Assert.HasCount(triggerCount, newItems);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void PropertyChangedOnRemove<T>(Func<IObservableCollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var removeItems = collection.ToArray();

        var triggerCount = 0;

        collection.PropertyChanged += Collection_PropertyChanged;
        foreach (var item in removeItems)
        {
            cCollection.Remove(item);
            collection.Remove(item);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
        collection.PropertyChanged -= Collection_PropertyChanged;

        Assert.HasCount(triggerCount, removeItems);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void PropertyChangedOnRemoveFailed<T>(
        Func<IObservableCollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var oldCount = collection.Count;

        collection.PropertyChanged += Collection_PropertyChanged;
        foreach (var item in newItems)
        {
            Assert.AreEqual(cCollection.Remove(item), collection.Remove(item));
        }
        collection.PropertyChanged -= Collection_PropertyChanged;

        Assert.HasCount(oldCount, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void PropertyChangedOnClear<T>(Func<IObservableCollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        var triggerCount = 0;

        collection.PropertyChanged += Collection_PropertyChanged;
        cCollection.Clear();
        collection.Clear();
        collection.PropertyChanged -= Collection_PropertyChanged;

        Assert.AreEqual(1, triggerCount);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggerCount++;
            Assert.AreEqual(sender, collection);
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }
    #endregion
}
