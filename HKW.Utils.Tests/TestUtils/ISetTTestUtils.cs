using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests;

public static class ISetTTestUtils
{
    public static void Test<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        ICollectionTTestUtils.Test(createSet, newItems);

        UnionWith(createSet, newItems);
        IntersectWith(createSet, newItems);
        ExceptWith(createSet, newItems);
        SymmetricExceptWith(createSet, newItems);

        IReadOnlySetTTestUtils.IsProperSubsetOf(createSet, newItems);
        IReadOnlySetTTestUtils.IsProperSupersetOf(createSet, newItems);
        IReadOnlySetTTestUtils.IsSubsetOf(createSet, newItems);
        IReadOnlySetTTestUtils.IsSupersetOf(createSet, newItems);
        IReadOnlySetTTestUtils.Overlaps(createSet, newItems);
    }

    public static void UnionWith<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        set.UnionWith(copySet);
        cSet.UnionWith(copySet);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.UnionWith(newItems);
        cSet.UnionWith(newItems);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.UnionWith(concatSet);
        cSet.UnionWith(concatSet);
        Assert.IsTrue(set.UnorderedEqual(cSet));
    }

    public static void IntersectWith<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        set.IntersectWith(copySet);
        cSet.IntersectWith(copySet);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.IntersectWith(newItems);
        cSet.IntersectWith(newItems);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.IntersectWith(concatSet);
        cSet.IntersectWith(concatSet);
        Assert.IsTrue(set.UnorderedEqual(cSet));
    }

    public static void ExceptWith<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        set.ExceptWith(copySet);
        cSet.ExceptWith(copySet);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.ExceptWith(newItems);
        cSet.ExceptWith(newItems);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.ExceptWith(concatSet);
        cSet.ExceptWith(concatSet);
        Assert.IsTrue(set.UnorderedEqual(cSet));
    }

    public static void SymmetricExceptWith<T>(
        Func<ISet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var copySet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        set.SymmetricExceptWith(copySet);
        cSet.SymmetricExceptWith(copySet);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.SymmetricExceptWith(newItems);
        cSet.SymmetricExceptWith(newItems);
        Assert.IsTrue(set.UnorderedEqual(cSet));

        set = createSet();
        cSet = set.ToHashSet();
        set.SymmetricExceptWith(concatSet);
        cSet.SymmetricExceptWith(concatSet);
        Assert.IsTrue(set.UnorderedEqual(cSet));
    }
}
