using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKWTests.ExtensionsTests;

[TestClass]
public class IEnumerableT
{
    [TestMethod]
    public void EqualOnComparer()
    {
        List<List<int>> ll = Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList();
        Collection<List<int>> cl =
            new(Enumerable.Range(0, 10).Select(i => new List<int>() { i }).ToList());
        Assert.IsFalse(cl.SequenceEqual(ll));
        Assert.IsTrue(cl.EqualOnComparer(ll, (x, y) => x.SequenceEqual(y)));
    }
}
