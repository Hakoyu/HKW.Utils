namespace HKW.HKWUtilsTests;

public static class ICollectionTTestUtils
{
    public static void Test<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        if (collection.Count == 0)
            Assert.Fail("Collection must contain items");

        if (newItems.Count == 0)
            Assert.Fail("NewItems must contain items");

        if (collection.Any(i => newItems.Contains(i)))
            Assert.Fail("NewItems must not contain items already present in collection.");

        Add(createCollection, newItems);
        Remove(createCollection, newItems);
        Clear(createCollection);

        IReadOnlyCollectionTTestUtils.Count(createCollection);
        IReadOnlyCollectionTTestUtils.Contains(createCollection, newItems);
        IReadOnlyCollectionTTestUtils.CopyTo(createCollection);
    }

    public static void Add<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var oldCount = cCollection.Count;

        var shuffled = newItems.ToArray();
        new Random(1703).Shuffle(shuffled);
        foreach (var item in shuffled)
        {
            collection.Add(item);
            cCollection.Add(item);
            CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
        }
        Assert.HasCount(oldCount + newItems.Count, collection);
        CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
    }

    public static void Remove<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        var copyCollection = collection.ToList();

        foreach (var item in newItems)
        {
            Assert.AreEqual(false, [collection.Remove(item), cCollection.Remove(item)]);
            CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
        }

        var shuffled = copyCollection.ToArray();
        new Random(1704).Shuffle(shuffled);
        foreach (var item in shuffled)
        {
            Assert.AreEqual(true, [collection.Remove(item), cCollection.Remove(item)]);
            CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
        }

        Assert.HasCount(0, collection);
        CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
    }

    public static void Clear<T>(Func<ICollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        cCollection.Clear();
        collection.Clear();
        Assert.HasCount(0, collection);
        CollectionAssert.AreEquivalent(cCollection.ToArray(), collection.ToArray());
    }
}
