using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtils.Tests.Collections;

[TestClass]
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
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, filterDictionary.Count);
        filterDictionary.Add(5, 5);
        Assert.IsEmpty(filterDictionary.FilteredDictionary);
        filterDictionary.Add(6, 6);
        Assert.HasCount(1, filterDictionary.FilteredDictionary);
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
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
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
        var oldCount = filterDictionary.FilteredDictionary.Count;
        filterDictionary.Remove(5);
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, oldCount);
        filterDictionary.Remove(6);
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, oldCount - 1);
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
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
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
        var oldCount = filterDictionary.FilteredDictionary.Count;
        filterDictionary[10] = 5;
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, oldCount);
        filterDictionary[10] = 6;
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, oldCount + 1);
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
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
        Assert.HasCount(filterDictionary.FilteredDictionary.Count, filterDictionary.Where(Filter));
        filterDictionary.Clear();
        Assert.AreEqual(filterDictionary.FilteredDictionary.Count, filterDictionary.Count);
        Assert.AreEqual(0, filterDictionary.Count);
    }
}
