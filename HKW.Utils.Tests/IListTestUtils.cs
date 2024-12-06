using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Tests.Collections;

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
    public static void Test<T>(IList<T> testList, IList<T> items, Func<T> createNewItem)
    {
        ICollectionTestUtils.Test(testList, items, createNewItem);

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

    public static void ValueWithIndex<T>(IList<T> testList, IList<T> items, int index, T newItem)
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

        Assert.IsTrue(testList[index]?.Equals(cList[index]));
        testList[index] = cList[index] = newItem;
        Assert.IsTrue(testList[index]?.Equals(cList[index]));

        testList.Clear();
    }

    public static void ValueWithIndexFalse<T>(
        IList<T> testList,
        IList<T> items,
        int outRangeIndex,
        T newItem
    )
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

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
        Assert.IsTrue(testList.SequenceEqual(cList));

        testList.Clear();
    }

    public static void IndexOf<T>(IList<T> testList, IList<T> items)
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

        var item = cList.Random();
        Assert.IsTrue(cList.IndexOf(item) == testList.IndexOf(item));

        testList.Clear();
    }

    public static void Insert<T>(IList<T> testList, IList<T> items, int index, T newItem)
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

        cList.Insert(index, newItem);
        testList.Insert(index, newItem);
        Assert.IsTrue(testList.SequenceEqual(cList));

        testList.Clear();
    }

    public static void InsertFalse<T>(
        IList<T> testList,
        IList<T> items,
        int outRangeIndex,
        T newItem
    )
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

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
        Assert.IsTrue(testList.SequenceEqual(cList));

        testList.Clear();
    }

    public static void RemoveAt<T>(IList<T> testList, IList<T> items, int index)
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

        cList.RemoveAt(index);
        testList.RemoveAt(index);

        testList.Clear();
    }

    public static void RemoveAtFalse<T>(IList<T> testList, IList<T> items, int index)
    {
        testList.Clear();
        var cList = items.ToList();
        testList.AddRange(cList);

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
