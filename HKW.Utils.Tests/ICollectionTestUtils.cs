using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Tests;

public class ICollectionTestUtils
{
    public static void Test(ICollection<int> collection)
    {
        Add(collection);
        Remove(collection);
        Clear(collection);
    }

    public static void Add(ICollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Add(1);
        collection.Add(1);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Add(2);
        collection.Add(2);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Add(3);
        collection.Add(3);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        collection.Clear();
    }

    public static void Remove(ICollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Remove(comparisonList.First());
        collection.Remove(collection.First());
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Remove(comparisonList.Last());
        collection.Remove(collection.Last());
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        collection.Clear();
    }

    public static void Clear(ICollection<int> collection)
    {
        collection.Clear();
        var comparisonList = Enumerable.Range(1, 10).ToList();
        collection.AddRange(comparisonList);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        comparisonList.Clear();
        collection.Clear();
        Assert.IsTrue(collection.Count == comparisonList.Count);
        Assert.IsTrue(collection.SequenceEqual(comparisonList));

        collection.Clear();
    }
}
