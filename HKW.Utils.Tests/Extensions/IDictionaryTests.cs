using HKW.HKWUtils.Extensions;
using System.Collections.ObjectModel;

namespace HKWTests.Extensions;

[TestClass]
public class IDictionaryTests
{
    [TestMethod]
    public void GetPair()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var pair = dic.GetPair(0);
        Assert.AreEqual(pair, new KeyValuePair<int, int>(0, 0));
    }

    [TestMethod]
    public void GetPai_Error()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        KeyValuePair<int, int>? pair = null;
        try
        {
            pair = dic.GetPair(-1);
            Assert.AreEqual(pair, new KeyValuePair<int, int>(0, 0));
        }
        catch
        {
            Assert.AreEqual(pair, null);
        }
    }

    [TestMethod]
    public void TryGetPai_True()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var result = dic.TryGetPair(0, out var pair);
        Assert.IsTrue(result);
        Assert.AreEqual(pair, new KeyValuePair<int, int>(0, 0));
    }

    [TestMethod]
    public void TryGetPai_False()
    {
        var dic = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
        var result = dic.TryGetPair(-1, out var pair);
        Assert.IsFalse(result);
        Assert.IsNull(pair);
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
            readOnlyDictionary.SequenceEqualOnComparer(
                readOnlyDictionaryOnWrapper,
                (x, y) =>
                {
                    return x.Value.SequenceEqual(y.Value);
                }
            )
        );
    }

    [TestMethod]
    public void AsReadOnlyOnWrappe_SourceDictionaryChange()
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
