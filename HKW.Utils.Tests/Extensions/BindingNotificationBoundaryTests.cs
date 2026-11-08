using System.Collections.Specialized;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Extensions;

[TestClass]
[DoNotParallelize]
public class BindingNotificationBoundaryTests
{
    [TestMethod]
    public void OrdinarySubscriptionsRemainAllowedWhileNewBindingsAreRejected()
    {
        var source = new ObservableList<int>();
        var target = new List<int> { 99 };
        var originalCalls = 0;
        var addedCalls = 0;
        ObservableListChangedEventHandler<int> original = (_, _) => originalCalls++;
        ObservableListChangedEventHandler<int> added = (_, _) => addedCalls++;
        source.ListChanged += original;
        var first = true;
        source.ListChanging += (_, _) =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => source.BindingList(target));
            if (first)
            {
                first = false;
                source.ListChanged -= original;
                source.ListChanged += added;
            }
        };
        source.Add(1);
        Assert.AreEqual(1, originalCalls);
        Assert.AreEqual(0, addedCalls);
        source.Add(2);
        Assert.AreEqual(1, originalCalls);
        Assert.AreEqual(1, addedCalls);
        CollectionAssert.AreEqual(new[] { 99 }, target);
    }

    [TestMethod]
    public void CancellationReleasesNotificationStateSoFirstBindingCanBeRetried()
    {
        var source = new ObservableList<int>([1]);
        var target = new List<int> { 99 };
        ObservableListChangingEventHandler<int> handler = (_, _) => source.BindingList(target);
        source.ListChanging += handler;
        Assert.ThrowsExactly<InvalidOperationException>(() => source.Add(2));
        source.ListChanging -= handler;
        CollectionAssert.AreEqual(new[] { 1 }, source.ToArray());
        CollectionAssert.AreEqual(new[] { 99 }, target);
        using var token = source.BindingList(target);
        source.Add(3);
        CollectionAssert.AreEqual(new[] { 1, 3 }, target);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void ReadOnlyForwardingSourceRejectsFirstBindingDuringChanging(int kind)
    {
        INotifyCollectionChanged view;
        Action<Action> subscribe;
        Action mutate;
        if (kind == 0)
        {
            var source = new ObservableList<int>([1]);
            view = new ReadOnlyObservableList<int>(source);
            subscribe = callback => source.ListChanging += (_, _) => callback();
            mutate = () => source.Add(2);
        }
        else if (kind == 2)
        {
            var source = new ObservableSet<int>([1]);
            view = new ReadOnlyObservableSet<int>(source);
            subscribe = callback => source.SetChanging += (_, _) => callback();
            mutate = () => source.Add(2);
        }
        else
        {
            var source = new ObservableDictionary<int, int> { [1] = 1 };
            view = kind switch
            {
                1 => new ReadOnlyObservableDictionary<int, int>(source),
                3 => source.ObservableKeys,
                _ => source.ObservableValues,
            };
            subscribe = callback => source.DictionaryChanging += (_, _) => callback();
            mutate = () => source.Add(2, 2);
        }
        var target = new List<object> { 99 };
        IDisposable? token = null;
        subscribe(() =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => token = view.BindingList(target));
            CollectionAssert.AreEqual(new object[] { 99 }, target);
        });
        try
        {
            mutate();
        }
        finally
        {
            token?.Dispose();
            (view as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    [DataRow(false, false, false)]
    [DataRow(false, false, true)]
    [DataRow(false, true, false)]
    [DataRow(false, true, true)]
    [DataRow(true, false, false)]
    [DataRow(true, false, true)]
    [DataRow(true, true, false)]
    [DataRow(true, true, true)]
    public void List_NewBindingDuringNotificationIsRejectedWithoutChangingTargets(
        bool custom,
        bool existing,
        bool changed
    )
    {
        var source = new ObservableList<int>([1]);
        var good = new List<int>();
        var target = new List<int> { 99 };
        using var previous = existing ? Bind(good) : null;
        IDisposable? created = null;
        void Check()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => created = Bind(target));
            CollectionAssert.AreEqual(new[] { 99 }, target);
        }
        if (changed)
            source.ListChanged += (_, _) => Check();
        else
            source.ListChanging += (_, _) => Check();
        try
        {
            source.Add(2);
            CollectionAssert.AreEqual(new[] { 99 }, target);
            if (existing)
                CollectionAssert.AreEqual(source.ToArray(), good);
        }
        finally
        {
            created?.Dispose();
        }
        IDisposable Bind(List<int> list) =>
            custom ? source.BindingListX(list) : source.BindingList(list);
    }

    [TestMethod]
    [DataRow(false, false, false)]
    [DataRow(false, false, true)]
    [DataRow(false, true, false)]
    [DataRow(false, true, true)]
    [DataRow(true, false, false)]
    [DataRow(true, false, true)]
    [DataRow(true, true, false)]
    [DataRow(true, true, true)]
    public void Dictionary_NewBindingDuringNotificationIsRejectedWithoutChangingTargets(
        bool custom,
        bool existing,
        bool changed
    )
    {
        var source = new ObservableDictionary<int, int> { [1] = 1 };
        var good = new Dictionary<int, int>();
        var target = new Dictionary<int, int> { [99] = 99 };
        using var previous = existing ? Bind(good) : null;
        IDisposable? created = null;
        void Check()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => created = Bind(target));
            Assert.AreEqual(1, target.Count);
            Assert.AreEqual(99, target[99]);
        }
        if (changed)
            source.DictionaryChanged += (_, _) => Check();
        else
            source.DictionaryChanging += (_, _) => Check();
        try
        {
            source.Add(2, 2);
            Assert.AreEqual(1, target.Count);
            Assert.AreEqual(99, target[99]);
            if (existing)
                CollectionAssert.AreEqual(source.ToArray(), good.ToArray());
        }
        finally
        {
            created?.Dispose();
        }
        IDisposable Bind(Dictionary<int, int> dictionary) =>
            custom ? source.BindingDictionaryX(dictionary) : source.BindingDictionary(dictionary);
    }

    [TestMethod]
    [DataRow(false, false, false)]
    [DataRow(false, false, true)]
    [DataRow(false, true, false)]
    [DataRow(false, true, true)]
    [DataRow(true, false, false)]
    [DataRow(true, false, true)]
    [DataRow(true, true, false)]
    [DataRow(true, true, true)]
    public void Set_NewBindingDuringNotificationIsRejectedWithoutChangingTargets(
        bool custom,
        bool existing,
        bool changed
    )
    {
        var source = new ObservableSet<int>([1]);
        var good = new HashSet<int>();
        var target = new HashSet<int> { 99 };
        using var previous = existing ? Bind(good) : null;
        IDisposable? created = null;
        void Check()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => created = Bind(target));
            Assert.IsTrue(target.SetEquals([99]));
        }
        if (changed)
            source.SetChanged += (_, _) => Check();
        else
            source.SetChanging += (_, _) => Check();
        try
        {
            source.Add(2);
            Assert.IsTrue(target.SetEquals([99]));
            if (existing)
                Assert.IsTrue(good.SetEquals(source));
        }
        finally
        {
            created?.Dispose();
        }
        IDisposable Bind(HashSet<int> set) =>
            custom ? source.BindingSetX(set) : source.BindingSet(set);
    }
}
