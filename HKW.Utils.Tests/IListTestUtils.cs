using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests;

public class IListTestUtils
{
    public static void Test(IList<int> list)
    {
        ICollectionTestUtils.Test(list);
        GetValueWithIndex(list);
        IndexOf(list);
        Insert(list);
        RemoveAt(list);
    }

    public static void GetValueWithIndex(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        Assert.IsTrue(comparisonList[0] == comparisonList.First());
        Assert.IsTrue(list[0] == list.First());

        Assert.IsTrue(comparisonList[comparisonList.Count / 2] == list[list.Count / 2]);

        Assert.IsTrue(comparisonList[^1] == comparisonList.Last());
        Assert.IsTrue(list[^1] == list.Last());

        list.Clear();
    }

    public static void IndexOf(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        Assert.IsTrue(comparisonList.IndexOf(comparisonList[0]) == 0);
        Assert.IsTrue(list.IndexOf(list[0]) == 0);

        Assert.IsTrue(
            comparisonList.IndexOf(comparisonList[comparisonList.Count / 2])
                == comparisonList.Count / 2
        );
        Assert.IsTrue(list.IndexOf(list[list.Count / 2]) == list.Count / 2);

        Assert.IsTrue(comparisonList.IndexOf(comparisonList[^1]) == list.Count - 1);
        Assert.IsTrue(list.IndexOf(list[^1]) == list.Count - 1);

        Assert.IsTrue(list.IndexOf(int.MinValue) == -1);
        Assert.IsTrue(list.IndexOf(int.MaxValue) == -1);

        list.Clear();
    }

    public static void Insert(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        comparisonList.Insert(0, 1);
        list.Insert(0, 1);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        comparisonList.Insert(comparisonList.Count / 2, 2);
        list.Insert(list.Count / 2, 2);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        comparisonList.Insert(comparisonList.Count - 1, 3);
        list.Insert(list.Count - 1, 3);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        try
        {
            list.Insert(-1, 3);
            Assert.Fail();
        }
        catch
        {
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }

        try
        {
            list.Insert(int.MaxValue, 3);
            Assert.Fail();
        }
        catch
        {
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }

        list.Clear();
    }

    public static void RemoveAt(IList<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);

        comparisonList.RemoveAt(0);
        list.RemoveAt(0);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        comparisonList.RemoveAt(comparisonList.Count / 2);
        list.RemoveAt(list.Count / 2);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        comparisonList.RemoveAt(comparisonList.Count - 1);
        list.RemoveAt(list.Count - 1);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        try
        {
            list.RemoveAt(-1);
            Assert.Fail();
        }
        catch
        {
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }

        try
        {
            list.RemoveAt(int.MaxValue);
            Assert.Fail();
        }
        catch
        {
            Assert.IsTrue(list.SequenceEqual(comparisonList));
        }

        list.Clear();
    }
}
