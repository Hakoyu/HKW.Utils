using System.Collections;
using HKW.HKWUtils.Collections;

namespace HKW.HKWUtilsTests.Collections;

[TestClass]
public class UndoableListRepairTests
{
    [TestMethod]
    public void ConstructorRejectsNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new UndoableListWrapper<string, IList<string>>(null!)
        );
    }

    [TestMethod]
    public void ExplicitOperationNamesAreUnambiguousForIntegers()
    {
        var type = typeof(UndoableList<int>);
        foreach (
            var name in new[] { "UndoCount", "UndoThroughItem", "RedoCount", "RedoThroughItem" }
        )
            Assert.IsNotNull(type.GetMethod(name, [typeof(int)]), name);
        Assert.IsNull(type.GetMethod("Undo", [typeof(int)]));
        Assert.IsNull(type.GetMethod("Redo", [typeof(int)]));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void FailedRedoRetainsRecoveryItem(int operation)
    {
        var backend = new Backend<string>(["a", "b", "c"]);
        var list = new UndoableListWrapper<string, Backend<string>>(backend);
        Assert.IsTrue(list.UndoCount(2));
        backend.AddsBeforeFailure = 0;
        Assert.ThrowsExactly<InvalidOperationException>(() => Redo(list, operation));
        CollectionAssert.AreEqual(new[] { "a" }, list.ToArray());
        CollectionAssert.AreEqual(new[] { "b", "c" }, list.UndoStack.ToArray());
        backend.AddsBeforeFailure = int.MaxValue;
        Assert.IsTrue(list.RedoCount(2));
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, list.ToArray());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void FailedBatchRedoCommitsOnlySuccessfulPrefix(int operation)
    {
        var backend = new Backend<string>(["a", "b", "c"]);
        var list = new UndoableListWrapper<string, Backend<string>>(backend);
        Assert.IsTrue(list.UndoCount(2));
        backend.AddsBeforeFailure = 1;
        Assert.ThrowsExactly<InvalidOperationException>(() => Redo(list, operation));
        CollectionAssert.AreEqual(new[] { "a", "b" }, list.ToArray());
        CollectionAssert.AreEqual(new[] { "c" }, list.UndoStack.ToArray());
        backend.AddsBeforeFailure = int.MaxValue;
        Assert.IsTrue(list.Redo());
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, list.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void ReadOnlyBackendRejectsRedoWithoutLosingHistory(int operation)
    {
        var backend = new Backend<string>(["a", "b", "c"]);
        var list = new UndoableListWrapper<string, Backend<string>>(backend);
        Assert.IsTrue(list.UndoCount(2));
        backend.IsReadOnly = true;
        Assert.IsTrue(list.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => Redo(list, operation));
        CollectionAssert.AreEqual(new[] { "a" }, list.ToArray());
        CollectionAssert.AreEqual(new[] { "b", "c" }, list.UndoStack.ToArray());
    }

    [TestMethod]
    public void ReadOnlyBackendRejectsUndoWithoutCreatingHistory()
    {
        var list = new UndoableListWrapper<int, IList<int>>(Array.AsReadOnly(new[] { 1, 2 }));
        Assert.IsTrue(list.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => list.Undo());
        Assert.IsEmpty(list.UndoStack);
        CollectionAssert.AreEqual(new[] { 1, 2 }, list.ToArray());
    }

    [TestMethod]
    public void FailedUndoPreservesSuccessfullyRemovedTail()
    {
        var backend = new Backend<string>(["a", "b", "c"]) { RemovesBeforeFailure = 1 };
        var list = new UndoableListWrapper<string, Backend<string>>(backend);
        Assert.ThrowsExactly<InvalidOperationException>(() => list.UndoCount(2));
        CollectionAssert.AreEqual(new[] { "a", "b" }, list.ToArray());
        CollectionAssert.AreEqual(new[] { "c" }, list.UndoStack.ToArray());
        Assert.IsTrue(list.Redo());
        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, list.ToArray());
    }

    [TestMethod]
    public void ItemOperationsUseLastSourceAndFirstRecoveryOccurrence()
    {
        var list = new UndoableList<string>(["a", "b", "a", "c", "a", "d"]);
        Assert.IsTrue(list.UndoThroughItem("a"));
        CollectionAssert.AreEqual(new[] { "a", "b", "a", "c" }, list.ToArray());
        Assert.IsTrue(list.RedoThroughItem("a"));
        CollectionAssert.AreEqual(new[] { "d" }, list.UndoStack.ToArray());
        Assert.IsTrue(list.UndoCount(5));
        Assert.IsTrue(list.RedoThroughItem("a"));
        CollectionAssert.AreEqual(new[] { "a" }, list.ToArray());
        CollectionAssert.AreEqual(new[] { "b", "a", "c", "a", "d" }, list.UndoStack.ToArray());
    }

    [TestMethod]
    public void FailedOrdinaryEditRetainsHistoryAndSuccessfulEditClearsIt()
    {
        var backend = new Backend<string>(["a", "b"]);
        var list = new UndoableListWrapper<string, Backend<string>>(backend);
        Assert.IsTrue(list.Undo());
        backend.AddsBeforeFailure = 0;
        Assert.ThrowsExactly<InvalidOperationException>(() => list.Add("c"));
        Assert.IsFalse(list.Remove("missing"));
        CollectionAssert.AreEqual(new[] { "b" }, list.UndoStack.ToArray());
        backend.AddsBeforeFailure = int.MaxValue;
        list.Add("c");
        Assert.IsEmpty(list.UndoStack);
        Assert.IsTrue(list.Undo());
        CollectionAssert.AreEqual(new[] { "a" }, list.ToArray());
    }

    [TestMethod]
    public void IntegerCountsAndItemsHaveDistinctSemantics()
    {
        var list = new UndoableList<int>([1, 2, 1, 3]);
        Assert.IsFalse(list.UndoCount(0));
        Assert.IsFalse(list.UndoCount(-1));
        Assert.IsFalse(list.UndoCount(5));
        Assert.IsFalse(list.UndoThroughItem(9));
        Assert.IsTrue(list.UndoThroughItem(1));
        CollectionAssert.AreEqual(new[] { 1, 2 }, list.ToArray());
        Assert.IsTrue(list.RedoThroughItem(1));
        CollectionAssert.AreEqual(new[] { 1, 2, 1 }, list.ToArray());
        Assert.IsFalse(list.RedoCount(0));
        Assert.IsFalse(list.RedoCount(-1));
        Assert.IsFalse(list.RedoCount(2));
        Assert.IsFalse(list.RedoThroughItem(9));
        Assert.IsTrue(list.RedoCount(1));
        Assert.IsTrue(list.UndoCount(1));
        CollectionAssert.AreEqual(new[] { 1, 2, 1 }, list.ToArray());
        CollectionAssert.AreEqual(new[] { 3 }, list.UndoStack.ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void SuccessfulOrdinaryEditsClearRecoveryHistory(int operation)
    {
        var list = new UndoableList<int>([1, 2, 3]);
        var history = list.UndoStack;
        Assert.IsTrue(list.Undo());
        switch (operation)
        {
            case 0:
                list[0] = 4;
                break;
            case 1:
                list.Insert(0, 4);
                break;
            case 2:
                Assert.IsTrue(list.Remove(1));
                break;
            case 3:
                list.RemoveAt(0);
                break;
            default:
                list.Clear();
                break;
        }
        Assert.IsEmpty(history);
        Assert.IsFalse(list.Redo());
    }

    [TestMethod]
    public void ReadOnlyEmptyBackendAllowsNoOpRequests()
    {
        var list = new UndoableListWrapper<int, IList<int>>(Array.AsReadOnly(Array.Empty<int>()));
        Assert.IsFalse(list.Undo());
        Assert.IsFalse(list.UndoCount(1));
        Assert.IsFalse(list.UndoThroughItem(0));
        Assert.IsFalse(list.Redo());
        Assert.IsFalse(list.RedoCount(1));
        Assert.IsFalse(list.RedoThroughItem(0));
    }

    private static bool Redo(UndoableListWrapper<string, Backend<string>> list, int operation) =>
        operation switch
        {
            0 => list.Redo(),
            1 => list.RedoCount(2),
            _ => list.RedoThroughItem("c"),
        };

    private sealed class Backend<T>(IEnumerable<T> items) : IList<T>
    {
        private readonly List<T> _items = new(items);
        public int AddsBeforeFailure { get; set; } = int.MaxValue;
        public int RemovesBeforeFailure { get; set; } = int.MaxValue;
        public bool IsReadOnly { get; set; }
        public int Count => _items.Count;
        public T this[int index]
        {
            get => _items[index];
            set
            {
                CheckWritable();
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            CheckWritable();
            if (AddsBeforeFailure == 0)
                throw new InvalidOperationException("Append rejected.");
            AddsBeforeFailure--;
            _items.Add(item);
        }

        public void RemoveAt(int index)
        {
            CheckWritable();
            if (RemovesBeforeFailure == 0)
                throw new InvalidOperationException("Removal rejected.");
            RemovesBeforeFailure--;
            _items.RemoveAt(index);
        }

        public void Clear()
        {
            CheckWritable();
            _items.Clear();
        }

        public void Insert(int index, T item)
        {
            CheckWritable();
            _items.Insert(index, item);
        }

        public bool Remove(T item)
        {
            CheckWritable();
            return _items.Remove(item);
        }

        public bool Contains(T item) => _items.Contains(item);

        public int IndexOf(T item) => _items.IndexOf(item);

        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void CheckWritable()
        {
            if (IsReadOnly)
                throw new NotSupportedException("The backend is read-only.");
        }
    }
}
