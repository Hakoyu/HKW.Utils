using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
public class FilterListTests
{
    bool Filter(int i) => i > 5;

    [TestMethod]
    public void Add()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            new(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Count);
        filterList.AddRange(Enumerable.Range(0, 10));
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
    }

    [TestMethod]
    public void Insert()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            [],
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Insert(3, 3);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Insert(6, 10);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Insert(8, 5);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Insert(9, 11);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Insert(10, 15);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
    }

    [TestMethod]
    public void Remove()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        var oldCount = filterList.FilteredList.Count;
        filterList.Remove(5);
        Assert.AreEqual(filterList.FilteredList.Count, oldCount);
        filterList.Remove(6);
        Assert.AreEqual(filterList.FilteredList.Count, oldCount - 1);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
    }

    [TestMethod]
    public void RemoveAll()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.RemoveAll(Filter);
        Assert.AreEqual(0, filterList.FilteredList.Count);
    }

    [TestMethod]
    public void RemoveAt()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        var oldCount = filterList.FilteredList.Count;
        filterList.RemoveAt(0);
        Assert.AreEqual(filterList.FilteredList.Count, oldCount);
        filterList.RemoveAt(8);
        Assert.AreEqual(filterList.FilteredList.Count, oldCount - 1);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
    }

    [TestMethod]
    public void ValueChange()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        var oldCount = filterList.FilteredList.Count;
        filterList[0] = 5;
        Assert.AreEqual(filterList.FilteredList.Count, oldCount);
        filterList[0] = 10;
        Assert.AreEqual(filterList.FilteredList.Count, oldCount + 1);
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
    }

    [TestMethod]
    public void Clear()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Where(Filter).Count());
        filterList.Clear();
        Assert.AreEqual(filterList.FilteredList.Count, filterList.Count);
        Assert.AreEqual(0, filterList.Count);
    }
}
