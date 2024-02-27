using System.Collections.ObjectModel;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class IEnumerableTests
{
    [TestMethod]
    public void SequenceEqual()
    {
        List<List<int>> ll = Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList();
        Collection<List<int>> cl =
            new(Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList());
        Assert.IsTrue(cl.SequenceEqual(ll) is false);
        Assert.IsTrue(cl.SequenceEqual(ll, (x, y) => x.SequenceEqual(y)));
    }

    [TestMethod]
    public void EnumerateIndex()
    {
        var index = 0;
        var list = Enumerable.Range(0, 10).ToList();
        foreach (var (i, item) in list.EnumerateIndex())
        {
            Assert.IsTrue(index == i);
            Assert.IsTrue(list[i] == item);
            index++;
        }
    }

    [TestMethod]
    public void Random_1()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var randomItem = set.Random();
        Assert.IsTrue(set.Contains(randomItem));
    }

    [TestMethod]
    public void Random_2()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var random = new Random(set.GetHashCode());
        var randomItem = set.Random(random);
        Assert.IsTrue(set.Contains(randomItem));
    }

    [TestMethod]
    public void RandomOrder_1()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var randomOrder = set.RandomOrder();
        Assert.IsTrue(set.ItemsEqual(randomOrder));
    }

    [TestMethod]
    public void RandomOrder_2()
    {
        var set = Enumerable.Range(0, 10).ToHashSet();
        var random = new Random(set.GetHashCode());
        var randomOrder = set.RandomOrder();
        Assert.IsTrue(set.ItemsEqual(randomOrder));
    }
}
