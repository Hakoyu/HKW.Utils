using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests.Collections;

public class IListRangeTestUtils
{
    public static void Test(IListRange<int> list)
    {
        AddRange(list);
        InsertRange(list, 0);
        InsertRange(list, 4);
        InsertRange(list, 9);

        try
        {
            InsertRange(list, -1);
            Assert.Fail();
        }
        catch { }

        try
        {
            InsertRange(list, int.MaxValue);
            Assert.Fail();
        }
        catch { }

        RemoveRange(list, 0, 0);
        RemoveRange(list, 0, 9);
        RemoveRange(list, 4, 0);
        RemoveRange(list, 4, 4);
        RemoveRange(list, 9, 0);
        RemoveRange(list, 9, 1);

        try
        {
            RemoveRange(list, -1, 0);
            Assert.Fail();
        }
        catch { }
        try
        {
            RemoveRange(list, 0, int.MaxValue);
            Assert.Fail();
        }
        catch { }
        try
        {
            RemoveRange(list, int.MaxValue, 0);
            Assert.Fail();
        }
        catch { }
        try
        {
            RemoveRange(list, 4, 9);
            Assert.Fail();
        }
        catch { }

        Reverse(list);

        Reverse(list, 0, 0);
        Reverse(list, 0, 9);
        Reverse(list, 4, 0);
        Reverse(list, 4, 4);
        Reverse(list, 9, 0);
        Reverse(list, 9, 1);

        try
        {
            Reverse(list, -1, 0);
            Assert.Fail();
        }
        catch { }
        try
        {
            Reverse(list, 0, int.MaxValue);
            Assert.Fail();
        }
        catch { }
        try
        {
            Reverse(list, int.MaxValue, 0);
            Assert.Fail();
        }
        catch { }
        try
        {
            Reverse(list, 4, 9);
            Assert.Fail();
        }
        catch { }
    }

    public static void AddRange(IListRange<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        list.AddRange(comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));
        list.Clear();
    }

    public static void InsertRange(IListRange<int> list, int index)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();

        list.InsertRange(index, comparisonList);
        comparisonList.InsertRange(index, comparisonList);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.Clear();
    }

    public static void RemoveRange(IListRange<int> list, int index, int count)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();

        list.RemoveRange(index, count);
        comparisonList.RemoveRange(index, count);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.Clear();
    }

    public static void RemoveAll(IListRange<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();

        var predicate = new Predicate<int>(i => i > 5);
        list.RemoveAll(predicate);
        comparisonList.RemoveAll(predicate);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.Clear();
    }

    public static void Reverse(IListRange<int> list)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();

        list.Reverse();
        comparisonList.Reverse();
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.Clear();
    }

    public static void Reverse(IListRange<int> list, int index, int count)
    {
        list.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();

        list.Reverse(index, count);
        comparisonList.Reverse(index, count);
        Assert.IsTrue(list.SequenceEqual(comparisonList));

        list.Clear();
    }
}
