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
        UnionWith(testSet, items);
        IntersectWith(testSet, items);
        ExceptWith(testSet, items);
        SymmetricExceptWith(testSet, items);
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

    public static void UnionWith<T>(ISet<T> set, ISet<T> items)
    {
        var items1 = items.Where((x, i) => i != 0 && i % 2 == 0).ToHashSet();
        var items2 = items.Where((x, i) => i != 0 && i % 2 != 0).ToHashSet();
        set.AddRange(items1);
        var cSet = items1.ToHashSet();

        set.UnionWith(items2);
        cSet.UnionWith(items2);
        Assert.IsTrue(set.ItemsEqual(cSet));

        set.Clear();
    }

    public static void IntersectWith<T>(ISet<T> set, ISet<T> items)
    {
        var items1 = items.Where((x, i) => i != 0 && i % 2 == 0).ToHashSet();
        var items2 = items.Where((x, i) => i != 0 && i % 2 != 0).ToHashSet();
        set.AddRange(items1);
        var cSet = items1.ToHashSet();

        set.IntersectWith(items2);
        cSet.IntersectWith(items2);
        Assert.IsTrue(set.ItemsEqual(cSet));

        set.Clear();
    }

    public static void ExceptWith<T>(ISet<T> set, ISet<T> items)
    {
        var items1 = items.Where((x, i) => i != 0 && i % 2 == 0).ToHashSet();
        var items2 = items.Where((x, i) => i != 0 && i % 2 != 0).ToHashSet();
        set.AddRange(items1);
        var cSet = items1.ToHashSet();

        set.ExceptWith(items2);
        cSet.ExceptWith(items2);
        Assert.IsTrue(set.ItemsEqual(cSet));

        set.Clear();
    }

    public static void SymmetricExceptWith<T>(ISet<T> set, ISet<T> items)
    {
        var items1 = items.Where((x, i) => i != 0 && i % 2 == 0).ToHashSet();
        var items2 = items.Where((x, i) => i != 0 && i % 2 != 0).ToHashSet();
        set.AddRange(items1);
        var cSet = items1.ToHashSet();

        set.SymmetricExceptWith(items2);
        cSet.SymmetricExceptWith(items2);
        Assert.IsTrue(set.ItemsEqual(cSet));

        set.Clear();
    }
}
