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
        Assert.IsTrue(filterList.Count == filterList.FilteredList.Count);
        filterList.AddRange(Enumerable.Range(0, 10));
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
    }

    [TestMethod]
    public void Remove()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
        var oldCount = filterList.FilteredList.Count;
        filterList.Remove(5);
        Assert.IsTrue(oldCount == filterList.FilteredList.Count);
        filterList.Remove(6);
        Assert.IsTrue(oldCount - 1 == filterList.FilteredList.Count);
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
    }

    [TestMethod]
    public void RemoveAll()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
        filterList.RemoveAll(Filter);
        Assert.IsTrue(filterList.FilteredList.Count == 0);
    }

    [TestMethod]
    public void RemoveAt()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
        var oldCount = filterList.FilteredList.Count;
        filterList.RemoveAt(0);
        Assert.IsTrue(oldCount == filterList.FilteredList.Count);
        filterList.RemoveAt(8);
        Assert.IsTrue(oldCount - 1 == filterList.FilteredList.Count);
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
    }

    [TestMethod]
    public void ValueChange()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
        var oldCount = filterList.FilteredList.Count;
        filterList[0] = 5;
        Assert.IsTrue(oldCount == filterList.FilteredList.Count);
        filterList[0] = 10;
        Assert.IsTrue(oldCount + 1 == filterList.FilteredList.Count);
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
    }

    [TestMethod]
    public void Clear()
    {
        var filterList = new FilterListWrapper<int, List<int>, List<int>>(
            Enumerable.Range(0, 10).ToList(),
            filteredList: new(),
            Filter
        );
        Assert.IsTrue(filterList.Where(Filter).Count() == filterList.FilteredList.Count);
        filterList.Clear();
        Assert.IsTrue(filterList.Count == filterList.FilteredList.Count);
        Assert.IsTrue(filterList.Count == 0);
    }
}
