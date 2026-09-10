namespace HKW.HKWUtilsTests;

public static class IReadOnlySetTTestUtils
{
    public static void Test<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        IReadOnlyCollectionTTestUtils.Test(createSet, newItems);

        UnionWithFail(createSet, newItems);
        IntersectWithFail(createSet, newItems);
        ExceptWithFail(createSet, newItems);
        SymmetricExceptWithFail(createSet, newItems);

        IsProperSubsetOf(createSet, newItems);
        IsProperSupersetOf(createSet, newItems);
        IsSubsetOf(createSet, newItems);
        IsSupersetOf(createSet, newItems);
        Overlaps(createSet, newItems);

        var set = createSet();
        if (set is IReadOnlySet<T>)
            ReadOnly_Test(() => (IReadOnlySet<T>)createSet(), newItems);
    }

    public static void UnionWithFail<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        Assert.Throws<NotSupportedException>(() => set.UnionWith(newItems));
    }

    public static void IntersectWithFail<T>(
        Func<ISet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        Assert.Throws<NotSupportedException>(() => set.IntersectWith(newItems));
    }

    public static void ExceptWithFail<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        Assert.Throws<NotSupportedException>(() => set.ExceptWith(newItems));
    }

    public static void SymmetricExceptWithFail<T>(
        Func<ISet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        Assert.Throws<NotSupportedException>(() => set.SymmetricExceptWith(newItems));
    }

    public static void IsProperSubsetOf<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsProperSubsetOf(cSet), cSet.IsProperSubsetOf(set));
        Assert.AreEqual(set.IsProperSubsetOf(newItems), cSet.IsProperSubsetOf(newItems));
        Assert.AreEqual(set.IsProperSubsetOf(concatSet), cSet.IsProperSubsetOf(concatSet));
    }

    public static void IsProperSupersetOf<T>(
        Func<ISet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsProperSupersetOf(cSet), cSet.IsProperSupersetOf(set));
        Assert.AreEqual(set.IsProperSupersetOf(newItems), cSet.IsProperSupersetOf(newItems));
        Assert.AreEqual(set.IsProperSupersetOf(concatSet), cSet.IsProperSupersetOf(concatSet));
    }

    public static void IsSubsetOf<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsSubsetOf(cSet), cSet.IsSubsetOf(set));
        Assert.AreEqual(set.IsSubsetOf(newItems), cSet.IsSubsetOf(newItems));
        Assert.AreEqual(set.IsSubsetOf(concatSet), cSet.IsSubsetOf(concatSet));
    }

    public static void IsSupersetOf<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsSupersetOf(cSet), cSet.IsSupersetOf(set));
        Assert.AreEqual(set.IsSupersetOf(newItems), cSet.IsSupersetOf(newItems));
        Assert.AreEqual(set.IsSupersetOf(concatSet), cSet.IsSupersetOf(concatSet));
    }

    public static void Overlaps<T>(Func<ISet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.Overlaps(cSet), cSet.Overlaps(set));
        Assert.AreEqual(set.Overlaps(newItems), cSet.Overlaps(newItems));
        Assert.AreEqual(set.Overlaps(concatSet), cSet.Overlaps(concatSet));
    }

    public static void ReadOnly_Test<T>(
        Func<IReadOnlySet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        IReadOnlyCollectionTTestUtils.ReadOnly_Test(createSet, newItems);

        IsProperSubsetOf(createSet, newItems);
        IsProperSupersetOf(createSet, newItems);
        IsSubsetOf(createSet, newItems);
        IsSupersetOf(createSet, newItems);
        Overlaps(createSet, newItems);
    }

    public static void IsProperSubsetOf<T>(
        Func<IReadOnlySet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsProperSubsetOf(cSet), cSet.IsProperSubsetOf(set));
        Assert.AreEqual(set.IsProperSubsetOf(newItems), cSet.IsProperSubsetOf(newItems));
        Assert.AreEqual(set.IsProperSubsetOf(concatSet), cSet.IsProperSubsetOf(concatSet));
    }

    public static void IsProperSupersetOf<T>(
        Func<IReadOnlySet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsProperSupersetOf(cSet), cSet.IsProperSupersetOf(set));
        Assert.AreEqual(set.IsProperSupersetOf(newItems), cSet.IsProperSupersetOf(newItems));
        Assert.AreEqual(set.IsProperSupersetOf(concatSet), cSet.IsProperSupersetOf(concatSet));
    }

    public static void IsSubsetOf<T>(
        Func<IReadOnlySet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsSubsetOf(cSet), cSet.IsSubsetOf(set));
        Assert.AreEqual(set.IsSubsetOf(newItems), cSet.IsSubsetOf(newItems));
        Assert.AreEqual(set.IsSubsetOf(concatSet), cSet.IsSubsetOf(concatSet));
    }

    public static void IsSupersetOf<T>(
        Func<IReadOnlySet<T>> createSet,
        IReadOnlyCollection<T> newItems
    )
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.IsSupersetOf(cSet), cSet.IsSupersetOf(set));
        Assert.AreEqual(set.IsSupersetOf(newItems), cSet.IsSupersetOf(newItems));
        Assert.AreEqual(set.IsSupersetOf(concatSet), cSet.IsSupersetOf(concatSet));
    }

    public static void Overlaps<T>(Func<IReadOnlySet<T>> createSet, IReadOnlyCollection<T> newItems)
    {
        var set = createSet();
        var cSet = set.ToHashSet();
        var concatSet = set.Concat(newItems).ToHashSet();

        Assert.AreEqual(set.Overlaps(cSet), cSet.Overlaps(set));
        Assert.AreEqual(set.Overlaps(newItems), cSet.Overlaps(newItems));
        Assert.AreEqual(set.Overlaps(concatSet), cSet.Overlaps(concatSet));
    }
}
