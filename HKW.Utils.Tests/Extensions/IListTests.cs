using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class IListTests
{
    [TestMethod]
    public void Random()
    {
        IList<int> set = Enumerable.Range(1, 10).ToList();
        var randomItem = set.Random();
        Assert.Contains(randomItem, set);
    }

    #region IListFind
    [TestMethod]
    public void Find()
    {
        IList<int> list = Enumerable.Range(1, 10).ToList();

        Assert.AreEqual(1, list.Find(x => x == 1));
        Assert.AreEqual((3, 4), list.FindPair(2, x => x == 4));
        Assert.AreEqual((7, 8), list.FindPair(4, 4, x => x == 8));

        Assert.AreEqual(default, list.Find(x => x == -1));
        Assert.AreEqual((-1, default), list.FindPair(2, x => x == -1));
        Assert.AreEqual((-1, default), list.FindPair(4, 4, x => x == -1));
    }

    [TestMethod]
    public void TryFind()
    {
        IList<int> list = Enumerable.Range(1, 10).ToList();

        Assert.IsTrue(list.TryFind(x => x == 1, out var i1) && i1 == 1);
        Assert.IsTrue(list.TryFindPair(1, x => x == 2, out var i2) && i2 == (1, 2));
        Assert.IsTrue(list.TryFindPair(1, 3, x => x == 3, out var i3) && i3 == (2, 3));

        Assert.IsTrue(list.TryFind(x => x == -1, out var i4) is false && i4 == default);
        Assert.IsTrue(
            list.TryFindPair(1, x => x == -1, out var i5) is false && i5 == (-1, default)
        );
        Assert.IsTrue(
            list.TryFindPair(1, 3, x => x == -1, out var i6) is false && i6 == (-1, default)
        );
    }

    [TestMethod]
    public void FindLast()
    {
        IList<int> list = Enumerable.Range(1, 10).ToList();

        Assert.AreEqual(1, list.FindLast(x => x == 1));
        Assert.AreEqual((1, 2), list.FindLastPair(1, x => x == 2));
        Assert.AreEqual((2, 3), list.FindLastPair(4, 3, x => x == 3));

        Assert.AreEqual(default, list.FindLast(x => x == -1));
        Assert.AreEqual((-1, default), list.FindLastPair(1, x => x == -1));
        Assert.AreEqual((-1, default), list.FindLastPair(4, 3, x => x == -1));
    }

    [TestMethod]
    public void TryFindLast()
    {
        IList<int> list = Enumerable.Range(1, 10).ToList();

        Assert.IsTrue(list.TryFindLast(x => x == 1, out var i1) && i1 == 1);
        Assert.IsTrue(list.TryFindLast(1, x => x == 2, out var i2) && i2 == (1, 2));
        Assert.IsTrue(list.TryFindLast(4, 3, x => x == 3, out var i3) && i3 == (2, 3));

        Assert.IsTrue(list.TryFindLast(x => x == -1, out var i4) is false && i4 == default);
        Assert.IsTrue(
            list.TryFindLast(1, x => x == -1, out var i5) is false && i5 == (-1, default)
        );
        Assert.IsTrue(
            list.TryFindLast(4, 3, x => x == -1, out var i6) is false && i6 == (-1, default)
        );
    }
    #endregion
}
