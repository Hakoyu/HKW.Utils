using System.Collections.ObjectModel;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class IEnumerableTests
{
    [TestMethod]
    public void SequenceEqual()
    {
        List<List<int>> ll = Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList();
        Collection<List<int>> cl = new(
            Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList()
        );
        Assert.IsFalse(cl.SequenceEqual(ll));
        Assert.IsTrue(cl.SequenceEqual(ll, (x, y) => x.SequenceEqual(y)));
    }

    [TestMethod]
    public void EnumerateIndex()
    {
        var index = 0;
        var list = Enumerable.Range(0, 10).ToList();
        foreach (var (e, i) in list.WithIndex())
        {
            Assert.AreEqual(i, index);
            Assert.AreEqual(e, list[i]);
            index++;
        }
    }

    [TestMethod]
    public void Random_1()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var randomItem = set.Random();
        Assert.Contains(randomItem, set);
    }

    [TestMethod]
    public void Random_2()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var random = new Random(set.GetHashCode());
        var randomItem = set.Random(random);
        Assert.Contains(randomItem, set);
    }

    [TestMethod]
    public void RandomOrder_1()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var randomOrder = set.RandomOrder();
        Assert.IsTrue(set.UnorderedEqual(randomOrder));
    }

    [TestMethod]
    public void RandomOrder_2()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var random = new Random(set.GetHashCode());
        var randomOrder = set.RandomOrder(random);
        Assert.IsTrue(set.UnorderedEqual(randomOrder));
    }
}
