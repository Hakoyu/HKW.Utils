using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtilsTests.Collections;

namespace HKW.HKWUtilsTests;

public static class IListTestUtils
{
    static Func<object?, object?, bool> _itemComparer = (a, b) => a?.Equals(b) is true;

    /// <summary>
    /// 测试
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="createList">测试列表</param>
    /// <param name="items">测试项目</param>
    /// <param name="createNewItem">创建新项目</param>
    public static void Test(Func<IList> createList, ICollection newItems)
    {
        Item(createList, newItems);
        ItemFail(createList, newItems);

        IndexOf(createList);

        Insert(createList);
        InsertFail(createList);

        RemoveAt(createList);
        RemoveAtFail(createList);
    }

    public static void Item(Func<IList> createList, ICollection newItems)
    {
        var list = createList();
        var cList = list.Cast<object>().ToList();

        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], cList[i]);
        }
        Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));

        foreach (var (e, i) in newItems.WithIndex())
        {
            list[i] = cList[i] = e;
            Assert.AreEqual(e, [list[i], cList[i]]);
            Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
        }
    }

    public static void ItemFail(Func<IList> createList, ICollection newItems)
    {
        var list = createList();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list[-1] = default!;
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list[list.Count] = default!;
        });
    }

    public static void Insert(Func<IList> createList)
    {
        var list = createList();
        var cList = list.Cast<object>().ToList();
        var count = list.Count + 100;

        var newItem = list[0]!;
        for (var i = 0; i < count; i++)
        {
            list.Insert(i, newItem);
            cList.Insert(i, newItem);
            Assert.AreEqual(list[i], cList[i]);
            Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
        }
        Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
    }

    public static void InsertFail(Func<IList> createList)
    {
        var list = createList();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.Insert(-1, default!);
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.Insert(list.Count + 100, default!);
        });
    }

    public static void RemoveAt(Func<IList> createList)
    {
        var list = createList();
        var cList = list.Cast<object>().ToList();
        var count = list.Count;

        for (var i = 0; i < count; i++)
        {
            list.RemoveAt(0);
            cList.RemoveAt(0);
            Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
        }

        list = createList();
        cList = list.Cast<object>().ToList();

        for (var i = list.Count - 1; i >= 0; i--)
        {
            list.RemoveAt(i);
            cList.RemoveAt(i);
            Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
        }

        Assert.IsTrue(list.SequenceEqual(cList, _itemComparer));
    }

    public static void RemoveAtFail(Func<IList> createList)
    {
        var list = createList();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.RemoveAt(-1);
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            list.RemoveAt(list.Count);
        });
    }

    public static void IndexOf(Func<IList> createList)
    {
        var list = createList();
        var cList = list.Cast<object>().ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i]!;
            Assert.AreEqual(list.IndexOf(item), cList.IndexOf(item));
        }
    }
}
