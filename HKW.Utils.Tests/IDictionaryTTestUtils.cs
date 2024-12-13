using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Tests.Extensions;

namespace HKW.HKWUtils.Tests;

public class IDictionaryTTestUtils
{
    public static void Test<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> items,
        Func<KeyValuePair<TKey, TValue>> createNewItem
    )
    {
        ICollectionTestUtils.Test(dictionary, items, createNewItem);

        Add(dictionary, items, createNewItem());
        Remove(dictionary, items);
        Clear(dictionary, items);
    }

    public static void Add<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> items,
        KeyValuePair<TKey, TValue> newItem
    )
    {
        dictionary.Clear();
        var cDictionary = items.ToList();
        cDictionary.Clear();

        cDictionary.Add(newItem);
        dictionary.Add(newItem);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        dictionary.Clear();
    }

    public static void Remove<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> items
    )
    {
        dictionary.Clear();
        var cDictionary = items.ToList();
        dictionary.AddRange(cDictionary);

        var removeItem = cDictionary.Random();
        cDictionary.Remove(removeItem);
        dictionary.Remove(removeItem);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        dictionary.Clear();
    }

    public static void Clear<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> items
    )
    {
        dictionary.Clear();
        var cDictionary = items.ToList();
        dictionary.AddRange(cDictionary);

        cDictionary.Clear();
        dictionary.Clear();
        Assert.IsTrue(dictionary.Count == cDictionary.Count);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        dictionary.Clear();
    }
}
