using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests;

public class ISetTestUtils
{
    /// <summary>
    /// 测试
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="testSet">测试集合</param>
    /// <param name="items">测试项目</param>
    /// <param name="createNewItem">创建新项目</param>
    public static void Test<T>(ISet<T> testSet, ISet<T> items, Func<T> createNewItem)
    {
        ICollectionTestUtils.Test(testSet, items, createNewItem);
        IsProperSubsetOf(testSet, items, createNewItem());
        IsProperSupersetOf(testSet, items);
        IsSubsetOf(testSet, items, createNewItem());
        IsSupersetOf(testSet, items);
        Overlaps(testSet, items);
    }

    public static void IsProperSubsetOf<T>(ISet<T> set, ISet<T> items, T newItem)
    {
        set.AddRange(items);
        var cSet = items.ToHashSet();
        cSet.Add(newItem);

        Assert.IsTrue(set.IsProperSubsetOf(cSet));

        set.Clear();
    }

    public static void IsProperSupersetOf<T>(ISet<T> set, ISet<T> items)
    {
        set.AddRange(items);
        var cSet = items.RandomOrder().Skip(items.Count / 2).ToHashSet();

        Assert.IsTrue(set.IsProperSupersetOf(cSet));

        set.Clear();
    }

    public static void IsSubsetOf<T>(ISet<T> set, ISet<T> items, T newItem)
    {
        set.AddRange(items);
        var cSet = items.ToHashSet();
        cSet.Add(newItem);

        Assert.IsTrue(set.IsSubsetOf(cSet));

        set.Clear();
    }

    public static void IsSupersetOf<T>(ISet<T> set, ISet<T> items)
    {
        set.AddRange(items);
        var cSet = items.RandomOrder().Skip(items.Count / 2).ToHashSet();

        Assert.IsTrue(set.IsSupersetOf(cSet));

        set.Clear();
    }

    public static void Overlaps<T>(ISet<T> set, ISet<T> items)
    {
        set.AddRange(items);
        var cSet = items.ToHashSet();

        Assert.IsTrue(set.Overlaps(cSet));

        set.Clear();
    }
}
