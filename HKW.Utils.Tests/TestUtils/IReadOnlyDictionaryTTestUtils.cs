namespace HKW.HKWUtilsTests;

public static class IReadOnlyDictionaryTTestUtils
{
    public static void Test<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        IReadOnlyCollectionTTestUtils.Test(createDictionary, newItems);

        ItemGet(createDictionary, newItems);
        ItemSetFail(createDictionary, newItems);
        AddFail(createDictionary, newItems);
        RemoveFail(createDictionary, newItems);

        Keys(createDictionary, newItems);
        Values(createDictionary, newItems);
        ContainsKey(createDictionary, newItems);
        TryGetValue(createDictionary, newItems);

        var dictionary = createDictionary();
        if (dictionary is IReadOnlyDictionary<TKey, TValue>)
            ReadOnly_Test(() => (IReadOnlyDictionary<TKey, TValue>)createDictionary(), newItems);
    }

    public static void ItemGet<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        foreach (var pair in dictionary)
        {
            Assert.AreEqual(dictionary[pair.Key], cDictionary[pair.Key]);
        }

        foreach (var item in newItems)
        {
            Assert.Throws<KeyNotFoundException>(() => dictionary[item.Key]);
        }
    }

    public static void ItemSetFail<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
    {
        var dictionary = createDictionary();

        foreach (var item in newItems)
        {
            Assert.Throws<NotSupportedException>(() => dictionary[item.Key] = item.Value);
        }
    }

    public static void AddFail<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
    {
        var dictionary = createDictionary();
        foreach (var item in newItems)
        {
            Assert.Throws<NotSupportedException>(() => dictionary.Add(item.Key, item.Value));
        }
    }

    public static void RemoveFail<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
    {
        var dictionary = createDictionary();
        foreach (var pair in dictionary)
        {
            Assert.IsFalse(dictionary.Remove(pair.Key));
        }

        foreach (var item in newItems)
        {
            Assert.IsFalse(dictionary.Remove(item.Key));
        }
    }

    public static void Keys<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        Assert.IsTrue(dictionary.Keys.SequenceEqual(cDictionary.Keys));
    }

    public static void Values<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        Assert.IsTrue(dictionary.Values.SequenceEqual(cDictionary.Values));
    }

    public static void ContainsKey<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        foreach (var pair in dictionary)
        {
            Assert.IsTrue(dictionary.ContainsKey(pair.Key));
        }

        foreach (var item in newItems)
        {
            Assert.IsFalse(dictionary.ContainsKey(item.Key));
        }
    }

    public static void TryGetValue<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        foreach (var pair in dictionary)
        {
            Assert.AreEqual(
                dictionary.TryGetValue(pair.Key, out var value1),
                cDictionary.TryGetValue(pair.Key, out var value2)
            );
            Assert.AreEqual(value1, value2);
        }

        foreach (var item in newItems)
        {
            Assert.AreEqual(
                dictionary.TryGetValue(item.Key, out var value1),
                cDictionary.TryGetValue(item.Key, out var value2)
            );
            Assert.AreEqual(value1, value2);
        }
    }

    public static void ReadOnly_Test<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        IReadOnlyCollectionTTestUtils.ReadOnly_Test(createDictionary, newItems);

        ReadOnly_ItemGet(createDictionary, newItems);

        ReadOnly_Keys(createDictionary, newItems);
        ReadOnly_Values(createDictionary, newItems);
        ReadOnly_ContainsKey(createDictionary, newItems);
        ReadOnly_TryGetValue(createDictionary, newItems);
    }

    public static void ReadOnly_ItemGet<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        foreach (var pair in dictionary)
        {
            Assert.AreEqual(dictionary[pair.Key], cDictionary[pair.Key]);
        }

        foreach (var item in newItems)
        {
            Assert.Throws<KeyNotFoundException>(() => dictionary[item.Key]);
        }
    }

    public static void ReadOnly_Keys<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        Assert.IsTrue(dictionary.Keys.SequenceEqual(cDictionary.Keys));
    }

    public static void ReadOnly_Values<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        Assert.IsTrue(dictionary.Values.SequenceEqual(cDictionary.Values));
    }

    public static void ReadOnly_ContainsKey<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        foreach (var pair in dictionary)
        {
            Assert.IsTrue(dictionary.ContainsKey(pair.Key));
        }

        foreach (var item in newItems)
        {
            Assert.IsFalse(dictionary.ContainsKey(item.Key));
        }
    }

    public static void ReadOnly_TryGetValue<TKey, TValue>(
        Func<IReadOnlyDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        foreach (var pair in dictionary)
        {
            Assert.AreEqual(
                dictionary.TryGetValue(pair.Key, out var value1),
                cDictionary.TryGetValue(pair.Key, out var value2)
            );
            Assert.AreEqual(value1, value2);
        }

        foreach (var item in newItems)
        {
            Assert.AreEqual(
                dictionary.TryGetValue(item.Key, out var value1),
                cDictionary.TryGetValue(item.Key, out var value2)
            );
            Assert.AreEqual(value1, value2);
        }
    }
}
