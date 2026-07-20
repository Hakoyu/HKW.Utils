using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class FilterDictionaryTests
{
    static readonly Func<
        FilteredDictionaryWrapper<int, string, Dictionary<int, string>, Dictionary<int, string>>
    > _createDictionary = () =>
        new(
            new(Enumerable.Range(5, 10).Select(i => KeyValuePair.Create(i, i.ToString()))),
            filteredDictionary: new(),
            Filter
        );

    static readonly Func<
        FilteredDictionaryWrapper<int, string, Dictionary<int, string>, Dictionary<int, string>>
    > _createEmptyDictionary = () => new(new(), filteredDictionary: new(), Filter);

    static IReadOnlyCollection<KeyValuePair<int, string>> _newItems = Enumerable
        .Range(0, 5)
        .Concat(Enumerable.Range(100, 5))
        .Select(i => KeyValuePair.Create(i, i.ToString()))
        .ToArray();

    static bool Filter(KeyValuePair<int, string> kv) => kv.Key > 10 && int.Parse(kv.Value) > 10;

    [TestMethod]
    public void IDictionaryTest()
    {
        IDictionaryTTestUtils.Test(_createDictionary, _newItems);
    }

    [TestMethod]
    public void Add()
    {
        var dictionary = _createEmptyDictionary();
        Assert.AreEqual(0, [dictionary.Count, dictionary.FilteredDictionary.Count]);

        foreach (var item in _newItems)
        {
            dictionary.Add(item.Key, item.Value);
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }
    }

    [TestMethod]
    public void TryAdd()
    {
        var dictionary = _createEmptyDictionary();
        Assert.AreEqual(0, [dictionary.Count, dictionary.FilteredDictionary.Count]);

        foreach (var item in _newItems)
        {
            Assert.IsTrue(dictionary.TryAdd(item.Key, item.Value));
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }

        foreach (var item in _newItems)
        {
            Assert.IsFalse(dictionary.TryAdd(item.Key, item.Value));
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }
    }

    [TestMethod]
    public void Remove()
    {
        var dictionary = _createDictionary();
        var removeItems = dictionary.ToArray();

        foreach (var item in removeItems)
        {
            Assert.IsTrue(dictionary.Remove(item.Key));
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }

        foreach (var item in removeItems)
        {
            Assert.IsFalse(dictionary.Remove(item.Key));
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }
    }

    [TestMethod]
    public void ItemSet()
    {
        var dictionary = _createDictionary();
        var newItems = dictionary.Keys.Zip(_newItems.Select(x => x.Value)).ToArray();

        foreach (var (key, value) in newItems)
        {
            dictionary[key] = value;
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }

        foreach (var item in _newItems)
        {
            dictionary[item.Key] = item.Value;
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));
        }
    }

    [TestMethod]
    public void Clear()
    {
        var dictionary = _createDictionary();
        dictionary.Clear();
        Assert.AreEqual(0, [dictionary.Count, dictionary.FilteredDictionary.Count]);

        foreach (var item in _newItems)
        {
            dictionary.Add(item.Key, item.Value);
        }
        Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(dictionary.Where(Filter)));

        dictionary.Clear();
        Assert.AreEqual(0, [dictionary.Count, dictionary.FilteredDictionary.Count]);
    }

    [TestMethod]
    public void AutoFilter()
    {
        var dictionary = _createDictionary();
        dictionary.AutoFilter = false;
        var oldFilteredDictionary = dictionary.FilteredDictionary.ToDictionary();
        Assert.IsGreaterThan(0, oldFilteredDictionary.Count);

        foreach (var item in _newItems)
        {
            dictionary.Add(item.Key, item.Value);
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(oldFilteredDictionary));
        }

        foreach (var item in _newItems)
        {
            dictionary.Remove(item.Key);
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(oldFilteredDictionary));
        }

        var newItems = dictionary
            .Select(kv => KeyValuePair.Create(kv.Key, (kv.Value + 1).ToString()))
            .ToArray();

        foreach (var item in newItems)
        {
            dictionary[item.Key] = item.Value;
            Assert.IsTrue(dictionary.FilteredDictionary.SequenceEqual(oldFilteredDictionary));
        }

        // Clear 不受 AutoFilter 影响
        dictionary.Clear();
        Assert.AreEqual(0, [dictionary.Count, dictionary.FilteredDictionary.Count]);
    }
}
