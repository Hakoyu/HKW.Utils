using HKW.HKWUtils.Collections;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class OrderedHashSetTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void DirectSelfInputHasSetSemantics(int operation)
    {
        var set = new OrderedHashSet<int>([1, 2, 3]);
        Apply(set, set, operation);
        CollectionAssert.AreEqual(
            operation is 1 or 3 ? Array.Empty<int>() : [1, 2, 3],
            set.ToArray()
        );
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void DelayedSelfInputIsSnapshottedBeforeMutation(int operation)
    {
        var set = new OrderedHashSet<int>([1, 2, 3]);
        Apply(set, set.Select(x => x + 1), operation);
        int[] expected = operation switch
        {
            0 => [1, 2, 3, 4],
            1 => [1],
            2 => [2, 3],
            _ => [1, 4],
        };
        CollectionAssert.AreEqual(expected, set.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void EntireInputIsReadBeforeAnyMutation(int operation)
    {
        var set = new OrderedHashSet<int>([1, 2, 3]);
        var enumerations = 0;
        IEnumerable<int> Input()
        {
            enumerations++;
            foreach (var item in new[] { 2, 4, 4, 5, 2 })
            {
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, set.ToArray());
                yield return item;
            }
        }
        Apply(set, Input(), operation);
        Assert.AreEqual(1, enumerations);
        int[] expected = operation switch
        {
            0 => [1, 2, 3, 4, 5],
            1 => [1, 3],
            2 => [2],
            _ => [1, 3, 4, 5],
        };
        CollectionAssert.AreEqual(expected, set.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void InputFailureLeavesSetUnchanged(int operation)
    {
        var set = new OrderedHashSet<int>([1, 2, 3]);
        IEnumerable<int> Input()
        {
            yield return 4;
            yield return 2;
            throw new InvalidOperationException("Input enumeration failed.");
        }
        Assert.ThrowsExactly<InvalidOperationException>(() => Apply(set, Input(), operation));
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, set.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void ReceiverComparerControlsDuplicatesAndFirstRepresentatives(int operation)
    {
        var set = new OrderedHashSet<string>(["a", "B", "c"], StringComparer.OrdinalIgnoreCase);
        Apply(set, new[] { "b", "D", "d", "E", "e", "B" }, operation);
        string[] expected = operation switch
        {
            0 => ["a", "B", "c", "D", "E"],
            1 => ["a", "c"],
            2 => ["B"],
            _ => ["a", "c", "D", "E"],
        };
        CollectionAssert.AreEqual(expected, set.ToArray());
    }

    [TestMethod]
    public void EmptySetAndConstructorPreserveFirstOccurrences()
    {
        var input = new[] { "B", "a", "b", "C", "A" };
        var set = new OrderedHashSet<string>(input, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEqual(new[] { "B", "a", "C" }, set.ToArray());
        set.Clear();
        set.SymmetricExceptWith(input);
        CollectionAssert.AreEqual(new[] { "B", "a", "C" }, set.ToArray());
        Assert.IsTrue(set.Remove("A"));
        Assert.IsTrue(set.Add("a"));
        CollectionAssert.AreEqual(new[] { "B", "C", "a" }, set.ToArray());
    }

    [TestMethod]
    public void SetRelationsUseReceiverComparerWithoutChangingOrder()
    {
        var set = new OrderedHashSet<string>(["b", "A"], StringComparer.OrdinalIgnoreCase);
        var other = new HashSet<string>(["B", "a", "A"], StringComparer.Ordinal);
        Assert.IsTrue(set.SetEquals(other));
        Assert.IsTrue(set.IsSubsetOf(other));
        Assert.IsTrue(set.IsSupersetOf(other));
        Assert.IsFalse(set.IsProperSubsetOf(other));
        Assert.IsFalse(set.IsProperSupersetOf(other));
        Assert.IsTrue(set.IsProperSubsetOf(other.Append("c")));
        Assert.IsTrue(set.IsProperSupersetOf(new[] { "a", "A" }));
        Assert.IsTrue(set.Overlaps(new[] { "B" }));
        CollectionAssert.AreEqual(new[] { "b", "A" }, set.ToArray());
    }

    [TestMethod]
    public void ListOperationsPreserveOrderAndRejectDuplicates()
    {
        IList<string> set = new OrderedHashSet<string>(
            ["a", "b", "c"],
            StringComparer.OrdinalIgnoreCase
        );
        set.Insert(1, "d");
        set[2] = "e";
        Assert.ThrowsExactly<ArgumentException>(() => set.Insert(0, "A"));
        Assert.ThrowsExactly<ArgumentException>(() => set[1] = "C");
        CollectionAssert.AreEqual(new[] { "a", "d", "e", "c" }, set.ToArray());
        set.RemoveAt(0);
        CollectionAssert.AreEqual(new[] { "d", "e", "c" }, set.ToArray());
        Assert.AreEqual(1, set.IndexOf("E"));
    }

    private static void Apply<T>(OrderedHashSet<T> set, IEnumerable<T> other, int operation)
        where T : notnull
    {
        switch (operation)
        {
            case 0:
                set.UnionWith(other);
                break;
            case 1:
                set.ExceptWith(other);
                break;
            case 2:
                set.IntersectWith(other);
                break;
            default:
                set.SymmetricExceptWith(other);
                break;
        }
    }
}
