using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class FilterListTests
{
    static readonly Func<FilteredListWrapper<string, List<string>, List<string>>> _createList =
        () => new(new(Enumerable.Range(5, 10).Select(i => i.ToString())), new(), Filter);

    static readonly Func<FilteredListWrapper<string, List<string>, List<string>>> _createEmptyList =
        () => new(new(), filteredList: new(), Filter);

    static IReadOnlyCollection<string> _newItems = Enumerable
        .Range(0, 5)
        .Concat(Enumerable.Range(100, 5))
        .Select(i => i.ToString())
        .ToArray();

    static bool Filter(string x) => int.Parse(x) > 10;

    [TestMethod]
    public void IListTest()
    {
        IListTTestUtils.Test(_createList, _newItems);
    }

    [TestMethod]
    public void Add()
    {
        var list = _createEmptyList();
        Assert.AreEqual(0, [list.Count, list.FilteredList.Count]);

        foreach (var item in _newItems)
        {
            list.Add(item);
            Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));
        }
    }

    [TestMethod]
    public void Insert()
    {
        var list = _createList();

        foreach (var (e, i) in _newItems.WithIndex())
        {
            list.Insert(i, e);
            Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));
        }
    }

    [TestMethod]
    public void Remove()
    {
        var list = _createList();
        var removeItems = list.ToArray();

        foreach (var item in removeItems)
        {
            Assert.IsTrue(list.Remove(item));
            Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));
        }

        Assert.AreEqual(0, [list.Count, list.FilteredList.Count]);
        foreach (var item in removeItems)
        {
            Assert.IsFalse(list.Remove(item));
            Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));
        }
    }

    [TestMethod]
    public void ItemSet()
    {
        var list = _createList();

        foreach (var (e, i) in _newItems.WithIndex())
        {
            list[i] = e;
            Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));
        }
    }

    [TestMethod]
    public void Clear()
    {
        var list = _createList();
        list.Clear();
        Assert.AreEqual(0, [list.Count, list.FilteredList.Count]);

        foreach (var item in _newItems)
        {
            list.Add(item);
        }
        Assert.IsTrue(list.FilteredList.SequenceEqual(list.Where(Filter)));

        list.Clear();
        Assert.AreEqual(0, [list.Count, list.FilteredList.Count]);
    }

    [TestMethod]
    public void AutoFilter()
    {
        var list = _createList();
        list.AutoFilter = false;
        var oldFilteredList = list.FilteredList.ToList();
        Assert.IsGreaterThan(0, oldFilteredList.Count);

        foreach (var item in _newItems)
        {
            list.Add(item);
            Assert.IsTrue(list.FilteredList.SequenceEqual(oldFilteredList));
        }

        foreach (var item in _newItems)
        {
            list.Remove(item);
            Assert.IsTrue(list.FilteredList.SequenceEqual(oldFilteredList));
        }

        foreach (var (e, i) in _newItems.WithIndex())
        {
            list[i] = e;
            Assert.IsTrue(list.FilteredList.SequenceEqual(oldFilteredList));
        }

        // Clear 不受 AutoFilter 影响
        list.Clear();
        Assert.AreEqual(0, [list.Count, list.FilteredList.Count]);
    }
}
