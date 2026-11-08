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
public class IObservableCollectionTests
{
    [TestMethod]
    public void BindingListX()
    {
        var o = new ObservableList<int>();
        var t = new List<int>();
        try
        {
            ObservableCollectionExtensions.BindingListX(o, t);
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

            o.Insert(1, 99);
            CollectionAssert.AreEqual(new[] { 1, 99, 2, 3, 4, 5 }, t.ToArray());
            o.RemoveAt(1);
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
        finally
        {
            ObservableCollectionExtensions.BindingListX(o, t, true);
        }
    }

    [TestMethod]
    public void BindingList()
    {
        var o = new ObservableList<int>();
        var t = new List<int>();
        try
        {
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
        finally
        {
            o.BindingList(t, true);
        }
    }

    [TestMethod]
    public void BindingDictionaryX()
    {
        var o = new ObservableDictionary<int, int>();
        var t = new Dictionary<int, int>();
        try
        {
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
        finally
        {
            o.BindingDictionaryX(t, true);
        }
    }

    [TestMethod]
    public void BindingDictionary()
    {
        var o = new ObservableDictionary<int, int>();
        var t = new Dictionary<int, int>();
        try
        {
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
        finally
        {
            o.BindingDictionary(t, true);
        }
    }

    [TestMethod]
    public void BindingSetX()
    {
        var o = new ObservableSet<int>();
        var t = new HashSet<int>();
        try
        {
            o.BindingSetX(t);
            o.Add(1);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(2);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(3);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(4);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(5);
            Assert.IsTrue(t.SetEquals(o));

            o.Remove(2);
            Assert.IsTrue(t.SetEquals(o));
            o.Remove(3);
            Assert.IsTrue(t.SetEquals(o));
            o.Remove(1);
            Assert.IsTrue(t.SetEquals(o));

            o.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(t.SetEquals(o));
            o.ExceptWith(Enumerable.Range(0, 3));
            Assert.IsTrue(t.SetEquals(o));
            o.SymmetricExceptWith(Enumerable.Range(0, 5));
            Assert.IsTrue(t.SetEquals(o));
            o.IntersectWith(Enumerable.Range(3, 10));
            Assert.IsTrue(t.SetEquals(o));

            o.Clear();
            Assert.IsTrue(t.SetEquals(o));
        }
        finally
        {
            o.BindingSetX(t, true);
        }
    }

    [TestMethod]
    public void BindingSet()
    {
        var o = new ObservableSet<int>();
        var t = new HashSet<int>();
        try
        {
            o.BindingSet(t);
            o.Add(1);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(2);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(3);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(4);
            Assert.IsTrue(t.SetEquals(o));
            o.Add(5);
            Assert.IsTrue(t.SetEquals(o));

            o.Remove(2);
            Assert.IsTrue(t.SetEquals(o));
            o.Remove(3);
            Assert.IsTrue(t.SetEquals(o));
            o.Remove(1);
            Assert.IsTrue(t.SetEquals(o));

            o.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(t.SetEquals(o));
            o.ExceptWith(Enumerable.Range(0, 3));
            Assert.IsTrue(t.SetEquals(o));
            o.SymmetricExceptWith(Enumerable.Range(0, 5));
            Assert.IsTrue(t.SetEquals(o));
            o.IntersectWith(Enumerable.Range(3, 10));
            Assert.IsTrue(t.SetEquals(o));

            o.Clear();
            Assert.IsTrue(t.SetEquals(o));
        }
        finally
        {
            o.BindingSet(t, true);
        }
    }
}
