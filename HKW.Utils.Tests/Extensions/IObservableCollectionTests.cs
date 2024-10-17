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
public class IObservableCollectionTests
{
    [TestMethod]
    public void BindingListX()
    {
        var o = new ObservableList<int>();
        var t = new List<int>();
        o.BindingListX(t);
        o.Add(1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o[0] = 9;
        Assert.IsTrue(o.SequenceEqual(t));
        o[1] = 8;
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }

    [TestMethod]
    public void BindingList()
    {
        var o = new ObservableList<int>();
        var t = new List<int>();
        o.BindingList(t);
        o.Add(1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o[0] = 9;
        Assert.IsTrue(o.SequenceEqual(t));
        o[1] = 8;
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }

    [TestMethod]
    public void BindingDictionaryX()
    {
        var o = new ObservableDictionary<int, int>();
        var t = new Dictionary<int, int>();
        o.BindingDictionaryX(t);
        o.Add(1, 1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2, 2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3, 3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4, 4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5, 5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o[4] = 9;
        Assert.IsTrue(o.SequenceEqual(t));
        o[5] = 8;
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }

    [TestMethod]
    public void BindingDictionary()
    {
        var o = new ObservableDictionary<int, int>();
        var t = new Dictionary<int, int>();
        o.BindingDictionary(t);
        o.Add(1, 1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2, 2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3, 3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4, 4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5, 5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o[4] = 9;
        Assert.IsTrue(o.SequenceEqual(t));
        o[5] = 8;
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }

    [TestMethod]
    public void BindingSetX()
    {
        var o = new ObservableSet<int>();
        var t = new OrderedSet<int>();
        o.BindingSetX(t);
        o.Add(1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o.UnionWith(Enumerable.Range(0, 10));
        Assert.IsTrue(o.SequenceEqual(t));
        o.ExceptWith(Enumerable.Range(0, 3));
        Assert.IsTrue(o.SequenceEqual(t));
        o.SymmetricExceptWith(Enumerable.Range(0, 5));
        Assert.IsTrue(o.SequenceEqual(t));
        o.IntersectWith(Enumerable.Range(3, 10));
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }

    [TestMethod]
    public void BindingSet()
    {
        var o = new ObservableSet<int>();
        var t = new OrderedSet<int>();
        o.BindingSet(t);
        o.Add(1);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(4);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Add(5);
        Assert.IsTrue(o.SequenceEqual(t));

        o.Remove(2);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(3);
        Assert.IsTrue(o.SequenceEqual(t));
        o.Remove(1);
        Assert.IsTrue(o.SequenceEqual(t));

        o.UnionWith(Enumerable.Range(0, 10));
        Assert.IsTrue(o.SequenceEqual(t));
        o.ExceptWith(Enumerable.Range(0, 3));
        Assert.IsTrue(o.SequenceEqual(t));
        o.SymmetricExceptWith(Enumerable.Range(0, 5));
        Assert.IsTrue(o.SequenceEqual(t));
        o.IntersectWith(Enumerable.Range(3, 10));
        Assert.IsTrue(o.SequenceEqual(t));

        o.Clear();
        Assert.IsTrue(o.SequenceEqual(t));
    }
}
