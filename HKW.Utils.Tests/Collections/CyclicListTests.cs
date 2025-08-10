using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
public class CyclicListTests
{
    [TestMethod]
    public void Cyclic()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
        cyclicList.MoveNext();
        Assert.AreEqual(cyclicList[1], cyclicList.Current);
    }

    [TestMethod]
    public void Cyclic_AutoReset()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10)) { AutoReset = true };
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
        for (var i = 1; i < 10; i++)
        {
            cyclicList.MoveNext();
            Assert.AreEqual(cyclicList[i], cyclicList.Current);
        }
        Assert.AreEqual(cyclicList.Last(), cyclicList.Current);
        cyclicList.MoveNext();
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
    }

    [TestMethod]
    public void Add()
    {
        var cyclicList = new CyclicList<int>();
        Assert.AreEqual(0, cyclicList.Count);
        Assert.AreEqual(default, cyclicList.Current);
        cyclicList.Add(1);
        Assert.AreEqual(1, cyclicList.Count);
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
    }

    [TestMethod]
    public void Remove()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
        ((System.Collections.IList)cyclicList).RemoveAt(0);
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
    }

    [TestMethod]
    public void Clear()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
        ((System.Collections.IList)cyclicList).Clear();
        Assert.AreEqual(default, cyclicList.Current);
    }

    [TestMethod]
    public void ValueChange()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
        cyclicList[0] = 10;
        Assert.AreEqual(cyclicList.First(), cyclicList.Current);
    }
}
