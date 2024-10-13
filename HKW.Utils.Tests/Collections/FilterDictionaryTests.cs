using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Tests.Collections;

public class FilterDictionaryTests
{
    bool Filter(KeyValuePair<int, int> kv) => kv.Key > 5 && kv.Value > 5;

    [TestMethod]
    public void Add()
    {
        var filterDictionary = new FilterDictionaryWrapper<
            int,
            int,
            Dictionary<int, int>,
            Dictionary<int, int>
        >(new(), filteredDictionary: new(), Filter);
        Assert.IsTrue(filterDictionary.Count == filterDictionary.FilteredDictionary.Count);
        filterDictionary.Add(5, 5);
        Assert.IsTrue(0 == filterDictionary.FilteredDictionary.Count);
        filterDictionary.Add(6, 6);
        Assert.IsTrue(1 == filterDictionary.FilteredDictionary.Count);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
    }

    [TestMethod]
    public void Remove()
    {
        var filterDictionary = new FilterDictionaryWrapper<
            int,
            int,
            Dictionary<int, int>,
            Dictionary<int, int>
        >(Enumerable.Range(0, 10).ToDictionary(i => i, i => i), filteredDictionary: new(), Filter);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
        var oldCount = filterDictionary.FilteredDictionary.Count;
        filterDictionary.Remove(5);
        Assert.IsTrue(oldCount == filterDictionary.FilteredDictionary.Count);
        filterDictionary.Remove(6);
        Assert.IsTrue(oldCount - 1 == filterDictionary.FilteredDictionary.Count);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
    }

    [TestMethod]
    public void ValueChange()
    {
        var filterDictionary = new FilterDictionaryWrapper<
            int,
            int,
            Dictionary<int, int>,
            Dictionary<int, int>
        >(Enumerable.Range(0, 10).ToDictionary(i => i, i => i), filteredDictionary: new(), Filter);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
        var oldCount = filterDictionary.FilteredDictionary.Count;
        filterDictionary[10] = 5;
        Assert.IsTrue(oldCount == filterDictionary.FilteredDictionary.Count);
        filterDictionary[10] = 6;
        Assert.IsTrue(oldCount + 1 == filterDictionary.FilteredDictionary.Count);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
    }

    [TestMethod]
    public void Clear()
    {
        var filterDictionary = new FilterDictionaryWrapper<
            int,
            int,
            Dictionary<int, int>,
            Dictionary<int, int>
        >(Enumerable.Range(0, 10).ToDictionary(i => i, i => i), filteredDictionary: new(), Filter);
        Assert.IsTrue(
            filterDictionary.Where(Filter).Count() == filterDictionary.FilteredDictionary.Count
        );
        filterDictionary.Clear();
        Assert.IsTrue(filterDictionary.Count == filterDictionary.FilteredDictionary.Count);
        Assert.IsTrue(filterDictionary.Count == 0);
    }
}
