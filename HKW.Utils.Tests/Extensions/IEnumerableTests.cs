using HKW.HKWUtils.Extensions;
using System.Collections.ObjectModel;

namespace HKWTests.Extensions;

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
    public void Enumerate()
    {
        var index = 0;
        foreach (var item in Enumerable.Range(0, 10).Enumerate())
        {
            Assert.IsTrue(index == item.Value);
            Assert.IsTrue(index == item.Index);
            index++;
        }
    }
}
