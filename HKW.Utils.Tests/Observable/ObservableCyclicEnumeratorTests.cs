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
        TrackNotifications(cyclicEnumerator);
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
        TrackNotifications(cyclicEnumerator);
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
        TrackNotifications(cyclicEnumerator);
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

    private static void TrackNotifications<T>(ObservableCyclicEnumerator<T> enumerator)
    {
        var previousIndex = -1;
        enumerator.PropertyChanged += (sender, args) =>
        {
            Assert.AreSame(enumerator, sender);
            if (args.PropertyName == nameof(enumerator.Current))
            {
                Assert.IsGreaterThanOrEqualTo(0, enumerator.CurrentIndex);
                _ = enumerator.Current;
            }
            else
            {
                Assert.AreEqual(nameof(enumerator.CurrentIndex), args.PropertyName);
                Assert.AreNotEqual(previousIndex, enumerator.CurrentIndex);
                previousIndex = enumerator.CurrentIndex;
            }
        };
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Reset_NotifiesOnlyRealIndexChangesWithInvalidCurrent(bool autoReset)
    {
        using var enumerator = ObservableCyclicEnumerator.Create(
            HKW.HKWUtilsTests.Utils.CyclicEnumeratorContractTests.YieldValues(3),
            autoReset
        );
        var changes = new List<(string? Name, int OldIndex, int NewIndex)>();
        var previousIndex = -1;
        enumerator.PropertyChanged += (sender, args) =>
        {
            Assert.AreSame(enumerator, sender);
            if (args.PropertyName == nameof(enumerator.Current))
                Assert.AreEqual(enumerator.CurrentIndex + 1, enumerator.Current);
            else
            {
                Assert.AreEqual(nameof(enumerator.CurrentIndex), args.PropertyName);
                Assert.AreNotEqual(previousIndex, enumerator.CurrentIndex);
                if (enumerator.CurrentIndex == -1)
                    Assert.ThrowsExactly<InvalidOperationException>(() => _ = enumerator.Current);
                changes.Add((args.PropertyName, previousIndex, enumerator.CurrentIndex));
                previousIndex = enumerator.CurrentIndex;
            }
        };
        enumerator.Reset();
        Assert.IsEmpty(changes);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.IsTrue(enumerator.MoveNext());
        enumerator.Reset();
        enumerator.Reset();
        Assert.AreSequenceEqual(
            new (string?, int, int)[]
            {
                ("CurrentIndex", -1, 0),
                ("CurrentIndex", 0, 1),
                ("CurrentIndex", 1, -1),
            },
            changes
        );
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    public void AutoReset_NotifiesOnlyFinalIndexAndValidCurrent(int count)
    {
        using var enumerator = ObservableCyclicEnumerator.Create(
            HKW.HKWUtilsTests.Utils.CyclicEnumeratorContractTests.YieldValues(count),
            true
        );
        var names = new List<string?>();
        enumerator.PropertyChanged += (sender, args) =>
        {
            Assert.AreSame(enumerator, sender);
            names.Add(args.PropertyName);
            Assert.AreEqual(enumerator.CurrentIndex + 1, enumerator.Current);
        };
        for (var step = 0; step < count * 3 + 2; step++)
        {
            names.Clear();
            var previous = enumerator.CurrentIndex;
            Assert.AreEqual(count > 0, enumerator.MoveNext());
            string?[] expected =
                count == 0 ? []
                : previous == enumerator.CurrentIndex ? ["Current"]
                : ["Current", "CurrentIndex"];
            Assert.AreSequenceEqual(expected, names);
        }
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void FailedReset_NotifiesInvalidatedIndexWithoutCurrent(bool recreate, bool autoReset)
    {
        var failure = new InvalidOperationException("Reset failed.");
        var source = new HKW.HKWUtilsTests.Utils.CyclicEnumeratorContractTests.ProbeSource
        {
            ResetFailure = recreate ? new NotSupportedException() : failure,
            CreateFailure = recreate ? failure : null,
        };
        using var enumerator = ObservableCyclicEnumerator.Create(source, autoReset);
        Assert.IsTrue(enumerator.MoveNext());
        var names = new List<string?>();
        enumerator.PropertyChanged += (_, args) =>
        {
            names.Add(args.PropertyName);
            Assert.AreEqual(-1, enumerator.CurrentIndex);
            Assert.ThrowsExactly<InvalidOperationException>(() => _ = enumerator.Current);
        };
        var actual = autoReset
            ? Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext())
            : Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset());
        Assert.AreSame(failure, actual);
        Assert.AreSequenceEqual(new[] { "CurrentIndex" }, names);
    }
}
#pragma warning restore S108
