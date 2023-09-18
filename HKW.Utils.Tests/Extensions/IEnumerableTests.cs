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
        Assert.IsFalse(cl.SequenceEqual(ll));
        Assert.IsTrue(cl.SequenceEqual(ll, (x, y) => x.SequenceEqual(y)));
    }
}
