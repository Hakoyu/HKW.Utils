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

public class ObservableCollectionTests
{
    public static void Test<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection,
        Func<T> createNewItem
    )
    {
        ICollectionTestUtils.Test(collection, comparisonCollection, createNewItem);
        CollectionChangedOnAdd(collection, comparisonCollection, createNewItem());
        CollectionChangedOnRemove(collection, comparisonCollection);
        CollectionChangedOnRemoveFailed(collection, comparisonCollection, createNewItem());
        CollectionChangedOnClear(collection, comparisonCollection);

        PropertyChangedOnAdd(collection, comparisonCollection, createNewItem());
        PropertyChangedOnRemove(collection, comparisonCollection);
        PropertyChangedOnRemoveFailed(collection, comparisonCollection, createNewItem());
        PropertyChangedOnClear(collection, comparisonCollection);
    }

    #region CollectionChanged
    public static void CollectionChangedOnAdd<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection,
        T newItem
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        collection.CollectionChanged += Collection_CollectionChanged;
        cCollection.Add(newItem);
        collection.Add(newItem);

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.IsNull(e.OldItems?[0]);
            Assert.IsTrue(e.NewItems?[0]?.Equals(cCollection.Last()));
            Assert.AreEqual(collection.Count - 1, e.NewStartingIndex);
            Assert.AreEqual(-1, e.OldStartingIndex);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void CollectionChangedOnRemove<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        var removeIndex = 0;
        var removeItem = cCollection[removeIndex];
        collection.CollectionChanged += Collection_CollectionChanged;
        cCollection.Remove(removeItem);
        collection.Remove(removeItem);

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.IsTrue(e.OldItems?[0]?.Equals(removeItem));
            Assert.IsNull(e.NewItems?[0]);
            Assert.AreEqual(-1, e.NewStartingIndex);
            Assert.AreEqual(removeIndex, e.OldStartingIndex);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
    }

    public static void CollectionChangedOnRemoveFailed<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection,
        T nonExeistItem
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        collection.CollectionChanged += Collection_CollectionChanged;
        Assert.AreEqual(cCollection.Remove(nonExeistItem), collection.Remove(nonExeistItem));

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void CollectionChangedOnClear<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        collection.CollectionChanged += Collection_CollectionChanged;
        cCollection.Clear();
        collection.Clear();

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
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
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection,
        T newItem
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        cCollection.Add(newItem);
        collection.Add(newItem);

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
        }
    }

    public static void PropertyChangedOnRemove<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        cCollection.Remove(cCollection.First());
        collection.Remove(collection.First());

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
        }
    }

    public static void PropertyChangedOnRemoveFailed<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection,
        T nonExeistItem
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        collection.PropertyChanged += Collection_PropertyChanged;
        Assert.AreEqual(cCollection.Remove(nonExeistItem), collection.Remove(nonExeistItem));

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void PropertyChangedOnClear<T>(
        IObservableCollection<T> collection,
        ICollection<T> comparisonCollection
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        cCollection.Clear();
        collection.Clear();

        Assert.IsTrue(collection.SequenceEqual(cCollection));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.AreEqual(nameof(collection.Count), e.PropertyName);
            Assert.HasCount(collection.Count, cCollection);
        }
    }
    #endregion
}
