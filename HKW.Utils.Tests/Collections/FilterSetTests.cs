using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class FilterSetTests
{
    static readonly Func<FilteredSetWrapper<string, HashSet<string>, HashSet<string>>> _createSet =
        () => new(new(Enumerable.Range(5, 10).Select(i => i.ToString())), new(), Filter);

    static readonly Func<
        FilteredSetWrapper<string, HashSet<string>, HashSet<string>>
    > _createEmptySet = () => new(new(), filteredSet: new(), Filter);

    static IReadOnlyCollection<string> _newItems = Enumerable
        .Range(0, 5)
        .Concat(Enumerable.Range(100, 5))
        .Select(i => i.ToString())
        .ToArray();

    static bool Filter(string x) => int.Parse(x) > 10;

    [TestMethod]
    public void ISetTest()
    {
        ISetTTestUtils.Test(_createSet, _newItems);
    }

    [TestMethod]
    public void Add()
    {
        var set = _createEmptySet();
        Assert.AreEqual(0, [set.Count, set.FilteredSet.Count]);

        foreach (var item in _newItems)
        {
            set.Add(item);
            Assert.IsTrue(set.FilteredSet.SequenceEqual(set.Where(Filter)));
        }
    }

    [TestMethod]
    public void Remove()
    {
        var set = _createSet();
        var removeItems = set.ToArray();

        foreach (var item in removeItems)
        {
            Assert.IsTrue(set.Remove(item));
            Assert.IsTrue(set.FilteredSet.SequenceEqual(set.Where(Filter)));
        }

        Assert.AreEqual(0, [set.Count, set.FilteredSet.Count]);
        foreach (var item in removeItems)
        {
            Assert.IsFalse(set.Remove(item));
            Assert.IsTrue(set.FilteredSet.SequenceEqual(set.Where(Filter)));
        }
    }

    [TestMethod]
    public void Clear()
    {
        var set = _createSet();
        set.Clear();
        Assert.AreEqual(0, [set.Count, set.FilteredSet.Count]);

        foreach (var item in _newItems)
        {
            set.Add(item);
        }
        Assert.IsTrue(set.FilteredSet.SequenceEqual(set.Where(Filter)));

        set.Clear();
        Assert.AreEqual(0, [set.Count, set.FilteredSet.Count]);
    }

    [TestMethod]
    public void AutoFilter()
    {
        var set = _createSet();
        set.AutoFilter = false;
        var oldFilteredSet = set.FilteredSet.ToHashSet();
        Assert.IsGreaterThan(0, oldFilteredSet.Count);

        foreach (var item in _newItems)
        {
            set.Add(item);
            Assert.IsTrue(set.FilteredSet.SequenceEqual(oldFilteredSet));
        }

        foreach (var item in _newItems)
        {
            set.Remove(item);
            Assert.IsTrue(set.FilteredSet.SequenceEqual(oldFilteredSet));
        }

        // Clear 不受 AutoFilter 影响
        set.Clear();
        Assert.AreEqual(0, [set.Count, set.FilteredSet.Count]);
    }
}
