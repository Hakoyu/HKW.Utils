using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

public class ObservableCollectionTests
{
    public static void Test(IObservableCollection<int> collection)
    {
        CollectionChangedOnAdd(collection);
        CollectionChangedOnRemove(collection);
        CollectionChangedOnRemoveFailed(collection);
        CollectionChangedOnClear(collection);

        PropertyChangedOnAdd(collection);
        PropertyChangedOnRemove(collection);
        PropertyChangedOnRemoveFailed(collection);
        PropertyChangedOnClear(collection);
    }

    #region CollectionChanged
    public static void CollectionChangedOnAdd(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        collection.CollectionChanged += Collection_CollectionChanged;
        comparisonList.Add(int.MaxValue);
        collection.Add(int.MaxValue);

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.Action is NotifyCollectionChangedAction.Add);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is int i && i == comparisonList.Last());
            Assert.IsTrue(e.NewStartingIndex == -1 || e.NewStartingIndex == collection.Count - 1);
            Assert.IsTrue(e.OldStartingIndex == -1);
            Assert.IsTrue(collection.SequenceEqual(comparisonList));
        }
    }

    public static void CollectionChangedOnRemove(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        var removeItem = comparisonList.First();
        collection.CollectionChanged += Collection_CollectionChanged;
        comparisonList.Remove(removeItem);
        collection.Remove(removeItem);

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.Action is NotifyCollectionChangedAction.Remove);
            Assert.IsTrue(e.OldItems?[0] is int i && i == removeItem);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == 0);
            Assert.IsTrue(collection.SequenceEqual(comparisonList));
        }
    }

    public static void CollectionChangedOnRemoveFailed(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        collection.CollectionChanged += Collection_CollectionChanged;
        Assert.IsTrue(collection.Remove(-1) == comparisonList.Remove(-1));

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void CollectionChangedOnClear(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        collection.CollectionChanged += Collection_CollectionChanged;
        comparisonList.Clear();
        collection.Clear();

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.CollectionChanged -= Collection_CollectionChanged;
        collection.Clear();

        void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.Action is NotifyCollectionChangedAction.Reset);
            Assert.IsTrue(e.OldItems?[0] is null);
            Assert.IsTrue(e.NewItems?[0] is null);
            Assert.IsTrue(e.NewStartingIndex == -1);
            Assert.IsTrue(e.OldStartingIndex == -1);
            Assert.IsTrue(collection.SequenceEqual(comparisonList));
        }
    }
    #endregion

    #region PropertyChanged
    public static void PropertyChangedOnAdd(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        comparisonList.Add(10);
        collection.Add(10);

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.PropertyName == nameof(collection.Count));
            Assert.IsTrue(comparisonList.Count == collection.Count);
        }
    }

    public static void PropertyChangedOnRemove(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        comparisonList.Remove(comparisonList.First());
        collection.Remove(collection.First());

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.PropertyName == nameof(collection.Count));
            Assert.IsTrue(comparisonList.Count == collection.Count);
        }
    }

    public static void PropertyChangedOnRemoveFailed(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        collection.PropertyChanged += Collection_PropertyChanged;
        Assert.IsTrue(collection.Remove(-1) == comparisonList.Remove(-1));

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Assert.Fail();
        }
    }

    public static void PropertyChangedOnClear(IObservableCollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        var triggered = false;
        collection.PropertyChanged += Collection_PropertyChanged;
        comparisonList.Clear();
        collection.Clear();

        Assert.IsTrue(collection.SequenceEqual(comparisonList));
        Assert.IsTrue(triggered);
        collection.PropertyChanged -= Collection_PropertyChanged;
        collection.Clear();

        void Collection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            triggered = true;
            Assert.IsTrue(sender?.Equals(collection));
            Assert.IsTrue(e.PropertyName == nameof(collection.Count));
            Assert.IsTrue(comparisonList.Count == collection.Count);
        }
    }
    #endregion
}
