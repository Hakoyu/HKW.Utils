using System.Collections;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class CyclicEnumeratorContractTests
{
    [TestMethod]
    [DataRow(false, false, 0)]
    [DataRow(false, false, 1)]
    [DataRow(false, false, 3)]
    [DataRow(false, true, 0)]
    [DataRow(false, true, 1)]
    [DataRow(false, true, 3)]
    [DataRow(true, false, 0)]
    [DataRow(true, false, 1)]
    [DataRow(true, false, 3)]
    [DataRow(true, true, 0)]
    [DataRow(true, true, 1)]
    [DataRow(true, true, 3)]
    public void YieldSource_BoundedCyclesAndInvalidCurrent(
        bool observable,
        bool autoReset,
        int count
    )
    {
        using var enumerator = Create(observable, autoReset, YieldValues(count));
        AssertInvalidCurrent(enumerator);
        Assert.AreEqual(-1, Index(enumerator));
        for (var step = 0; step < count * 3 + 2; step++)
        {
            var expected = count > 0 && (autoReset || step < count);
            Assert.AreEqual(expected, enumerator.MoveNext());
            if (expected)
            {
                Assert.AreEqual(step % count + 1, enumerator.Current);
                Assert.AreEqual(step % count, Index(enumerator));
            }
            else
            {
                AssertInvalidCurrent(enumerator);
                Assert.AreEqual(count - 1, Index(enumerator));
            }
        }
        enumerator.Reset();
        Assert.AreEqual(-1, Index(enumerator));
        AssertInvalidCurrent(enumerator);
        Assert.AreEqual(count > 0, enumerator.MoveNext());
        if (count > 0)
            Assert.AreEqual(1, enumerator.Current);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void UnsupportedReset_DisposesBeforeRecreating(bool observable, bool autoReset)
    {
        var source = new ProbeSource { ResetFailure = new NotSupportedException() };
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        if (autoReset)
            Assert.IsTrue(enumerator.MoveNext());
        else
        {
            enumerator.Reset();
            AssertInvalidCurrent(enumerator);
            Assert.IsTrue(enumerator.MoveNext());
        }
        Assert.AreEqual(0, Index(enumerator));
        Assert.AreEqual(1, enumerator.Current);
        Assert.AreSequenceEqual(new[] { "create", "dispose", "create" }, source.Operations);
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        enumerator.Dispose();
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[1].DisposeCount);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void SupportedReset_ReusesIterator(bool observable, bool autoReset)
    {
        var source = new ProbeSource();
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        enumerator.Reset();
        Assert.AreEqual(-1, Index(enumerator));
        AssertInvalidCurrent(enumerator);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(1, source.CreateCount);
        Assert.AreEqual(0, source.Iterators[0].DisposeCount);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void RealResetFailure_PropagatesWithoutRecreation(bool observable, bool autoReset)
    {
        var failure = new InvalidOperationException("Reset failed.");
        var source = new ProbeSource { ResetFailure = failure };
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        var actual = autoReset
            ? Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext())
            : Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset());
        Assert.AreSame(failure, actual);
        Assert.AreEqual(1, source.CreateCount);
        Assert.AreEqual(0, source.Iterators[0].DisposeCount);
        Assert.AreEqual(-1, Index(enumerator));
        AssertInvalidCurrent(enumerator);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void MoveNextFailure_PropagatesWithoutReset(bool observable, bool autoReset)
    {
        var failure = new InvalidOperationException("MoveNext failed.");
        var source = new ProbeSource { MoveNextFailure = failure };
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreSame(
            failure,
            Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext())
        );
        Assert.AreEqual(1, source.CreateCount);
        Assert.AreEqual(0, source.Iterators[0].ResetCount);
        AssertInvalidCurrent(enumerator);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void RecreationFailure_DetachesDisposedIteratorAndAllowsResetRetry(
        bool observable,
        bool autoReset
    )
    {
        var failure = new InvalidOperationException("Re-enumeration failed.");
        var source = new ProbeSource
        {
            ResetFailure = new NotSupportedException(),
            CreateFailure = failure,
        };
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        var actual = autoReset
            ? Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext())
            : Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset());
        Assert.AreSame(failure, actual);
        Assert.AreEqual(-1, Index(enumerator));
        AssertInvalidCurrent(enumerator);
        Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext());
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        source.CreateFailure = null;
        enumerator.Reset();
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(1, enumerator.Current);
        Assert.AreEqual(3, source.CreateCount);
        enumerator.Dispose();
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        Assert.AreEqual(1, source.Iterators[1].DisposeCount);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void FailedRecreation_DisposeDoesNotDisposeOldIteratorAgain(
        bool observable,
        bool autoReset
    )
    {
        var source = new ProbeSource
        {
            ResetFailure = new NotSupportedException(),
            CreateFailure = new InvalidOperationException("Re-enumeration failed."),
        };
        var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset());
        enumerator.Dispose();
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        AssertDisposed(enumerator);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void DisposeFailure_IsPropagatedOnlyOnce(bool observable, bool autoReset)
    {
        var failure = new InvalidOperationException("Dispose failed.");
        var source = new ProbeSource { DisposeFailure = failure };
        var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreSame(
            failure,
            Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Dispose())
        );
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        AssertDisposed(enumerator);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void ReplacementDisposeFailure_DoesNotRecreateOrDisposeTwice(
        bool observable,
        bool autoReset
    )
    {
        var failure = new InvalidOperationException("Dispose failed.");
        var source = new ProbeSource
        {
            DisposeFailure = failure,
            ResetFailure = new NotSupportedException(),
        };
        using var enumerator = Create(observable, autoReset, source);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreSame(
            failure,
            Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset())
        );
        Assert.AreEqual(1, source.CreateCount);
        AssertInvalidCurrent(enumerator);
        Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext());
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void Dispose_IsIdempotentAndBlocksOperations(bool observable, bool autoReset)
    {
        var source = new ProbeSource();
        var enumerator = Create(observable, autoReset, source);
        enumerator.Dispose();
        enumerator.Dispose();
        Assert.AreEqual(1, source.Iterators[0].DisposeCount);
        AssertDisposed(enumerator);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void DefaultStruct_IsUninitializedButSafeToDispose(bool autoReset)
    {
        var value = default(CyclicEnumerator<int>);
        value.AutoReset = autoReset;
        IEnumerator<int> enumerator = value;
        Assert.AreEqual(-1, Index(enumerator));
        AssertInvalidCurrent(enumerator);
        Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.MoveNext());
        Assert.ThrowsExactly<InvalidOperationException>(() => enumerator.Reset());
        enumerator.Dispose();
        enumerator.Dispose();
        AssertDisposed(enumerator);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void StructCopy_SharesCursorRatherThanIndependentEnumeration(bool autoReset)
    {
        var source = new ProbeSource { Count = 3 };
        var first = CyclicEnumerator.Create(source, autoReset);
        var copy = first;
        Assert.IsTrue(first.MoveNext());
        Assert.IsTrue(copy.MoveNext());
        Assert.AreEqual(2, first.Current);
        Assert.AreEqual(2, copy.Current);
        Assert.AreEqual(0, first.CurrentIndex);
        Assert.AreEqual(0, copy.CurrentIndex);
        Assert.AreEqual(1, source.CreateCount);
        first.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = copy.Current);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void NullSource_IsRejected(bool observable)
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Create(observable, false, null!));
    }

    internal static IEnumerable<int> YieldValues(int count)
    {
        for (var index = 1; index <= count; index++)
            yield return index;
    }

    private static IEnumerator<int> Create(
        bool observable,
        bool autoReset,
        IEnumerable<int> source
    ) =>
        observable
            ? ObservableCyclicEnumerator.Create(source, autoReset)
            : CyclicEnumerator.Create(source, autoReset);

    private static int Index(IEnumerator<int> enumerator) =>
        enumerator is ObservableCyclicEnumerator<int> observable
            ? observable.CurrentIndex
            : ((CyclicEnumerator<int>)enumerator).CurrentIndex;

    private static void AssertInvalidCurrent(IEnumerator<int> enumerator)
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = enumerator.Current);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            _ = ((IEnumerator)enumerator).Current
        );
    }

    private static void AssertDisposed(IEnumerator<int> enumerator)
    {
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = enumerator.Current);
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = ((IEnumerator)enumerator).Current);
        Assert.ThrowsExactly<ObjectDisposedException>(() => enumerator.MoveNext());
        Assert.ThrowsExactly<ObjectDisposedException>(() => enumerator.Reset());
    }

    internal sealed class ProbeSource : IEnumerable<int>
    {
        public int Count { get; set; } = 1;
        public int CreateCount { get; private set; }
        public Exception? ResetFailure { get; set; }
        public Exception? MoveNextFailure { get; set; }
        public Exception? CreateFailure { get; set; }
        public Exception? DisposeFailure { get; set; }
        public List<ProbeEnumerator> Iterators { get; } = [];
        public List<string> Operations { get; } = [];

        public IEnumerator<int> GetEnumerator()
        {
            CreateCount++;
            Operations.Add("create");
            if (CreateCount > 1 && CreateFailure is not null)
                throw CreateFailure;
            var enumerator = new ProbeEnumerator(this);
            Iterators.Add(enumerator);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal sealed class ProbeEnumerator(ProbeSource source) : IEnumerator<int>
    {
        private int _position = -1;
        public int DisposeCount { get; private set; }
        public int ResetCount { get; private set; }
        public int Current
        {
            get
            {
                ObjectDisposedException.ThrowIf(DisposeCount > 0, this);
                return _position + 1;
            }
        }
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            ObjectDisposedException.ThrowIf(DisposeCount > 0, this);
            if (_position >= 0 && source.MoveNextFailure is not null)
                throw source.MoveNextFailure;
            return ++_position < source.Count;
        }

        public void Reset()
        {
            ObjectDisposedException.ThrowIf(DisposeCount > 0, this);
            ResetCount++;
            if (source.ResetFailure is not null)
                throw source.ResetFailure;
            _position = -1;
        }

        public void Dispose()
        {
            DisposeCount++;
            source.Operations.Add("dispose");
            if (source.DisposeFailure is not null)
                throw source.DisposeFailure;
        }
    }
}
