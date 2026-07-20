using System;
using System.Collections.Generic;
using System.Text;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Observable;

#pragma warning disable S108
[TestClass]
public class ObservableCyclicEnumeratorTests
{
    [TestMethod]
    public void List()
    {
        var list = Enumerable.Range(1, 10).ToList();
        var cyclicEnumerator = ObservableCyclicEnumerator.Create(list);
        var firstProperty = true;
        cyclicEnumerator.PropertyChanged += (s, e) =>
        {
            Assert.AreEqual(cyclicEnumerator, s);
            if (firstProperty)
            {
                Assert.AreEqual(nameof(ObservableCyclicEnumerator<int>.Current), e.PropertyName);
                firstProperty = false;
            }
            else
            {
                Assert.AreEqual(
                    nameof(ObservableCyclicEnumerator<int>.CurrentIndex),
                    e.PropertyName
                );
                firstProperty = true;
            }
        };
        for (var i = 0; i < list.Count; i++)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(i, cyclicEnumerator.CurrentIndex);
            Assert.AreEqual(list[i], cyclicEnumerator.Current);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.Reset();

        for (var i = 0; i < list.Count; i++)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(list[i], cyclicEnumerator.Current);
            Assert.AreEqual(i, cyclicEnumerator.CurrentIndex);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.AutoReset = true;
        for (var i = 0; i < list.Count; i++)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(list[i], cyclicEnumerator.Current);
            Assert.AreEqual(i, cyclicEnumerator.CurrentIndex);
        }
        for (var i = 0; i < list.Count; i++)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(list[i], cyclicEnumerator.Current);
            Assert.AreEqual(i, cyclicEnumerator.CurrentIndex);
        }

        list.Add(0);
        Assert.Throws<InvalidOperationException>(() =>
        {
            cyclicEnumerator.MoveNext();
        });
    }

    [TestMethod]
    public void Dictionary()
    {
        var dictionary = Enumerable.Range(1, 10).ToDictionary(i => i, i => i);
        var cyclicEnumerator = ObservableCyclicEnumerator.Create(dictionary);
        var firstProperty = true;
        cyclicEnumerator.PropertyChanged += (s, e) =>
        {
            Assert.AreEqual(cyclicEnumerator, s);
            if (firstProperty)
            {
                Assert.AreEqual(nameof(ObservableCyclicEnumerator<int>.Current), e.PropertyName);
                firstProperty = false;
            }
            else
            {
                Assert.AreEqual(
                    nameof(ObservableCyclicEnumerator<int>.CurrentIndex),
                    e.PropertyName
                );
                firstProperty = true;
            }
        };
        foreach (var p in dictionary)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(p, cyclicEnumerator.Current);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.Reset();

        foreach (var p in dictionary)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(p, cyclicEnumerator.Current);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.AutoReset = true;
        foreach (var p in dictionary)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(p, cyclicEnumerator.Current);
        }
        foreach (var p in dictionary)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(p, cyclicEnumerator.Current);
        }

        dictionary.Add(0, 0);
        Assert.Throws<InvalidOperationException>(() =>
        {
            cyclicEnumerator.MoveNext();
        });
    }

    [TestMethod]
    public void HashSet()
    {
        var set = Enumerable.Range(1, 10).ToHashSet();
        var cyclicEnumerator = ObservableCyclicEnumerator.Create(set);
        var firstProperty = true;
        cyclicEnumerator.PropertyChanged += (s, e) =>
        {
            Assert.AreEqual(cyclicEnumerator, s);
            if (firstProperty)
            {
                Assert.AreEqual(nameof(ObservableCyclicEnumerator<int>.Current), e.PropertyName);
                firstProperty = false;
            }
            else
            {
                Assert.AreEqual(
                    nameof(ObservableCyclicEnumerator<int>.CurrentIndex),
                    e.PropertyName
                );
                firstProperty = true;
            }
        };
        foreach (var i in set)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(i, cyclicEnumerator.Current);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.Reset();

        foreach (var i in set)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(i, cyclicEnumerator.Current);
        }
        Assert.IsFalse(cyclicEnumerator.MoveNext());

        cyclicEnumerator.AutoReset = true;
        foreach (var i in set)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(i, cyclicEnumerator.Current);
        }
        foreach (var i in set)
        {
            cyclicEnumerator.MoveNext();
            Assert.AreEqual(i, cyclicEnumerator.Current);
        }

        set.Add(0);
        Assert.Throws<InvalidOperationException>(() =>
        {
            cyclicEnumerator.MoveNext();
        });
    }
}
#pragma warning restore S108
