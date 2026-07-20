using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
public class IListTests
{
    private static Func<IList<string>> _createList =>
        () => new ObservableCollection<string>(Enumerable.Range(0, 10).Select(i => i.ToString()));

    [TestMethod]
    public void RandomItem()
    {
        var list = _createList();

        while (list.Count > 0)
        {
            var randomItem = list.RandomItem();
            Assert.Contains(randomItem, list);
            list.Remove(randomItem);
            Assert.DoesNotContain(randomItem, list);
        }
    }

    [TestMethod]
    public void RandomIndex()
    {
        var list = _createList();

        while (list.Count > 0)
        {
            var randomIndex = list.RandomIndex();
            var randomItem = list[randomIndex];
            Assert.Contains(randomItem, list);
            list.RemoveAt(randomIndex);
            Assert.DoesNotContain(randomItem, list);
        }
    }

    #region IListFind

    [TestMethod]
    public void Find()
    {
        var list = _createList();
        var cList = list.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Assert.AreEqual(item, [list.Find(x => x == item), cList.Find(x => x == item)]);
        }

        Assert.AreEqual(default, [list.Find(x => x == null), cList.Find(x => x == null)]);
    }

    [TestMethod]
    public void FindIndex()
    {
        var list = _createList();
        var cList = list.ToList();
        var count = list.Count;

        for (var i = 0; i < count; i++)
        {
            var item = list[i];
            Assert.AreEqual(i, [cList.FindIndex(x => x == item), cList.FindIndex(x => x == item)]);
            Assert.AreEqual(
                i,
                [cList.FindIndex(i, x => x == item), cList.FindIndex(i, x => x == item)]
            );
            Assert.AreEqual(
                i,
                [cList.FindIndex(i, 1, x => x == item), cList.FindIndex(i, 1, x => x == item)]
            );
            Assert.AreEqual(
                i,
                [
                    cList.FindIndex(i, count - i, x => x == item),
                    cList.FindIndex(i, count - i, x => x == item),
                ]
            );
        }

        Assert.AreEqual(-1, [cList.FindIndex(x => x == null), cList.FindIndex(x => x == null)]);
        Assert.AreEqual(
            -1,
            [cList.FindIndex(1, x => x == null), cList.FindIndex(1, x => x == null)]
        );
        Assert.AreEqual(
            -1,
            [cList.FindIndex(1, 1, x => x == null), cList.FindIndex(1, 1, x => x == null)]
        );
    }

    [TestMethod]
    public void FindPair()
    {
        var list = _createList();
        var cList = list.ToList();
        var count = list.Count;

        for (var i = 0; i < count; i++)
        {
            var item = list[i];
            var findResult = cList.Find(x => x == item);
            Assert.AreEqual(item, findResult);
            Assert.AreEqual((i, item), list.FindPair(x => x == item));
            Assert.AreEqual((i, item), list.FindPair(i, x => x == item));
            Assert.AreEqual((i, item), list.FindPair(i, 1, x => x == item));
            Assert.AreEqual((i, item), list.FindPair(i, count - i, x => x == item));
        }

        Assert.AreEqual((-1, default), list.FindPair(x => x == null));
        Assert.AreEqual((-1, default), list.FindPair(1, x => x == null));
        Assert.AreEqual((-1, default), list.FindPair(1, 1, x => x == null));
    }

    [TestMethod]
    public void FindLast()
    {
        var list = _createList();
        var cList = list.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Assert.AreEqual(item, [list.FindLast(x => x == item), cList.FindLast(x => x == item)]);
        }

        Assert.AreEqual(default, [list.FindLast(x => x == null), cList.FindLast(x => x == null)]);
    }

    [TestMethod]
    public void FindLastIndex()
    {
        var list = _createList();
        var cList = list.ToList();
        var count = list.Count;

        for (var i = 0; i < count; i++)
        {
            var item = list[i];
            Assert.AreEqual(
                i,
                [list.FindLastIndex(x => x == item), cList.FindLastIndex(x => x == item)]
            );
            Assert.AreEqual(
                i,
                [list.FindLastIndex(i, x => x == item), cList.FindLastIndex(i, x => x == item)]
            );
            Assert.AreEqual(
                i,
                [
                    list.FindLastIndex(i, 1, x => x == item),
                    cList.FindLastIndex(i, 1, x => x == item),
                ]
            );
            Assert.AreEqual(
                i,
                [
                    list.FindLastIndex(i, i + 1, x => x == item),
                    cList.FindLastIndex(i, i + 1, x => x == item),
                ]
            );
        }

        Assert.AreEqual(
            -1,
            [list.FindLastIndex(x => x == null), cList.FindLastIndex(x => x == null)]
        );
        Assert.AreEqual(
            -1,
            [list.FindLastIndex(1, x => x == null), cList.FindLastIndex(1, x => x == null)]
        );
        Assert.AreEqual(
            -1,
            [list.FindLastIndex(1, 0, x => x == null), cList.FindLastIndex(1, 0, x => x == null)]
        );
    }

    [TestMethod]
    public void FindLastPair()
    {
        var list = _createList();
        var cList = list.ToList();
        var count = list.Count;

        for (var i = 0; i < count; i++)
        {
            var item = list[i];
            var findResult = cList.Find(x => x == item);
            Assert.AreEqual(item, findResult);
            Assert.AreEqual((i, item), list.FindLastPair(x => x == item));
            Assert.AreEqual((i, item), list.FindLastPair(i, x => x == item));
            Assert.AreEqual((i, item), list.FindLastPair(i, 1, x => x == item));
            Assert.AreEqual((i, item), list.FindLastPair(i, i + 1, x => x == item));
        }

        Assert.AreEqual((-1, default), list.FindLastPair(x => x == null));
        Assert.AreEqual((-1, default), list.FindLastPair(1, x => x == null));
        Assert.AreEqual((-1, default), list.FindLastPair(1, 1, x => x == null));
    }
    #endregion
}
