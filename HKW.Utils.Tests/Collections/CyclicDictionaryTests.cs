using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
public class CyclicDictionaryTests
{
    [TestMethod]
    public void Cyclic()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
        cyclicDictionary.MoveNext();
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.GetPair(1)));
    }

    [TestMethod]
    public void Cyclic_AutoReset()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        )
        {
            AutoReset = true
        };
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
        for (var i = 1; i < 10; i++)
        {
            cyclicDictionary.MoveNext();
            Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.GetPair(i)));
        }
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.Last()));
        cyclicDictionary.MoveNext();
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
    }

    [TestMethod]
    public void Add()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>();
        Assert.IsTrue(cyclicDictionary.Count == 0);
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(default));
        cyclicDictionary.Add(1, 1);
        Assert.IsTrue(cyclicDictionary.Count == 1);
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
    }

    [TestMethod]
    public void Remove()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
        cyclicDictionary.Remove(0);
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
    }

    [TestMethod]
    public void Clear()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
        cyclicDictionary.Clear();
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(default));
    }

    [TestMethod]
    public void ValueChange()
    {
        var cyclicDictionary = new CyclicDictionary<int, int>(
            Enumerable.Range(0, 10).ToDictionary(i => i, i => i)
        );
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
        cyclicDictionary[0] = 10;
        Assert.IsTrue(cyclicDictionary.Current.EqualsContent(cyclicDictionary.First()));
    }
}
