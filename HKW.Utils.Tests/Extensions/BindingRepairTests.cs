using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
[DoNotParallelize]
public class BindingRepairTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void F16_ListInitialSnapshot(bool custom)
    {
        var source = new ObservableList<int>([1, 2]);
        var target = new List<int> { 99 };
        BindList(source, target, custom);
        try
        {
            CollectionAssert.AreEqual(new[] { 1, 2 }, target);
        }
        finally
        {
            BindList(source, target, custom, true);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void F16_DictionaryAndSetInitialSnapshot(bool custom)
    {
        var source = new ObservableDictionary<int, int> { [1] = 2 };
        var target = new Dictionary<int, int> { [99] = 99 };
        var setSource = new ObservableSet<int>([1, 2]);
        var set = new HashSet<int> { 99 };
        if (custom)
        {
            source.BindingDictionaryX(target);
            setSource.BindingSetX(set);
        }
        else
        {
            source.BindingDictionary(target);
            setSource.BindingSet(set);
        }
        try
        {
            Assert.AreEqual(1, target.Count);
            Assert.AreEqual(2, target[1]);
            Assert.IsTrue(set.SetEquals(setSource));
        }
        finally
        {
            if (custom)
            {
                source.BindingDictionaryX(target, true);
                setSource.BindingSetX(set, true);
            }
            else
            {
                source.BindingDictionary(target, true);
                setSource.BindingSet(set, true);
            }
        }
    }

    [TestMethod]
    public void F13_BatchAddPreservesOrder()
    {
        var source = new RangeSource<int>([1, 4]);
        var target = new List<int>(source);
        source.BindingList(target);
        try
        {
            source.AddRange(1, [2, 3]);
            CollectionAssert.AreEqual(source.ToArray(), target);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F13_BatchRemoveUsesOriginalRange()
    {
        var source = new RangeSource<int>([1, 2, 3, 4]);
        var target = new List<int>(source);
        source.BindingList(target);
        try
        {
            source.RemoveRange(1, 2);
            CollectionAssert.AreEqual(source.ToArray(), target);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F13_UnknownIndexAndResetResnapshot()
    {
        var source = new RangeSource<int>([1, 2]);
        var target = new List<int>(source);
        source.BindingList(target);
        try
        {
            source.AddRange(1, [3, 4], true);
            CollectionAssert.AreEqual(source.ToArray(), target);
            source.Reset([9, 8]);
            CollectionAssert.AreEqual(source.ToArray(), target);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F14_MoveAndUnequalReplace()
    {
        var source = new RangeSource<int>([1, 2, 3, 4]);
        var target = new List<int>(source);
        source.BindingList(target);
        try
        {
            source.MoveRange(0, 2, 2);
            CollectionAssert.AreEqual(source.ToArray(), target);
            source.ReplaceRange(1, 1, [8, 9]);
            CollectionAssert.AreEqual(source.ToArray(), target);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F14_DictionaryReplaceRemovesOldKey()
    {
        var source = new RangeSource<KeyValuePair<string, int>>([new("a", 1)]);
        var target = new Dictionary<string, int> { ["a"] = 1 };
        source.BindingDictionary(target);
        try
        {
            source.ReplaceRange(0, 1, [new("b", 2)]);
            Assert.AreEqual(1, target.Count);
            Assert.AreEqual(2, target["b"]);
        }
        finally
        {
            source.BindingDictionary(target, true);
        }
    }

    [TestMethod]
    public void F14_SetProjectionCountsDuplicatesAndReplacements()
    {
        var source = new RangeSource<string>(["a", "A", "b"]);
        var target = new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
        source.BindingSet(target);
        try
        {
            source.RemoveRange(0, 1);
            Assert.IsTrue(target.SetEquals(source));
            source.ReplaceRange(0, 1, ["c"]);
            Assert.IsTrue(target.SetEquals(source));
            source.Reset(["d", "D"]);
            Assert.IsTrue(target.SetEquals(source));
        }
        finally
        {
            source.BindingSet(target, true);
        }
    }

    [TestMethod]
    public void F15_CustomListRemovesByIndex()
    {
        var source = new ObservableList<int>([1, 2, 1]);
        var target = new List<int>(source);
        source.BindingListX(target);
        try
        {
            source.RemoveAt(2);
            CollectionAssert.AreEqual(source.ToArray(), target);
        }
        finally
        {
            source.BindingListX(target, true);
        }
    }

    [TestMethod]
    public void F17_MixedGenericTargetsDetachActualHandlers()
    {
        var source = new RangeSource<string>(["a"]);
        var strings = new List<string>(source);
        var objects = new List<object>(source);
        source.BindingList(strings);
        source.BindingList(objects);
        try
        {
            source.AddRange(1, ["b"]);
            CollectionAssert.AreEqual(source.ToArray(), strings);
            CollectionAssert.AreEqual(source.Cast<object>().ToArray(), objects);
        }
        finally
        {
            source.BindingList(strings, true);
            source.BindingList(objects, true);
        }
        Assert.AreEqual(0, source.Subscribers);
        source.AddRange(source.Count, ["c"]);
        Assert.AreEqual(2, objects.Count);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void F18_GrowthDoesNotShrinkReservedSetCapacity(bool custom)
    {
        var source = new ObservableSet<int>();
        var target = new HashSet<int>(4096);
        var capacity = target.EnsureCapacity(0);
        if (custom)
            source.BindingSetX(target);
        else
            source.BindingSet(target);
        try
        {
            source.Add(1);
            source.UnionWith([2, 3]);
            Assert.AreEqual(capacity, target.EnsureCapacity(0));
        }
        finally
        {
            if (custom)
                source.BindingSetX(target, true);
            else
                source.BindingSet(target, true);
        }
    }

    [TestMethod]
    public void F16_DuplicateSelfAndCrossProtocolCyclesRejectedBeforeMutation()
    {
        var first = new ObservableList<int>();
        var second = new ObservableList<int>();
        var third = new ObservableList<int>();
        first.BindingList(second);
        second.BindingListX(third);
        try
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => first.BindingList(second));
            Assert.ThrowsExactly<InvalidOperationException>(() => first.BindingListX(second));
            Assert.ThrowsExactly<InvalidOperationException>(() => first.BindingList(first));
            Assert.ThrowsExactly<InvalidOperationException>(() => third.BindingList(first));
        }
        finally
        {
            first.BindingList(second, true);
            first.BindingListX(second, true);
            second.BindingListX(third, true);
            first.BindingList(first, true);
            third.BindingList(first, true);
        }
    }

    [TestMethod]
    public void F16_UnknownUnbindDoesNotSubscribe()
    {
        var source = new RangeSource<int>([]);
        var target = new List<int>();
        source.BindingList(target, true);
        source.BindingList(target, true);
        Assert.AreEqual(0, source.SubscriptionAdds);
        Assert.AreEqual(0, source.Subscribers);
    }

    [TestMethod]
    public void F16_InitialFailureDetachesAndAllowsRetry()
    {
        var source = new RangeSource<int>([1]);
        var target = new ThrowingList { Fail = true };
        try
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingList(target));
            Assert.AreEqual(0, source.Subscribers);
            target.Fail = false;
            source.BindingList(target);
            CollectionAssert.AreEqual(new[] { 1 }, target.ToArray());
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F16_PropagationFailureDetachesFailedTargetButUpdatesHealthyTarget()
    {
        var source = new RangeSource<int>([]);
        var bad = new ThrowingList();
        var good = new List<int>();
        source.BindingList(bad);
        source.BindingList(good);
        bad.Fail = true;
        try
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => source.AddRange(0, [1]));
            CollectionAssert.AreEqual(new[] { 1 }, good);
            bad.Fail = false;
            source.AddRange(1, [2]);
            Assert.AreEqual(0, bad.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, good);
        }
        finally
        {
            source.BindingList(bad, true);
            source.BindingList(good, true);
        }
    }

    [TestMethod]
    public void F16_TargetMutationIsOneWayAndSourceCancellationDoesNotPropagate()
    {
        var source = new ObservableList<int>();
        var target = new ObservableList<int>();
        source.BindingListX(target);
        try
        {
            target.Add(9);
            Assert.AreEqual(0, source.Count);
            target.Clear();
            source.ListChanging += (_, _) => throw new OperationCanceledException("Cancelled.");
            Assert.ThrowsExactly<OperationCanceledException>(() => source.Add(1));
            Assert.AreEqual(0, source.Count);
            Assert.AreEqual(0, target.Count);
        }
        finally
        {
            source.BindingListX(target, true);
        }
    }

    [TestMethod]
    public void F16_TargetCancellationDetachesAndPreservesCommittedSource()
    {
        var source = new ObservableList<int>();
        var target = new ObservableList<int>();
        source.BindingListX(target);
        target.ListChanging += (_, _) => throw new OperationCanceledException("Cancelled.");
        try
        {
            Assert.ThrowsExactly<OperationCanceledException>(() => source.Add(1));
            Assert.AreEqual(1, source.Count);
            source.Add(2);
            Assert.AreEqual(0, target.Count);
        }
        finally
        {
            source.BindingListX(target, true);
        }
    }

    [TestMethod]
    public void F16_RegistrationDuringDispatchUsesSnapshot()
    {
        var source = new ObservableList<int>();
        var first = new ObservableList<int>();
        var second = new List<int>();
        source.BindingListX(first);
        first.ListChanged += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingListX(second));
        try
        {
            source.Add(1);
            Assert.AreEqual(0, second.Count);
            CollectionAssert.AreEqual(new[] { 1 }, first.ToArray());
        }
        finally
        {
            source.BindingListX(first, true);
            source.BindingListX(second, true);
        }
    }

    [TestMethod]
    public void F16_TokenDisposalIsIdempotentAndReleasesReferences()
    {
        var source = new RangeSource<int>([]);
        var target = new List<int>();
        var method = typeof(ObservableCollectionExtensions)
            .GetMethods()
            .Single(m => m.Name == nameof(ObservableCollectionExtensions.BindingList))
            .MakeGenericMethod(typeof(int));
        var token = method.Invoke(null, [source, target, false]);
        try
        {
            Assert.IsInstanceOfType<IDisposable>(token);
            ((IDisposable)token).Dispose();
            ((IDisposable)token).Dispose();
            Assert.AreEqual(0, source.Subscribers);
            source.AddRange(0, [1]);
            Assert.AreEqual(0, target.Count);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F16_AbandonedBindingHasNoStaticRoot()
    {
        var references = AbandonBinding();
        for (var i = 0; i < 5 && references.Any(r => r.IsAlive); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        Assert.IsTrue(references.All(r => r.IsAlive is false));
    }

    [TestMethod]
    public void F17_InitialConversionFailureDoesNotModifyTargetOrSubscribe()
    {
        var source = new RangeSource<object>([1, "invalid"]);
        var target = new List<int> { 99 };
        try
        {
            Assert.ThrowsExactly<InvalidCastException>(() => source.BindingList(target));
            CollectionAssert.AreEqual(new[] { 99 }, target);
            Assert.AreEqual(0, source.Subscribers);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F17_EventConversionIsStagedBeforeTargetWrites()
    {
        var source = new RangeSource<object>([]);
        var target = new List<int>();
        source.BindingList(target);
        try
        {
            Assert.ThrowsExactly<InvalidCastException>(() => source.AddRange(0, [1, "invalid"]));
            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(0, source.Subscribers);
        }
        finally
        {
            source.BindingList(target, true);
        }
    }

    [TestMethod]
    public void F16_BindingDuringChangingIsRejected()
    {
        var source = new ObservableList<int>();
        var first = new List<int>();
        var added = new List<int>();
        source.BindingListX(first);
        source.ListChanging += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingListX(added));
        try
        {
            source.Add(1);
            CollectionAssert.AreEqual(new[] { 1 }, first);
            Assert.AreEqual(0, added.Count);
        }
        finally
        {
            source.BindingListX(first, true);
            source.BindingListX(added, true);
        }
    }

    [TestMethod]
    public void F16_UnbindingDuringChangingKeepsCurrentSnapshotOnly()
    {
        var source = new ObservableList<int>();
        var target = new List<int>();
        source.BindingListX(target);
        source.ListChanging += (_, _) => source.BindingListX(target, true);
        source.Add(1);
        source.Add(2);
        CollectionAssert.AreEqual(new[] { 1 }, target);
    }

    [TestMethod]
    public void F16_TargetChangedFailureRetainsCommitAndDetaches()
    {
        var source = new ObservableList<int>();
        var target = new ObservableList<int>();
        source.BindingListX(target);
        target.ListChanged += (_, _) => throw new InvalidOperationException("Observer failed.");
        try
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => source.Add(1));
            CollectionAssert.AreEqual(new[] { 1 }, source.ToArray());
            CollectionAssert.AreEqual(new[] { 1 }, target.ToArray());
            source.Add(2);
            CollectionAssert.AreEqual(new[] { 1 }, target.ToArray());
        }
        finally
        {
            source.BindingListX(target, true);
        }
    }

    [TestMethod]
    public void F14_SetProjectionSupportsNullAndComparerFallback()
    {
        var source = new RangeSource<string?>([null, null, "a"]);
        var target = new HashSet<string?>(source);
        source.BindingSet(target);
        try
        {
            source.RemoveRange(0, 1);
            Assert.IsTrue(target.SetEquals(source));
            source.RemoveRange(0, 1);
            Assert.IsTrue(target.SetEquals(source));
        }
        finally
        {
            source.BindingSet(target, true);
        }
        var words = new RangeSource<string>(["a", "A"]);
        var sorted = new SortedSet<string>(words, StringComparer.OrdinalIgnoreCase);
        words.BindingSet(sorted);
        try
        {
            words.RemoveRange(0, 1);
            Assert.IsTrue(sorted.SetEquals(words));
        }
        finally
        {
            words.BindingSet(sorted, true);
        }
    }

    [TestMethod]
    public void F16_BindingFailureDuringChangingDoesNotPoisonSourceScope()
    {
        var source = new ObservableList<int>([1]);
        var good = new List<int>();
        var bad = new ThrowingList { Fail = true };
        using var binding = source.BindingListX(good);
        ObservableListChangingEventHandler<int> handler = (_, _) => source.BindingListX(bad);
        source.ListChanging += handler;
        try
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => source.Add(2));
            Assert.AreEqual(1, source.Count);
        }
        finally
        {
            source.ListChanging -= handler;
            source.BindingListX(bad, true);
        }
        source.Add(3);
        CollectionAssert.AreEqual(new[] { 1, 3 }, good);
    }

    [TestMethod]
    public void F16_DisposedTokenDoesNotRetainEitherEndpoint()
    {
        var (token, references) = DisposeBinding();
        for (var i = 0; i < 5 && references.Any(r => r.IsAlive); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        Assert.IsTrue(references.All(r => r.IsAlive is false));
        GC.KeepAlive(token);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (IDisposable Token, WeakReference[] References) DisposeBinding()
    {
        var source = new RangeSource<int>([]);
        var target = new List<int>();
        var token = source.BindingList(target);
        token.Dispose();
        return (token, [new(source), new(target)]);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void F16_AllBindingTokensStopPropagation(bool custom)
    {
        var listSource = new ObservableList<int>([1]);
        var list = new List<int>();
        var dictionarySource = new ObservableDictionary<int, int> { [1] = 1 };
        var dictionary = new Dictionary<int, int>();
        var setSource = new ObservableSet<int>([1]);
        var set = new HashSet<int>();
        IDisposable[] tokens = custom
            ?
            [
                listSource.BindingListX(list),
                dictionarySource.BindingDictionaryX(dictionary),
                setSource.BindingSetX(set),
            ]
            :
            [
                listSource.BindingList(list),
                dictionarySource.BindingDictionary(dictionary),
                setSource.BindingSet(set),
            ];
        foreach (var token in tokens)
        {
            token.Dispose();
            token.Dispose();
        }
        listSource.Add(2);
        dictionarySource.Add(2, 2);
        setSource.Add(2);
        CollectionAssert.AreEqual(new[] { 1 }, list);
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(set.SetEquals([1]));
    }

    [TestMethod]
    public void F16_EventAddFailureCleansSubscriptionAndRegistry()
    {
        var source = new RangeSource<int>([]) { FailSubscription = true };
        var target = new List<int>();
        Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingList(target));
        Assert.AreEqual(0, source.Subscribers);
        source.FailSubscription = false;
        using var token = source.BindingList(target);
        source.AddRange(0, [1]);
        CollectionAssert.AreEqual(new[] { 1 }, target);
    }

    [TestMethod]
    public void F16_MultipleFailuresAreAggregatedAfterHealthyTargetsSynchronize()
    {
        var source = new RangeSource<int>([]);
        var first = new ThrowingList();
        var second = new ThrowingList();
        var good = new List<int>();
        using var a = source.BindingList(first);
        using var b = source.BindingList(second);
        using var c = source.BindingList(good);
        first.Fail = second.Fail = true;
        var error = Assert.ThrowsExactly<AggregateException>(() => source.AddRange(0, [1]));
        Assert.AreEqual(2, error.InnerExceptions.Count);
        CollectionAssert.AreEqual(new[] { 1 }, good);
        source.AddRange(1, [2]);
        CollectionAssert.AreEqual(new[] { 1, 2 }, good);
    }

    [TestMethod]
    public void F16_IdentityIgnoresTargetValueEquality()
    {
        var source = new RangeSource<int>([]);
        var first = new EqualList();
        var second = new EqualList();
        using var a = source.BindingList(first);
        using var b = source.BindingList(second);
        source.AddRange(0, [1]);
        CollectionAssert.AreEqual(new[] { 1 }, first.ToArray());
        CollectionAssert.AreEqual(new[] { 1 }, second.ToArray());
    }

    [TestMethod]
    public void F16_InitializationReentrancyFailsWithoutLeakingSubscription()
    {
        var source = new RangeSource<int>([1]);
        var target = new ObservableList<int>();
        target.ListChanging += (_, _) => source.AddRange(source.Count, [2]);
        Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingList(target));
        Assert.AreEqual(0, source.Subscribers);
    }

    [TestMethod]
    public void F16_KeepLiveSourceAfterUnbindReleasesTarget()
    {
        var source = new RangeSource<int>([]);
        var target = BindAndUnbind(source);
        Assert.AreEqual(0, source.Subscribers);
        for (var i = 0; i < 5 && target.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        Assert.IsFalse(target.IsAlive);
        source.AddRange(0, [1]);
        GC.KeepAlive(source);
    }

    [TestMethod]
    public void F16_UnbindDuringChangingIsAllowedButRebindIsRejected()
    {
        var source = new ObservableList<int>([1]);
        var target = new List<int>();
        source.BindingListX(target);
        source.ListChanging += (_, _) =>
        {
            source.BindingListX(target, true);
            Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingListX(target));
        };
        try
        {
            source.Add(2);
            source.Add(3);
            CollectionAssert.AreEqual(new[] { 1, 2 }, target);
        }
        finally
        {
            source.BindingListX(target, true);
        }
    }

    [TestMethod]
    public void F14_CustomDictionaryReplacementCanChangeKeys()
    {
        var source = new DictionarySource();
        var target = new Dictionary<string, int>();
        using var token = source.BindingDictionaryX(target);
        source.Rename();
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(2, target["b"]);
    }

    [TestMethod]
    public void F14_StandardDictionaryResetAndMove()
    {
        var source = new RangeSource<KeyValuePair<string, int>>([new("a", 1), new("b", 2)]);
        var target = new Dictionary<string, int>();
        using var token = source.BindingDictionary(target);
        source.MoveRange(0, 1, 1);
        Assert.AreEqual(2, target.Count);
        source.Reset([new("c", 3)]);
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(3, target["c"]);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference BindAndUnbind(RangeSource<int> source)
    {
        var target = new List<int>();
        source.BindingList(target);
        source.BindingList(target, true);
        return new(target);
    }

    private sealed class EqualList : Collection<int>
    {
        public override bool Equals(object? obj) => obj is EqualList;

        public override int GetHashCode() => 0;
    }

    private sealed class DictionarySource
        : ObservableDictionary<string, int>,
            INotifyDictionaryChanged<string, int>
    {
        public DictionarySource() => Add("a", 1);

        public new event ObservableDictionaryChangedEventHandler<string, int>? DictionaryChanged;

        public void Rename()
        {
            var old = new KeyValuePair<string, int>("a", this["a"]);
            Remove("a");
            Add("b", 2);
            DictionaryChanged?.Invoke(this, new(DictionaryChangeAction.Replace, new("b", 2), old));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] AbandonBinding()
    {
        var source = new RangeSource<int>([]);
        var target = new List<int>();
        source.BindingList(target);
        return [new(source), new(target)];
    }

    private static void BindList(
        ObservableList<int> source,
        IList<int> target,
        bool custom,
        bool remove = false
    )
    {
        if (custom)
            source.BindingListX(target, remove);
        else
            source.BindingList(target, remove);
    }

    private sealed class ThrowingList : Collection<int>
    {
        public bool Fail { get; set; }

        protected override void InsertItem(int index, int item)
        {
            if (Fail)
                throw new InvalidOperationException("Target rejected insertion.");
            base.InsertItem(index, item);
        }
    }

    private sealed class RangeSource<T>(IEnumerable<T> items)
        : IEnumerable<T>,
            INotifyCollectionChanged
    {
        private readonly List<T> _items = new(items);
        private NotifyCollectionChangedEventHandler? _changed;
        public int Count => _items.Count;
        public int Subscribers => _changed?.GetInvocationList().Length ?? 0;
        public int SubscriptionAdds { get; private set; }
        public bool FailSubscription { get; set; }
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                SubscriptionAdds++;
                _changed += value;
                if (FailSubscription)
                    throw new InvalidOperationException("Subscription failed.");
            }
            remove { _changed -= value; }
        }

        public void AddRange(int index, T[] items, bool unknown = false)
        {
            _items.InsertRange(index, items);
            _changed?.Invoke(
                this,
                new(NotifyCollectionChangedAction.Add, items, unknown ? -1 : index)
            );
        }

        public void RemoveRange(int index, int count)
        {
            var removed = _items.GetRange(index, count).ToArray();
            _items.RemoveRange(index, count);
            _changed?.Invoke(this, new(NotifyCollectionChangedAction.Remove, removed, index));
        }

        public void ReplaceRange(int index, int count, T[] replacement)
        {
            var old = _items.GetRange(index, count).ToArray();
            _items.RemoveRange(index, count);
            _items.InsertRange(index, replacement);
            _changed?.Invoke(
                this,
                new(NotifyCollectionChangedAction.Replace, replacement, old, index)
            );
        }

        public void MoveRange(int oldIndex, int newIndex, int count)
        {
            var moved = _items.GetRange(oldIndex, count).ToArray();
            _items.RemoveRange(oldIndex, count);
            _items.InsertRange(newIndex, moved);
            _changed?.Invoke(
                this,
                new(NotifyCollectionChangedAction.Move, moved, newIndex, oldIndex)
            );
        }

        public void Reset(T[] items)
        {
            _items.Clear();
            _items.AddRange(items);
            _changed?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
