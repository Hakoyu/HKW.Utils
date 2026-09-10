using HKW.HKWUtils.Extensions;

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

        foreach (var item in newItems.RandomOrder())
        {
            collection.Add(item);
            cCollection.Add(item);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }
        Assert.HasCount(oldCount + newItems.Count, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));
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
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }

        foreach (var item in copyCollection.RandomOrder())
        {
            Assert.AreEqual(true, [collection.Remove(item), cCollection.Remove(item)]);
            Assert.IsTrue(collection.SequenceEqual(cCollection));
        }

        Assert.HasCount(0, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));
    }

    public static void Clear<T>(Func<ICollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        cCollection.Clear();
        collection.Clear();
        Assert.HasCount(0, collection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));
    }
}
