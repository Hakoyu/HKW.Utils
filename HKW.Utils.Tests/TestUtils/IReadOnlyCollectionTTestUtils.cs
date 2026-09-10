namespace HKW.HKWUtilsTests;

public static class IReadOnlyCollectionTTestUtils
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

        Assert.IsTrue(collection.IsReadOnly);

        AddFail(createCollection, newItems);
        RemoveFail(createCollection, newItems);
        ClearFail(createCollection);

        Count(createCollection);
        Contains(createCollection, newItems);
        CopyTo(createCollection);

        if (collection is IReadOnlyCollection<T>)
            ReadOnly_Test(() => (IReadOnlyCollection<T>)createCollection(), newItems);
    }

    public static void AddFail<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        Assert.Throws<NotSupportedException>(() => collection.Add(newItems.First()));
    }

    public static void RemoveFail<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        Assert.Throws<NotSupportedException>(() => collection.Remove(newItems.First()));
    }

    public static void ClearFail<T>(Func<ICollection<T>> createCollection)
    {
        var collection = createCollection();
        Assert.Throws<NotSupportedException>(() => collection.Clear());
    }

#pragma warning disable S2971
#pragma warning disable MSTEST0037
    public static void Count<T>(Func<ICollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        Assert.AreEqual(collection.Count, collection.Count());
        Assert.AreEqual(collection.Count, cCollection.Count);
    }
#pragma warning restore MSTEST0037
#pragma warning restore S2971

    public static void Contains<T>(
        Func<ICollection<T>> createCollection,
        IReadOnlyCollection<T> newItems
    )
    {
        var collection = createCollection();
        var cCollection = collection.ToList();

        foreach (var item in newItems)
        {
            Assert.AreEqual(false, [collection.Contains(item), cCollection.Contains(item)]);
        }

        foreach (var item in collection)
        {
            Assert.AreEqual(true, [collection.Contains(item), cCollection.Contains(item)]);
        }
    }

    public static void CopyTo<T>(Func<ICollection<T>> createCollection)
    {
        var collection = createCollection();

        for (var i = 0; i < collection.Count; i++)
        {
            var array = new T[collection.Count + i];
            collection.CopyTo(array, i);
            Assert.IsTrue(array.Skip(i).SequenceEqual(collection));
        }
    }

    public static void ReadOnly_Test<T>(
        Func<IReadOnlyCollection<T>> createCollection,
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

        ReadOnly_Count(createCollection);
    }

#pragma warning disable S2971
#pragma warning disable MSTEST0037
    public static void ReadOnly_Count<T>(Func<IReadOnlyCollection<T>> createCollection)
    {
        var collection = createCollection();
        var cCollection = collection.ToList();
        Assert.AreEqual(collection.Count, collection.Count());
        Assert.AreEqual(collection.Count, cCollection.Count);
    }
#pragma warning restore MSTEST0037
#pragma warning restore S2971
}
