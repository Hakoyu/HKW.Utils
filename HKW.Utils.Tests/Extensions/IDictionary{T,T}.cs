using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class IDictionaryTT
{
    [TestMethod]
    public void GetEntry()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var entry = dic.GetEntry(0);
        Assert.AreEqual(entry, new KeyValuePair<int, int>(0, 0));
    }

    [TestMethod]
    public void GetEntry_Error()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        KeyValuePair<int, int>? entry = null;
        try
        {
            entry = dic.GetEntry(-1);
            Assert.AreEqual(entry, new KeyValuePair<int, int>(0, 0));
        }
        catch
        {
            Assert.AreEqual(entry, null);
        }
    }

    [TestMethod]
    public void TryGetEntry_True()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var result = dic.TryGetEntry(0, out var entry);
        Assert.IsTrue(result);
        Assert.AreEqual(entry, new KeyValuePair<int, int>(0, 0));
    }

    [TestMethod]
    public void TryGetEntry_False()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var result = dic.TryGetEntry(-1, out var entry);
        Assert.IsFalse(result);
        Assert.IsNull(entry);
    }

    [TestMethod]
    public void AsReadOnlyOnWrapper()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => new List<int> { x });
        var readOnlyDictionary = new ReadOnlyDictionary<int, IReadOnlyCollection<int>>(
            dic.ToDictionary(x => x.Key, x => (IReadOnlyCollection<int>)x.Value)
        );
        var readOnlyDictionaryOnWrapper = dic.AsReadOnlyOnWrapper<
            int,
            List<int>,
            IReadOnlyCollection<int>
        >();
        Assert.IsTrue(
            readOnlyDictionary.EqualOnComparer(
                readOnlyDictionaryOnWrapper,
                (x, y) =>
                {
                    return x.Value.SequenceEqual(y.Value);
                }
            )
        );
    }

    [TestMethod]
    public void AsReadOnlyOnWrapper_SourceDictionaryChange()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => new List<int> { x });
        var readOnlyDictionary = new ReadOnlyDictionary<int, IReadOnlyList<int>>(
            dic.ToDictionary(x => x.Key, x => (IReadOnlyList<int>)x.Value)
        );
        var readOnlyDictionaryOnWrapper = dic.AsReadOnlyOnWrapper<
            int,
            List<int>,
            IReadOnlyList<int>
        >();
        dic.Add(dic.Count, new() { dic.Count });
        Assert.IsTrue(readOnlyDictionary.Count < readOnlyDictionaryOnWrapper.Count);
        Assert.IsTrue(readOnlyDictionary.Last().Value is List<int>);
        Assert.IsTrue(readOnlyDictionaryOnWrapper.Last().Value is IReadOnlyList<int>);
    }
}
