using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
[DoNotParallelize]
public class BindingCollectionTests
{
    [TestMethod]
    public void BindingListX_NonEmptyTargetsAndRepeatedBinding()
    {
        var source = new ObservableList<int>([1, 2, 3]);
        var target = new List<int> { 1, 2, 3 };
        try
        {
            ObservableCollectionExtensions.BindingListX(source, target);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ObservableCollectionExtensions.BindingListX(source, target)
            );
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, target);
            source.Insert(1, 9);
            CollectionAssert.AreEqual(new[] { 1, 9, 2, 3 }, target);
            source[2] = 8;
            CollectionAssert.AreEqual(new[] { 1, 9, 8, 3 }, target);
            source.RemoveAt(1);
            CollectionAssert.AreEqual(new[] { 1, 8, 3 }, target);
            source.Clear();
            Assert.AreEqual(0, target.Count);
            ObservableCollectionExtensions.BindingListX(source, target, true);
            source.Add(7);
            Assert.AreEqual(0, target.Count);
        }
        finally
        {
            ObservableCollectionExtensions.BindingListX(source, target, true);
        }
    }

    [TestMethod]
    public void BindingList()
    {
        var obList = new ObservableList<int>();
        var list = new List<int>();
        try
        {
            obList.BindingList(list);
            obList.Add(1);
            obList.Add(2);
            obList.Add(3);
            obList.Add(4);
            obList.Add(5);
            Assert.IsTrue(obList.SequenceEqual(list));
            obList.Remove(1);
            obList.Remove(3);
            Assert.IsTrue(obList.SequenceEqual(list));
            obList[0] = 5;
            obList[2] = 1;
            Assert.IsTrue(obList.SequenceEqual(list));
            obList.Clear();
            Assert.IsTrue(obList.SequenceEqual(list));
            obList.BindingList(list, true);
            obList.Add(1);
            Assert.IsTrue(obList.SequenceEqual(list) is false);
        }
        finally
        {
            obList.BindingList(list, true);
        }
    }

    [TestMethod]
    public void BindingListX()
    {
        var obList = new ObservableList<int>();
        var list = new List<int>();
        try
        {
            ObservableCollectionExtensions.BindingListX(obList, list);
            obList.Add(1);
            obList.Add(2);
            obList.Add(3);
            obList.Add(4);
            obList.Add(5);
            Assert.IsTrue(obList.SequenceEqual(list));
            obList.Insert(1, 99);
            CollectionAssert.AreEqual(new[] { 1, 99, 2, 3, 4, 5 }, list.ToArray());
            obList.RemoveAt(1);
            obList.Remove(1);
            obList.Remove(3);
            Assert.IsTrue(obList.SequenceEqual(list));
            obList[0] = 5;
            obList[2] = 1;
            Assert.IsTrue(obList.SequenceEqual(list));
            obList.Clear();
            Assert.IsTrue(obList.SequenceEqual(list));
            ObservableCollectionExtensions.BindingListX(obList, list, true);
            obList.Add(1);
            Assert.IsTrue(obList.SequenceEqual(list) is false);
        }
        finally
        {
            ObservableCollectionExtensions.BindingListX(obList, list, true);
        }
    }

    [TestMethod]
    public void BindingSet()
    {
        var obSet = new ObservableSet<int>();
        var set = new HashSet<int>();
        try
        {
            obSet.BindingSet(set);
            obSet.Add(1);
            obSet.Add(2);
            obSet.Add(3);
            obSet.Add(4);
            obSet.Add(5);
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.Remove(1);
            obSet.Remove(3);
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.Clear();
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.BindingSet(set, true);
            obSet.Add(1);
            Assert.IsTrue(set.SetEquals(obSet) is false);
        }
        finally
        {
            obSet.BindingSet(set, true);
        }
    }

    [TestMethod]
    public void BindingSetX()
    {
        var obSet = new ObservableSet<int>();
        var set = new HashSet<int>();
        try
        {
            obSet.BindingSetX(set);
            obSet.Add(1);
            obSet.Add(2);
            obSet.Add(3);
            obSet.Add(4);
            obSet.Add(5);
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.Remove(1);
            obSet.Remove(3);
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.Clear();
            Assert.IsTrue(set.SetEquals(obSet));
            obSet.BindingSetX(set, true);
            obSet.Add(1);
            Assert.IsTrue(set.SetEquals(obSet) is false);
        }
        finally
        {
            obSet.BindingSetX(set, true);
        }
    }

    [TestMethod]
    public void BindingDictionary()
    {
        var obDictionary = new ObservableDictionary<int, int>();
        var list = new Dictionary<int, int>();
        try
        {
            obDictionary.BindingDictionary(list);
            obDictionary.Add(1, 1);
            obDictionary.Add(2, 2);
            obDictionary.Add(3, 3);
            obDictionary.Add(4, 4);
            obDictionary.Add(5, 5);
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.Remove(1);
            obDictionary.Remove(3);
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary[2] = 5;
            obDictionary[4] = 1;
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.Clear();
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.BindingDictionary(list, true);
            obDictionary.Add(1, 1);
            Assert.IsTrue(obDictionary.SequenceEqual(list) is false);
        }
        finally
        {
            obDictionary.BindingDictionary(list, true);
        }
    }

    [TestMethod]
    public void BindingDictionaryX()
    {
        var obDictionary = new ObservableDictionary<int, int>();
        var list = new Dictionary<int, int>();
        try
        {
            obDictionary.BindingDictionaryX(list);
            obDictionary.Add(1, 1);
            obDictionary.Add(2, 2);
            obDictionary.Add(3, 3);
            obDictionary.Add(4, 4);
            obDictionary.Add(5, 5);
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.Remove(1);
            obDictionary.Remove(3);
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary[2] = 5;
            obDictionary[4] = 1;
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.Clear();
            Assert.IsTrue(obDictionary.SequenceEqual(list));
            obDictionary.BindingDictionaryX(list, true);
            obDictionary.Add(1, 1);
            Assert.IsTrue(obDictionary.SequenceEqual(list) is false);
        }
        finally
        {
            obDictionary.BindingDictionaryX(list, true);
        }
    }
}
