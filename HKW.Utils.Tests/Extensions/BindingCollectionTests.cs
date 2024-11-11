using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class BindingCollectionTests
{
    [TestMethod]
    public void BindingList()
    {
        var obList = new ObservableList<int>();
        var list = new List<int>();
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

    [TestMethod]
    public void BindingListX()
    {
        var obList = new ObservableList<int>();
        var list = new List<int>();
        obList.BindingListX(list);
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
        obList.BindingListX(list, true);
        obList.Add(1);
        Assert.IsTrue(obList.SequenceEqual(list) is false);
    }

    [TestMethod]
    public void BindingSet()
    {
        var obSet = new ObservableSet<int>();
        var set = new OrderedSet<int>();
        obSet.BindingSet(set);
        obSet.Add(1);
        obSet.Add(2);
        obSet.Add(3);
        obSet.Add(4);
        obSet.Add(5);
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.Remove(1);
        obSet.Remove(3);
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.Clear();
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.BindingSet(set, true);
        obSet.Add(1);
        Assert.IsTrue(obSet.SequenceEqual(set) is false);
    }

    [TestMethod]
    public void BindingSetX()
    {
        var obSet = new ObservableSet<int>();
        var set = new OrderedSet<int>();
        obSet.BindingSetX(set);
        obSet.Add(1);
        obSet.Add(2);
        obSet.Add(3);
        obSet.Add(4);
        obSet.Add(5);
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.Remove(1);
        obSet.Remove(3);
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.Clear();
        Assert.IsTrue(obSet.SequenceEqual(set));
        obSet.BindingSetX(set, true);
        obSet.Add(1);
        Assert.IsTrue(obSet.SequenceEqual(set) is false);
    }

    [TestMethod]
    public void BindingDictionary()
    {
        var obDictionary = new ObservableDictionary<int, int>();
        var list = new Dictionary<int, int>();
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

    [TestMethod]
    public void BindingDictionaryX()
    {
        var obDictionary = new ObservableDictionary<int, int>();
        var list = new Dictionary<int, int>();
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
}
