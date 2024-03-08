using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Extensions;

[TestClass]
public class IListTests
{
    [TestMethod]
    public void Random()
    {
        IList<int> set = Enumerable.Range(0, 10).ToList();
        var randomItem = set.Random();
        Assert.IsTrue(set.Contains(randomItem));
    }

    #region IListFind
    [TestMethod]
    public void Find()
    {
        IList<int> list = new List<int>(Enumerable.Range(1, 10));

        Assert.IsTrue(list.Find(x => x == 1) == 1);
        Assert.IsTrue(list.Find(1, x => x == 2) == (1, 2));
        Assert.IsTrue(list.Find(1, 3, x => x == 3) == (2, 3));

        Assert.IsTrue(list.Find(x => x == -1) == default);
        Assert.IsTrue(list.Find(1, x => x == -1) == (-1, default));
        Assert.IsTrue(list.Find(1, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void TryFind()
    {
        IList<int> list = new List<int>(Enumerable.Range(1, 10));

        Assert.IsTrue(list.TryFind(x => x == 1, out var i1) && i1 == 1);
        Assert.IsTrue(list.TryFind(1, x => x == 2, out var i2) && i2 == (1, 2));
        Assert.IsTrue(list.TryFind(1, 3, x => x == 3, out var i3) && i3 == (2, 3));

        Assert.IsTrue(list.TryFind(x => x == -1, out var i4) is false && i4 == default);
        Assert.IsTrue(list.TryFind(1, x => x == -1, out var i5) is false && i5 == (-1, default));
        Assert.IsTrue(list.TryFind(1, 3, x => x == -1, out var i6) is false && i6 == (-1, default));
    }

    [TestMethod]
    public void FindLast()
    {
        IList<int> list = new List<int>(Enumerable.Range(1, 10));

        Assert.IsTrue(list.FindLast(x => x == 1) == 1);
        Assert.IsTrue(list.FindLast(1, x => x == 2) == (1, 2));
        Assert.IsTrue(list.FindLast(4, 3, x => x == 3) == (2, 3));

        Assert.IsTrue(list.FindLast(x => x == -1) == default);
        Assert.IsTrue(list.FindLast(1, x => x == -1) == (-1, default));
        Assert.IsTrue(list.FindLast(4, 3, x => x == -1) == (-1, default));
    }

    [TestMethod]
    public void TryFindLast()
    {
        IList<int> list = new List<int>(Enumerable.Range(1, 10));

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
