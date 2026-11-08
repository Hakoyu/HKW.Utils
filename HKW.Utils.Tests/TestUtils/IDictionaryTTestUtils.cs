using HKW.HKWUtils.Extensions;
using HKW.HKWUtilsTests.Extensions;

namespace HKW.HKWUtilsTests;

public static class IDictionaryTTestUtils
{
    public static void Test<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        ICollectionTTestUtils.Test(createDictionary, newItems);

        ItemSet(createDictionary, newItems);
        Add(createDictionary, newItems);
        AddFail(createDictionary, newItems);
        Remove(createDictionary);
        Clear(createDictionary);

        IReadOnlyDictionaryTTestUtils.ItemGet(createDictionary, newItems);
        IReadOnlyDictionaryTTestUtils.Keys(createDictionary, newItems);
        IReadOnlyDictionaryTTestUtils.Values(createDictionary, newItems);
        IReadOnlyDictionaryTTestUtils.ContainsKey(createDictionary, newItems);
        IReadOnlyDictionaryTTestUtils.TryGetValue(createDictionary, newItems);
    }

    public static void ItemSet<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();
        var copyDictionary = dictionary.ToDictionary();

        foreach (var (key, value) in copyDictionary.Keys.Zip(copyDictionary.Values.Reverse()))
        {
            Assert.AreEqual(dictionary[key], cDictionary[key]);
            dictionary[key] = cDictionary[key] = value;
            Assert.AreEqual(dictionary[key], cDictionary[key]);
            CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
            CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
            CollectionAssert.AreEquivalent(
                cDictionary.Values.ToArray(),
                dictionary.Values.ToArray()
            );
        }

        foreach (var item in newItems)
        {
            dictionary[item.Key] = cDictionary[item.Key] = item.Value;
            Assert.AreEqual(dictionary[item.Key], cDictionary[item.Key]);
            CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
            CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
            CollectionAssert.AreEquivalent(
                cDictionary.Values.ToArray(),
                dictionary.Values.ToArray()
            );
        }

        CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Values.ToArray(), dictionary.Values.ToArray());
    }

    public static void Add<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        foreach (var item in newItems)
        {
            dictionary.Add(item.Key, item.Value);
            cDictionary.Add(item.Key, item.Value);
            Assert.AreEqual(dictionary[item.Key], cDictionary[item.Key]);
            CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
            CollectionAssert.AreEquivalent(
                cDictionary.Values.ToArray(),
                dictionary.Values.ToArray()
            );
        }
        CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Values.ToArray(), dictionary.Values.ToArray());
    }

    public static void AddFail<TKey, TValue>(
        Func<IDictionary<TKey, TValue>> createDictionary,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>> newItems
    )
        where TKey : notnull
    {
        var dictionary = createDictionary();

        Assert.IsTrue(dictionary.Count > 0);
        var existing = dictionary.First();
        var expected = dictionary.ToArray();
        Assert.ThrowsExactly<ArgumentException>(() => dictionary.Add(existing.Key, existing.Value));
        CollectionAssert.AreEquivalent(expected, dictionary.ToArray());
    }

    public static void Remove<TKey, TValue>(Func<IDictionary<TKey, TValue>> createDictionary)
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();
        var copyDictionary = dictionary.ToDictionary();

        foreach (var pair in copyDictionary)
        {
            dictionary.Remove(pair.Key);
            cDictionary.Remove(pair.Key);
            CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
            CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
            CollectionAssert.AreEquivalent(
                cDictionary.Values.ToArray(),
                dictionary.Values.ToArray()
            );
        }

        CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Values.ToArray(), dictionary.Values.ToArray());
    }

    public static void Clear<TKey, TValue>(Func<IDictionary<TKey, TValue>> createDictionary)
        where TKey : notnull
    {
        var dictionary = createDictionary();
        var cDictionary = dictionary.ToDictionary();

        dictionary.Clear();
        cDictionary.Clear();

        Assert.HasCount(0, dictionary);
        Assert.HasCount(0, dictionary.Keys);
        Assert.HasCount(0, dictionary.Values);

        CollectionAssert.AreEquivalent(cDictionary.ToArray(), dictionary.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Keys.ToArray(), dictionary.Keys.ToArray());
        CollectionAssert.AreEquivalent(cDictionary.Values.ToArray(), dictionary.Values.ToArray());
    }
}
