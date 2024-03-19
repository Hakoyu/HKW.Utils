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
    public static void Test<T>(IList<T> list, IList<T> comparisonList, Func<T> createNewItem)
    {
        ICollectionTestUtils.Test(list, comparisonList, createNewItem);

        ValueWithIndex(list, comparisonList, 0, createNewItem());
        ValueWithIndex(list, comparisonList, comparisonList.Count / 2, createNewItem());
        ValueWithIndex(list, comparisonList, comparisonList.Count - 1, createNewItem());

        ValueWithIndexFalse(list, comparisonList, -1, createNewItem());
        ValueWithIndexFalse(list, comparisonList, comparisonList.Count, createNewItem());

        IndexOf(list, comparisonList);

        Insert(list, comparisonList, 0, createNewItem());
        Insert(list, comparisonList, comparisonList.Count / 2, createNewItem());
        Insert(list, comparisonList, comparisonList.Count, createNewItem());

        InsertFalse(list, comparisonList, -1, createNewItem());
        InsertFalse(list, comparisonList, comparisonList.Count + 1, createNewItem());

        RemoveAt(list, comparisonList, 0);
        RemoveAt(list, comparisonList, comparisonList.Count / 2);
        RemoveAt(list, comparisonList, comparisonList.Count - 1);

        RemoveAtFalse(list, comparisonList, -1);
        RemoveAtFalse(list, comparisonList, comparisonList.Count);

        //if (list is IListFind<int> listFind)
        //    IListFindTestUtils.Test(listFind);
        //if (list is IListRange<int> listRange)
        //    IListRangeTestUtils.Test(listRange);
    }

    public static void ValueWithIndex<T>(
        IList<T> list,
        IList<T> comparisonList,
        int index,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        Assert.IsTrue(list[index]?.Equals(cList[index]));
        list[index] = cList[index] = newItem;
        Assert.IsTrue(list[index]?.Equals(cList[index]));

        list.Clear();
    }

    public static void ValueWithIndexFalse<T>(
        IList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        try
        {
            list[outRangeIndex] = newItem;
            Assert.Fail();
        }
        catch { }

        try
        {
            cList[outRangeIndex] = newItem;
        }
        catch { }
        Assert.IsTrue(list.SequenceEqual(cList));

        list.Clear();
    }

    public static void IndexOf<T>(IList<T> list, IList<T> comparisonList)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        var item = cList.Random();
        Assert.IsTrue(cList.IndexOf(item) == list.IndexOf(item));

        list.Clear();
    }

    public static void Insert<T>(IList<T> list, IList<T> comparisonList, int index, T newItem)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        cList.Insert(index, newItem);
        list.Insert(index, newItem);
        Assert.IsTrue(list.SequenceEqual(cList));

        list.Clear();
    }

    public static void InsertFalse<T>(
        IList<T> list,
        IList<T> comparisonList,
        int outRangeIndex,
        T newItem
    )
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        try
        {
            cList.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }

        try
        {
            list.Insert(outRangeIndex, newItem);
            Assert.Fail();
        }
        catch { }
        Assert.IsTrue(list.SequenceEqual(cList));

        list.Clear();
    }

    public static void RemoveAt<T>(IList<T> list, IList<T> comparisonList, int index)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        cList.RemoveAt(index);
        list.RemoveAt(index);

        list.Clear();
    }

    public static void RemoveAtFalse<T>(IList<T> list, IList<T> comparisonList, int index)
    {
        list.Clear();
        var cList = comparisonList.ToList();
        list.AddRange(cList);

        try
        {
            cList.RemoveAt(index);
            Assert.Fail();
        }
        catch { }

        try
        {
            list.RemoveAt(index);
            Assert.Fail();
        }
        catch { }

        list.Clear();
    }
}
