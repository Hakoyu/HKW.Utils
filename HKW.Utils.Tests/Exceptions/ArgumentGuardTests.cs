using System.Collections;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtilsTests.Exceptions;

[TestClass]
public class ArgumentGuardTests
{
    [TestMethod]
    public void InvalidIndex_ReportsArgumentAndValue()
    {
        IList<int> values = [1];
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(values, -1, "values", "position")
        );
        Assert.AreEqual("position", error.ParamName);
        Assert.AreEqual(-1, error.ActualValue);
        Assert.IsFalse(error.Message.Contains("must be -1"));
    }

    [TestMethod]
    public void ExpectedValues_AreEnumeratedOnce()
    {
        var source = new SingleUseEnumerable([1, 2]);
        var error = Assert.ThrowsExactly<ArgumentException>(() =>
            ArgumentException.ThrowIfAllNotEquals(3, source, "value")
        );
        Assert.AreEqual("value", error.ParamName);
        Assert.AreEqual(1, source.EnumerationCount);
        Assert.AreEqual(1, source.DisposeCount);
    }

    [TestMethod]
    public void ExpectedValues_NullIsRejected()
    {
        var error = Assert.ThrowsExactly<ArgumentNullException>(() =>
            ArgumentException.ThrowIfAllNotEquals(3, (IEnumerable<int>)null!)
        );
        Assert.AreEqual("expecteds", error.ParamName);
    }

    [TestMethod]
    public void TargetNullGuard_AcceptsObjectArgument()
    {
        var method = typeof(ArgumentNullExceptions)
            .GetMethods()
            .Single(m => m.Name == "ThrowIfNotNullableIsNull");
        Assert.AreEqual(typeof(object), method.GetParameters()[0].ParameterType);
    }

    [TestMethod]
    public void TargetNullGuard_UsesTargetTypeAndCallerName()
    {
        object? supplied = null;
        var error = Assert.ThrowsExactly<ArgumentNullException>(() =>
            ArgumentNullException.ThrowIfNotNullableIsNull<int>(supplied)
        );
        Assert.AreEqual("supplied", error.ParamName);
        ArgumentNullException.ThrowIfNotNullableIsNull<int?>(supplied);
        ArgumentNullException.ThrowIfNotNullableIsNull<string>(supplied);
        ArgumentNullException.ThrowIfNotNullableIsNull<int>(42);
    }

    [TestMethod]
    public void EmptyGuard_HasUnambiguousListOverload()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            InvalidOperationException.ThrowIfEmptyCollection(new List<int>())
        );
        InvalidOperationException.ThrowIfEmptyCollection(new List<int> { 1 });
        ICollection<int> collection = new List<int> { 1 };
        InvalidOperationException.ThrowIfEmptyCollection(collection);
        IReadOnlyCollection<int> readOnly = new List<int> { 1 };
        InvalidOperationException.ThrowIfEmptyCollection(readOnly);
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            InvalidOperationException.ThrowIfEmptyCollection((IEnumerable<int>)null!)
        );
    }

    [TestMethod]
    public void EmptyGuard_EnumeratesUnknownSourceOnceAndDisposes()
    {
        var nonempty = new SingleUseEnumerable([1, 2]);
        InvalidOperationException.ThrowIfEmptyCollection(nonempty);
        Assert.AreEqual(1, nonempty.EnumerationCount);
        Assert.AreEqual(1, nonempty.DisposeCount);
        var empty = new SingleUseEnumerable([]);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            InvalidOperationException.ThrowIfEmptyCollection(empty)
        );
        Assert.AreEqual(1, empty.EnumerationCount);
        Assert.AreEqual(1, empty.DisposeCount);
    }

    [TestMethod]
    public void IndexGuard_ValidatesBoundariesAndNull()
    {
        IList<int> values = [1];
        ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(values, 0);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ArgumentOutOfRangeException.ThrowIfIndexOutOfRange(values, 1)
        );
        var error = Assert.ThrowsExactly<ArgumentNullException>(() =>
            ArgumentOutOfRangeException.ThrowIfIndexOutOfRange((IList<int>)null!, 0, "items")
        );
        Assert.AreEqual("items", error.ParamName);
    }

    private sealed class SingleUseEnumerable(int[] values) : IEnumerable<int>
    {
        public int EnumerationCount { get; private set; }
        public int DisposeCount { get; private set; }

        public IEnumerator<int> GetEnumerator()
        {
            if (++EnumerationCount != 1)
                throw new InvalidOperationException("The sequence can only be enumerated once.");
            return Enumerate().GetEnumerator();
        }

        private IEnumerable<int> Enumerate()
        {
            try
            {
                foreach (var value in values)
                    yield return value;
            }
            finally
            {
                DisposeCount++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
