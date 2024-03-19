using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests;

public class ICollectionTestUtils
{
    static HashSet<Type> _testCompletedTypes = new();

    public static void Test<T>(
        ICollection<T> collection,
        ICollection<T> comparisonCollection,
        Func<T> createNewItem
    )
    {
        if (_testCompletedTypes.Contains(collection.GetType()))
            return;
        else
            _testCompletedTypes.Add(collection.GetType());

        if (comparisonCollection.HasValue() is false)
            throw new ArgumentException(
                "ComparisonCollection must has value",
                nameof(comparisonCollection)
            );
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);
        Assert.IsTrue(collection.SequenceEqual(cCollection));
        collection.Clear();

        Add(collection, comparisonCollection, createNewItem());
        Remove(collection, comparisonCollection);
        Clear(collection, comparisonCollection);
    }

    public static void Add<T>(
        ICollection<T> collection,
        ICollection<T> comparisonCollection,
        T newItem
    )
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        cCollection.Clear();

        cCollection.Add(newItem);
        collection.Add(newItem);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        collection.Clear();
    }

    public static void Remove<T>(ICollection<T> collection, ICollection<T> comparisonCollection)
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);

        var removeItem = cCollection.Random();
        cCollection.Remove(removeItem);
        collection.Remove(removeItem);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        collection.Clear();
    }

    public static void Clear<T>(ICollection<T> collection, ICollection<T> comparisonCollection)
    {
        collection.Clear();
        var cCollection = comparisonCollection.ToList();
        collection.AddRange(cCollection);

        cCollection.Clear();
        collection.Clear();
        Assert.IsTrue(collection.Count == cCollection.Count);
        Assert.IsTrue(collection.SequenceEqual(cCollection));

        collection.Clear();
    }
}
