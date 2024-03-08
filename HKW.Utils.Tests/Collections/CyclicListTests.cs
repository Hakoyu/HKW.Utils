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
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
        cyclicList.MoveNext();
        Assert.IsTrue(cyclicList.Current == cyclicList[1]);
    }

    [TestMethod]
    public void Cyclic_AutoReset()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10)) { AutoReset = true };
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
        for (var i = 1; i < 10; i++)
        {
            cyclicList.MoveNext();
            Assert.IsTrue(cyclicList.Current == cyclicList[i]);
        }
        Assert.IsTrue(cyclicList.Current == cyclicList.Last());
        cyclicList.MoveNext();
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
    }

    [TestMethod]
    public void Add()
    {
        var cyclicList = new CyclicList<int>();
        Assert.IsTrue(cyclicList.Count == 0);
        Assert.IsTrue(cyclicList.Current == default);
        cyclicList.Add(1);
        Assert.IsTrue(cyclicList.Count == 1);
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
    }

    [TestMethod]
    public void Remove()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
        cyclicList.RemoveAt(0);
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
    }

    [TestMethod]
    public void Clear()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
        cyclicList.Clear();
        Assert.IsTrue(cyclicList.Current == default);
    }

    [TestMethod]
    public void ValueChange()
    {
        var cyclicList = new CyclicList<int>(Enumerable.Range(0, 10));
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
        cyclicList[0] = 10;
        Assert.IsTrue(cyclicList.Current == cyclicList.First());
    }
}
