using System.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests;

public class IListTestUtils
{
    /// <summary>
    /// 测试
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="testList">测试列表</param>
    /// <param name="items">测试项目</param>
    /// <param name="createNewItem">创建新项目</param>
    public static void Test(IList testList, IList items, Func<object> createNewItem)
    {
        ValueWithIndex(testList, items, 0, createNewItem());
        ValueWithIndex(testList, items, items.Count / 2, createNewItem());
        ValueWithIndex(testList, items, items.Count - 1, createNewItem());

        ValueWithIndexFalse(testList, items, -1, createNewItem());
        ValueWithIndexFalse(testList, items, items.Count, createNewItem());

        IndexOf(testList, items);

        Insert(testList, items, 0, createNewItem());
        Insert(testList, items, items.Count / 2, createNewItem());
        Insert(testList, items, items.Count, createNewItem());

        InsertFalse(testList, items, -1, createNewItem());
        InsertFalse(testList, items, items.Count + 1, createNewItem());

        RemoveAt(testList, items, 0);
        RemoveAt(testList, items, items.Count / 2);
        RemoveAt(testList, items, items.Count - 1);

        RemoveAtFalse(testList, items, -1);
        RemoveAtFalse(testList, items, items.Count);
    }

    public static void ValueWithIndex(IList testList, IList items, int index, object newItem)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        Assert.IsTrue(testList[index]?.Equals(cList[index]));
        testList[index] = cList[index] = newItem;
        Assert.IsTrue(testList[index]?.Equals(cList[index]));

        testList.Clear();
    }

    public static void ValueWithIndexFalse(
        IList testList,
        IList items,
        int outRangeIndex,
        object newItem
    )
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        try
        {
            testList[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        try
        {
            cList[outRangeIndex] = newItem;
        }
        catch { }
        Assert.IsTrue(testList.ItemsEqual(cList));

        testList.Clear();
    }

    public static void IndexOf(IList testList, IList items)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        var item = cList.Random();
        Assert.AreEqual(testList.IndexOf(item), cList.IndexOf(item));

        testList.Clear();
    }

    public static void Insert(IList testList, IList items, int index, object newItem)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        cList.Insert(index, newItem);
        testList.Insert(index, newItem);
        Assert.IsTrue(testList.ItemsEqual(cList));

        testList.Clear();
    }

    public static void InsertFalse(IList testList, IList items, int outRangeIndex, object newItem)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        try
        {
            cList.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        try
        {
            testList.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }
        Assert.IsTrue(testList.ItemsEqual(cList));

        testList.Clear();
    }

    public static void RemoveAt(IList testList, IList items, int index)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        cList.RemoveAt(index);
        testList.RemoveAt(index);

        testList.Clear();
    }

    public static void RemoveAtFalse(IList testList, IList items, int index)
    {
        testList.Clear();
        var cList = items.Cast<object>().ToList();
        foreach (var i in cList)
            testList.Add(i);

        try
        {
            cList.RemoveAt(index);
            Assert.Fail();
        }
        catch { }

        try
        {
            testList.RemoveAt(index);
            Assert.Fail();
        }
        catch { }

        testList.Clear();
    }
}
