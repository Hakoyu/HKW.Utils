using System.Collections;
using System.Collections.ObjectModel;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests;

[TestClass]
public class TestUtilsTests
{
    [TestMethod]
    public void EmptyBatchAssertionsFail()
    {
        Assert.ThrowsExactly<AssertFailedException>(() =>
            Assert.AreEqual<int>(1, Array.Empty<int>())
        );
        Assert.ThrowsExactly<AssertFailedException>(() => Assert.IsTrue(Array.Empty<bool?>()));
        Assert.ThrowsExactly<AssertFailedException>(() => Assert.IsFalse(Array.Empty<bool?>()));
    }

    [TestMethod]
    public void BatchAssertionsCheckEveryValue()
    {
        Assert.AreEqual<int>(1, new[] { 1, 1 });
        Assert.IsTrue(new bool?[] { true, true });
        Assert.IsFalse(new bool?[] { false, false });
        Assert.ThrowsExactly<AssertFailedException>(() => Assert.AreEqual<int>(1, new[] { 1, 2 }));
        Assert.ThrowsExactly<AssertFailedException>(() =>
            Assert.IsTrue(new bool?[] { true, false })
        );
        Assert.ThrowsExactly<AssertFailedException>(() =>
            Assert.IsFalse(new bool?[] { false, true })
        );
    }

    [TestMethod]
    public void MutableCollectionSuite_DoesNotRequireInsertionOrder()
    {
        ICollectionTTestUtils.Test<int>(() => new SortedSet<int> { 1, 3, 5 }, new[] { 0, 2, 4 });
    }

    [TestMethod]
    public void MutableCollectionSuite_RejectsEmptyInputs()
    {
        Assert.ThrowsExactly<AssertFailedException>(() =>
            ICollectionTTestUtils.Test<int>(() => new List<int>(), new[] { 1 })
        );
        Assert.ThrowsExactly<AssertFailedException>(() =>
            ICollectionTTestUtils.Test<int>(() => new List<int> { 1 }, Array.Empty<int>())
        );
    }

    [TestMethod]
    public void DictionarySuite_UsesExistingKeyForDuplicateAdd()
    {
        IDictionaryTTestUtils.Test<int, int>(
            () => new Dictionary<int, int> { [1] = 10, [2] = 20 },
            new[] { KeyValuePair.Create(3, 30) }
        );
    }

    [TestMethod]
    public void ReadOnlySuites_MatchBclContracts()
    {
        IReadOnlyListTTestUtils.Test<int>(
            () => new ReadOnlyCollection<int>(new[] { 1, 2 }),
            new[] { 3 }
        );
        IReadOnlyDictionaryTTestUtils.Test<int, int>(
            () => new ReadOnlyDictionary<int, int>(new Dictionary<int, int> { [1] = 10 }),
            new[] { KeyValuePair.Create(2, 20) }
        );
        IReadOnlySetTTestUtils.Test<int>(
            () => new ReadOnlySet<int>(new HashSet<int> { 1, 2 }),
            new[] { 3 }
        );
    }

    [TestMethod]
    public void NonGenericListSuite_HandlesNulls()
    {
        IListTestUtils.Test(
            () => new ArrayList { null, "a", "b" },
            new object?[] { null, "c", "d" }
        );
    }

    [TestMethod]
    public void NonGenericListSuite_ExercisesObservableList()
    {
        IListTestUtils.Test(
            () => new ObservableList<object?>(new object?[] { null, "a", "b" }),
            new object?[] { null, "c", "d" }
        );
    }
}
