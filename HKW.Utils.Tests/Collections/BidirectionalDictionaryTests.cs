using System;
using System.Collections.Generic;
using System.Text;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class BidirectionalDictionaryTests
{
    readonly Func<BidirectionalDictionary<int, string>> _createDictionary = () =>
        new BidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).Select(i => (i, i.ToString()))
        );
    readonly IReadOnlyCollection<KeyValuePair<int, string>> _newItems = Enumerable
        .Range(100, 10)
        .Select(i => KeyValuePair.Create(i, i.ToString()))
        .ToArray();

    [TestMethod]
    public void IDictionaryTest()
    {
        IReadOnlyDictionaryTTestUtils.ItemGet(_createDictionary, _newItems);
        IDictionaryTTestUtils.Remove(_createDictionary);
        IDictionaryTTestUtils.Clear(_createDictionary);
        IReadOnlyDictionaryTTestUtils.Keys(_createDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.Values(_createDictionary, _newItems);
        IReadOnlyDictionaryTTestUtils.TryGetValue(_createDictionary, _newItems);
    }

    [TestMethod]
    public void Test()
    {
        var getDictionary = () =>
            new BidirectionalDictionary<int, string>(
                Enumerable.Range(1, 10).Select(i => (i, i.ToString()))
            );

        IDictionaryTTestUtils.Remove<int, string>(getDictionary);
        IDictionaryTTestUtils.Clear<int, string>(getDictionary);
    }

    [TestMethod]
    public void TrySetValue()
    {
        var dictionary = new BidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).Select(i => (i, i.ToString()))
        );
        var cDictionary = dictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        try
        {
            dictionary[1] = "1";
            Assert.Fail();
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType<UseAlternativeMethodException>(ex);
        }
        Assert.IsTrue(dictionary.TrySetValue(1, "100"));
        cDictionary[1] = "100";
        Assert.AreEqual(dictionary[1], cDictionary[1]);
        Assert.IsFalse(dictionary.TrySetValue(100, "100"));
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
    }

    [TestMethod]
    public void TryAdd()
    {
        var dictionary = new BidirectionalDictionary<int, string>(
            Enumerable.Range(1, 10).Select(i => (i, i.ToString()))
        );
        var cDictionary = dictionary.ToDictionary(kv => kv.Key, kv => kv.Value);

        try
        {
            ((IDictionary<int, string>)dictionary).Add(100, "100");
            Assert.Fail();
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType<UseAlternativeMethodException>(ex);
        }
        Assert.AreEqual(dictionary.TryAdd(100, "100"), cDictionary.TryAdd(100, "100"));
        Assert.AreEqual(dictionary.TryAdd(100, "100"), cDictionary.TryAdd(100, "100"));
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
    }
}
