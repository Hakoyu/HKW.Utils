using System.ComponentModel;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class I18nObserverTests
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr");

    internal sealed class Model<TKey>(TKey key) : INotifyPropertyChanged
        where TKey : notnull
    {
        public TKey Key { get; set; } = key;
        public TKey OtherKey { get; set; } = key;
        public List<string> Notices { get; } = [];
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Raise(string? name) => PropertyChanged?.Invoke(this, new(name));

        public void Notify(string name)
        {
            Notices.Add(name);
            Raise(name);
        }
    }

    private static I18nObserver<Model<TKey>, TKey, string> Observe<TKey>(
        ObservableI18nResource<TKey, string> resource,
        Model<TKey> source
    )
        where TKey : notnull
    {
        var observer = new I18nObserver<Model<TKey>, TKey, string>(
            resource,
            source,
            (s, n) => s.Notify(n)
        );
        observer.Add(new(s => s.Key, "Text", "Caption"));
        return observer;
    }

    [TestMethod]
    public void Recording_IsUniqueAndDisposeDetachesEverything()
    {
        using var r = new ObservableI18nResource<int, string>("r", En) { RecordObserver = true };
        var s = new Model<int>(1);
        var o = Observe(r, s);
        Assert.AreEqual(1, r.Observers.Count);
        o.Dispose();
        o.Dispose();
        Assert.AreEqual(0, r.Observers.Count);
        r.SetData(1, "new");
        s.Key = 2;
        s.Raise(nameof(s.Key));
        Assert.AreEqual(0, s.Notices.Count);
        Assert.AreEqual(0, o.Count);
    }

    [TestMethod]
    public void DefaultKeys_AreNotUpdatingSentinels()
    {
        Check(0);
        Check(Guid.Empty);
        static void Check<TKey>(TKey key)
            where TKey : notnull
        {
            using var r = new ObservableI18nResource<TKey, string>("r", En);
            var s = new Model<TKey>(key);
            using var o = Observe(r, s);
            r.SetData(key, "new");
            Assert.AreSequenceEqual(new[] { "Text", "Caption" }, s.Notices);
        }
    }

    [TestMethod]
    public void InternalWrites_NotifyAllSameKeyRegistrationsAndObservers()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(0);
        var other = new Model<int>(0);
        using var o = Observe(r, s);
        using var o2 = Observe(r, other);
        o.Add(new(x => x.OtherKey, "OtherText"));
        Assert.IsTrue(o.SetData(0, "new", keyPropertyName: "Key"));
        Assert.AreSequenceEqual(new[] { "Text", "Caption", "OtherText" }, s.Notices);
        Assert.AreEqual(2, other.Notices.Count);
    }

    [TestMethod]
    public void Validation_PrecedesWriteAndCallerExpressionIsResolved()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(1);
        using var o = Observe(r, s);
        r.SetData(1, "old");
        s.Notices.Clear();
        Assert.IsFalse(o.SetData(1, "bad", keyPropertyName: null));
        Assert.IsFalse(o.SetData(1, "bad", keyPropertyName: "missing"));
        Assert.IsFalse(o.SetData(2, "bad", keyPropertyName: "Key"));
        Assert.AreEqual("old", r.GetData(1));
        Assert.IsFalse(r.ContainsData(2));
        Assert.AreEqual(0, s.Notices.Count);
        Assert.IsTrue(o.SetData(s.Key, "new"));
        Assert.AreEqual("new", r.GetData(1));
        Assert.IsFalse(o.SetData(s.Key, "bad", Fr));
        Assert.AreEqual("new", r.GetData(1));
    }

    [TestMethod]
    public void Defaults_ForwardCallerAndAllowRealNoCulture()
    {
        using var r = new ObservableI18nResource<int, string>("r", En)
        {
            GetDefaultValue = (_, _) => "resource",
        };
        var s = new Model<int>(0);
        using var o = Observe(r, s);
        var calls = 0;
        string Fallback(int key, CultureInfo? culture)
        {
            calls++;
            return "caller";
        }
        Assert.AreEqual("caller", o.GetDataOrDefault(0, getDefaultValue: Fallback));
        r.SetData(0, null!);
        Assert.IsNull(o.GetDataOrDefault(0, getDefaultValue: Fallback));
        Assert.AreEqual(1, calls);
        r.CurrentCulture = null;
        Assert.ThrowsExactly<InvalidOperationException>(() => o.GetData(0));
        Assert.AreEqual(
            "caller",
            o.GetDataOrDefault(
                0,
                getDefaultValue: (key, culture) =>
                {
                    Assert.IsNull(culture);
                    return "caller";
                }
            )
        );
        Assert.IsFalse(o.SetData(0, "bad", keyPropertyName: "Key"));
        Assert.IsNull(r.GetData(0, En));
    }

    [TestMethod]
    public void SwitchingKeys_DoesNotRenameOrOverwriteSharedTranslations()
    {
        using var r = new ObservableI18nResource<string, string>("r", En);
        r.SetData("a", "A");
        r.SetData("b", "B");
        var s = new Model<string>("a");
        var t = new Model<string>("a");
        using var o = Observe(r, s);
        using var p = Observe(r, t);
        s.Key = "b";
        s.Raise(nameof(s.Key));
        Assert.AreEqual("A", r.GetData("a"));
        Assert.AreEqual("B", r.GetData("b"));
        r.SetData("a", "AA");
        Assert.AreEqual(2, s.Notices.Count);
        Assert.AreEqual(2, t.Notices.Count);
    }

    [TestMethod]
    public void SharedRegistration_HasIndependentObserverKeys()
    {
        using var r = new ObservableI18nResource<int, string>("r", En) { RecordObserver = true };
        var a = new Model<int>(1);
        var b = new Model<int>(2);
        using var first = new I18nObserver<Model<int>, int, string>(r, a, (s, n) => s.Notify(n));
        using var second = new I18nObserver<Model<int>, int, string>(r, b, (s, n) => s.Notify(n));
        var registration = new ObservedPropertyRegistration<Model<int>, int>(s => s.Key, "Text");
        first.Add(registration);
        second.Add(registration);
        r.SetData(1, "one");
        r.SetData(2, "two");
        r.SetData(3, "three");
        Assert.AreEqual(1, a.Notices.Count);
        Assert.AreEqual(1, b.Notices.Count);
        r.ClearUnobservedData();
        Assert.AreEqual(2, r.DatasByKey.Count);
        Assert.AreEqual(1, first.Registrations.Single().OldKey);
        Assert.AreEqual(2, second.Registrations.Single().OldKey);
    }

    [TestMethod]
    public void RecordingToggle_IsProspectiveAndZeroObserversClearAll()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(1);
        using var old = Observe(r, s);
        r.SetData(1, "one");
        r.ClearUnobservedData();
        Assert.AreEqual(1, r.DatasByKey.Count);
        r.RecordObserver = true;
        Assert.AreEqual(0, r.Observers.Count);
        r.ClearUnobservedData();
        Assert.AreEqual(0, r.DatasByKey.Count);
        using var current = Observe(r, new Model<int>(2));
        Assert.AreEqual(1, r.Observers.Count);
        r.RecordObserver = false;
        Assert.AreEqual(0, r.Observers.Count);
        r.RecordObserver = true;
        Assert.AreEqual(0, r.Observers.Count);
        r.SetData(2, "two");
        current.Clear();
        r.ClearUnobservedData();
        Assert.AreEqual(0, r.DatasByKey.Count);
    }

    [TestMethod]
    public void ResourceSwitch_RefreshesAndDetachesOldResource()
    {
        using var a = new ObservableI18nResource<int, string>("a", En) { RecordObserver = true };
        using var b = new ObservableI18nResource<int, string>("b", En) { RecordObserver = true };
        var s = new Model<int>(1);
        using var o = Observe(a, s);
        o.Resource = b;
        Assert.AreEqual(0, a.Observers.Count);
        Assert.AreEqual(1, b.Observers.Count);
        Assert.AreEqual(2, s.Notices.Count);
        s.Notices.Clear();
        a.SetData(1, "a");
        Assert.AreEqual(0, s.Notices.Count);
        b.SetData(1, "b");
        Assert.AreEqual(2, s.Notices.Count);
    }

    [TestMethod]
    public void ResourceOperations_FreezeTargetsAndRejectAllObserverWritesEvenNoOps()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        r.SetData(1, "old");
        var s = new Model<int>(1);
        using var o = Observe(r, s);
        var registration = o.Single();
        var readOnly = o.Registrations;
        var changingCalls = 0;
        var originalChanged = 0;
        var lateChanged = 0;
        CultureDataChangedEventHandler<int, string> original = (_, _) => originalChanged++;
        CultureDataChangedEventHandler<int, string> late = (_, _) => lateChanged++;
        r.CultureDataChanged += original;
        r.DatasByKey[1].ListChanging += (_, _) =>
        {
            changingCalls++;
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                o.SetData(1, "old", keyPropertyName: "Key")
            );
            Assert.ThrowsExactly<InvalidOperationException>(() => o.Resource = r);
            Assert.ThrowsExactly<InvalidOperationException>(() => o.TryAdd(registration));
            Assert.ThrowsExactly<InvalidOperationException>(() => o.Add(registration));
            Assert.ThrowsExactly<InvalidOperationException>(() => o.Remove("missing"));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                ((ICollection<ObservedPropertyRegistration<Model<int>, int>>)o).Remove(registration)
            );
            Assert.ThrowsExactly<InvalidOperationException>(o.Clear);
            Assert.ThrowsExactly<InvalidOperationException>(o.Dispose);
            Assert.ThrowsExactly<InvalidOperationException>(() => s.Raise(nameof(s.Key)));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                r.RecordObserver = r.RecordObserver
            );
            Assert.ThrowsExactly<InvalidOperationException>(() => Observe(r, new Model<int>(1)));
            registration.TargetPropertyNames.Clear();
            registration.TargetPropertyNames.Add("Next");
            r.CultureDataChanged -= original;
            r.CultureDataChanged += late;
        };
        Assert.IsTrue(o.SetData(s.Key, "new"));
        Assert.AreEqual(1, changingCalls);
        Assert.AreSequenceEqual(new[] { "Text", "Caption" }, s.Notices);
        Assert.AreEqual(1, originalChanged);
        Assert.AreEqual(0, lateChanged);
        s.Notices.Clear();
        r.SetData(1, "newer");
        Assert.AreSequenceEqual(new[] { "Next" }, s.Notices);
        Assert.AreEqual(1, originalChanged);
        Assert.AreEqual(1, lateChanged);
    }

    [TestMethod]
    public void ChangingFailure_DoesNotCommitAndChangedFailureKeepsCompleteState()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        r.SetData(1, "old");
        var source = new Model<int>(1);
        var fail = true;
        var notices = 0;
        using var observer = new I18nObserver<Model<int>, int, string>(
            r,
            source,
            (_, _) =>
            {
                notices++;
                Assert.AreEqual("new", r.GetData(1));
                throw new ApplicationException("Changed failure.");
            }
        );
        observer.Add(new(s => s.Key, "Text"));
        r.DatasByKey[1].ListChanging += (_, _) =>
        {
            if (fail)
                throw new ApplicationException("Changing failure.");
        };
        Assert.ThrowsExactly<ApplicationException>(() => observer.SetData(source.Key, "new"));
        Assert.AreEqual("old", r.GetData(1));
        Assert.AreEqual(0, notices);
        fail = false;
        Assert.ThrowsExactly<ApplicationException>(() => observer.SetData(source.Key, "new"));
        Assert.AreEqual("new", r.GetData(1));
        Assert.AreEqual(1, notices);
        Assert.IsTrue(observer.SetData(source.Key, "new"));
    }

    [TestMethod]
    public void OwnedRowSource_FollowsItsCommittedKeyNotificationWithoutReenteringResource()
    {
        using var r = new ObservableI18nResource<string, string>("r", En) { RecordObserver = true };
        r.SetData("a", "value");
        var row = r.DatasByKey["a"];
        var seen = new List<string>();
        using var observer = new I18nObserver<
            ObservableCultureDataList<string, string>,
            string,
            string
        >(r, row, (source, _) => seen.Add(r.GetData(source.Key)));
        observer.Add(new(source => source.Key, "Text"));
        Assert.IsTrue(r.RenameKey("a", "b"));
        Assert.AreSame(row, r.DatasByKey["b"]);
        Assert.AreEqual("b", observer.Registrations.Single().OldKey);
        Assert.IsGreaterThan(0, seen.Count);
        Assert.IsTrue(seen.All(value => value == "value"));
        r.ClearUnobservedData();
        Assert.IsTrue(r.ContainsData("b"));
        row[0] = "next";
        Assert.AreEqual("next", seen[^1]);
    }

    [TestMethod]
    public void ExternalRename_InvalidatesBothReferencesWithoutRewritingSourceKeys()
    {
        using var r = new ObservableI18nResource<string, string>("r", En)
        {
            GetDefaultValue = (_, _) => "missing",
        };
        r.SetData("a", "A");
        r.SetData("b", "B");
        var a = new Model<string>("a");
        var b = new Model<string>("b");
        using var first = Observe(r, a);
        using var second = Observe(r, b);
        Assert.IsFalse(r.RenameKey("a", "b"));
        Assert.AreEqual(0, a.Notices.Count);
        Assert.IsTrue(r.RenameKey("a", "b", true));
        Assert.AreEqual("a", a.Key);
        Assert.AreEqual("b", b.Key);
        Assert.AreEqual("a", first.Registrations.Single().OldKey);
        Assert.AreEqual("missing", first.GetDataOrDefault(a.Key));
        Assert.AreEqual("A", second.GetData(b.Key));
        Assert.AreEqual(2, a.Notices.Count);
        Assert.AreEqual(2, b.Notices.Count);
        a.Key = "b";
        a.Raise(nameof(a.Key));
        a.Notices.Clear();
        b.Notices.Clear();
        r.SetData("b", "new");
        Assert.AreEqual(2, a.Notices.Count);
        Assert.AreEqual(2, b.Notices.Count);
    }

    [TestMethod]
    public void StructuralInvalidations_IgnoreDefaultPayloadAndUnrelatedCultures()
    {
        using var r = new ObservableI18nResource<int, string>("r", En, [En, Fr]);
        var s = new Model<int>(0);
        using var o = Observe(r, s);
        r.SetData(0, "fr", Fr);
        Assert.AreEqual(2, s.Notices.Count);
        s.Notices.Clear();
        r.SetData(0, "FR", Fr);
        Assert.AreEqual(0, s.Notices.Count);
        r.SetData(0, null!, En);
        Assert.AreEqual(0, s.Notices.Count);
        r.ReplaceData(0, new(0, ["en", "FR"]));
        Assert.AreEqual(2, s.Notices.Count);
        Assert.AreEqual("en", o.GetData(0));
        s.Notices.Clear();
        r.RemoveData(0);
        Assert.AreEqual(2, s.Notices.Count);
        s.Notices.Clear();
        r.AddData(0, new(0));
        Assert.AreEqual(2, s.Notices.Count);
        s.Notices.Clear();
        r.ClearData();
        Assert.AreEqual(2, s.Notices.Count);
        r.CurrentCulture = null;
        Assert.AreEqual(4, s.Notices.Count);
        Assert.AreEqual(
            "none",
            o.GetDataOrDefault(0, getDefaultValue: (_, c) => c is null ? "none" : "culture")
        );
    }

    [TestMethod]
    public void CaseInsensitiveResource_UsesItsComparerForAllKeyMatching()
    {
        using var r = new ObservableI18nResource<string, string>(
            "r",
            En,
            EqualityComparer<string>.Create(
                StringComparer.OrdinalIgnoreCase.Equals,
                StringComparer.OrdinalIgnoreCase.GetHashCode
            )
        )
        {
            RecordObserver = true,
        };
        var s = new Model<string>("KEY");
        using var o = Observe(r, s);
        Assert.IsTrue(o.SetData("key", "value", keyPropertyName: "this.Key"));
        Assert.AreEqual(2, s.Notices.Count);
        r.ClearUnobservedData();
        Assert.IsTrue(r.ContainsData("key"));
        s.Key = "Key";
        s.Raise(nameof(s.Key));
        Assert.AreEqual(2, s.Notices.Count);
        Assert.AreEqual("Key", o.Registrations.Single().OldKey);
    }

    [TestMethod]
    public void EmptyRecordedObserver_DoesNotProtectAnyData()
    {
        using var r = new ObservableI18nResource<int, string>("r", En) { RecordObserver = true };
        using var o = new I18nObserver<Model<int>, int, string>(r, new(0), (_, _) => { });
        r.SetData(0, "value");
        Assert.AreEqual(1, r.Observers.Count);
        r.ClearUnobservedData();
        Assert.AreEqual(0, r.DatasByKey.Count);
        Assert.ThrowsExactly<NotSupportedException>(() =>
            ((ICollection<II18nObserver<int, string>>)r.Observers).Add(o)
        );
    }

    [TestMethod]
    public void ResourceDisposedFirst_ObserverCanDetachAndSwitch()
    {
        var old = new ObservableI18nResource<int, string>("old", En) { RecordObserver = true };
        using var next = new ObservableI18nResource<int, string>("next", En)
        {
            RecordObserver = true,
        };
        var s = new Model<int>(0);
        var o = Observe(old, s);
        old.Dispose();
        o.Resource = next;
        Assert.AreEqual(1, next.Observers.Count);
        next.Dispose();
        o.Dispose();
        o.Dispose();
        s.Key = 1;
        s.Raise(null);
        Assert.AreEqual(0, o.Count);
        Assert.ThrowsExactly<ObjectDisposedException>(() => o.Clear());
    }

    [TestMethod]
    public void FailedResourceSwitch_PreservesOldSubscriptions()
    {
        using var old = new ObservableI18nResource<int, string>("old", En)
        {
            RecordObserver = true,
        };
        var disposed = new ObservableI18nResource<int, string>("disposed", En);
        disposed.Dispose();
        var s = new Model<int>(1);
        using var o = Observe(old, s);
        Assert.ThrowsExactly<ArgumentNullException>(() => o.Resource = null!);
        Assert.ThrowsExactly<ObjectDisposedException>(() => o.Resource = disposed);
        Assert.AreSame(old, o.Resource);
        Assert.AreEqual(1, old.Observers.Count);
        old.SetData(1, "value");
        Assert.AreEqual(2, s.Notices.Count);
    }

    [TestMethod]
    public void ChangedFailureDuringSwitch_KeepsCompleteNewSubscriptionsAndKeys()
    {
        using var old = new ObservableI18nResource<int, string>("old", En)
        {
            RecordObserver = true,
        };
        using var next = new ObservableI18nResource<int, string>("next", En)
        {
            RecordObserver = true,
        };
        var s = new Model<int>(1);
        using var o = new I18nObserver<Model<int>, int, string>(
            old,
            s,
            (_, _) => throw new ApplicationException("Notify failure.")
        );
        o.Add(new(x => x.Key, "Text"));
        o.Add(new(x => x.OtherKey, "OtherText"));
        s.Key = 2;
        s.OtherKey = 3;
        Assert.ThrowsExactly<ApplicationException>(() => o.Resource = next);
        Assert.AreSame(next, o.Resource);
        Assert.AreSequenceEqual(new[] { 2, 3 }, o.Registrations.Select(x => x.OldKey));
        Assert.AreEqual(0, old.Observers.Count);
        Assert.AreEqual(1, next.Observers.Count);
        old.SetData(2, "old");
        Assert.ThrowsExactly<ApplicationException>(() => next.SetData(2, "next"));
        Assert.AreEqual("next", next.GetData(2));
    }

    [TestMethod]
    public void ChangedFailureDuringSourceUpdate_CommitsEveryReference()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(1);
        using var o = new I18nObserver<Model<int>, int, string>(
            r,
            s,
            (_, _) => throw new ApplicationException("Notify failure.")
        );
        o.Add(new(x => x.Key, "Text"));
        o.Add(new(x => x.OtherKey, "OtherText"));
        s.Key = 2;
        s.OtherKey = 3;
        Assert.ThrowsExactly<ApplicationException>(() => s.Raise(null));
        Assert.AreSequenceEqual(new[] { 2, 3 }, o.Registrations.Select(x => x.OldKey));
        Assert.AreEqual(0, r.DatasByKey.Count);
        s.Raise(null);
    }

    private sealed class ControlledModel : INotifyPropertyChanged
    {
        public string First { get; set; } = "a";
        public string Second { get; set; } = "b";
        public Action? Reading { get; set; }
        public string Key => First;
        public string OtherKey
        {
            get
            {
                Reading?.Invoke();
                return Second;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Raise(string? name = null) => PropertyChanged?.Invoke(this, new(name));
    }

    [TestMethod]
    public void GetterFailure_PrevalidatesAddAllKeysAndResourceSwitch()
    {
        using var old = new ObservableI18nResource<string, string>("old", En)
        {
            RecordObserver = true,
        };
        using var next = new ObservableI18nResource<string, string>("next", En)
        {
            RecordObserver = true,
        };
        var source = new ControlledModel();
        using var o = new I18nObserver<ControlledModel, string, string>(old, source, (_, _) => { });
        o.Add(new(s => s.Key, "Text"));
        var second = new ObservedPropertyRegistration<ControlledModel, string>(
            s => s.OtherKey,
            "OtherText"
        );
        source.Reading = () => throw new ApplicationException("Read failure.");
        Assert.ThrowsExactly<ApplicationException>(() => o.Add(second));
        Assert.AreEqual(1, o.Count);
        source.Reading = null;
        o.Add(second);
        source.First = "c";
        source.Second = "d";
        source.Reading = () =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(o.Clear);
            throw new ApplicationException("Read failure.");
        };
        Assert.ThrowsExactly<ApplicationException>(() => source.Raise());
        Assert.AreSequenceEqual(new[] { "a", "b" }, o.Registrations.Select(x => x.OldKey));
        Assert.ThrowsExactly<ApplicationException>(() => o.Resource = next);
        Assert.AreSame(old, o.Resource);
        Assert.AreEqual(1, old.Observers.Count);
        Assert.AreEqual(0, next.Observers.Count);
        source.Reading = null;
        source.Raise();
        Assert.AreSequenceEqual(new[] { "c", "d" }, o.Registrations.Select(x => x.OldKey));
        source.Second = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => source.Raise());
        Assert.AreEqual("d", o.Registrations.Last().OldKey);
    }

    [TestMethod]
    public void RegistrationViews_AreIndependentReadOnlyAndValidateBeforeCopy()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(0);
        using var o = Observe(r, s);
        var fake = new ObservedPropertyRegistration<Model<int>, int>(x => x.Key, "Other");
        Assert.IsFalse(o.Contains(fake));
        Assert.IsFalse(
            ((ICollection<ObservedPropertyRegistration<Model<int>, int>>)o).Remove(fake)
        );
        Assert.IsFalse(o.Registrations.Contains(fake));
        var registration = o.Registrations.Single();
        Assert.ThrowsExactly<NotSupportedException>(() => registration.OldKey = 1);
        Assert.ThrowsExactly<NotSupportedException>(() => o.Registrations.Add(fake));
        Assert.ThrowsExactly<NotSupportedException>(() => o.Registrations.Remove(registration));
        Assert.ThrowsExactly<NotSupportedException>(o.Registrations.Clear);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            o.Registrations.CopyTo(new IObservedPropertyRegistration<int>[1], -1)
        );
        Assert.ThrowsExactly<ArgumentException>(() => o.Registrations.CopyTo([], 0));
        var copied = new IObservedPropertyRegistration<int>[1];
        o.Registrations.CopyTo(copied, 0);
        Assert.AreSame(registration, copied[0]);
        Assert.ThrowsExactly<ArgumentNullException>(() => o.Add(null!));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ObservedPropertyRegistration<Model<int>, int>(x => x.Key, " ")
        );
        var key = s.Key;
        Assert.IsFalse(o.SetData(key, "local"));
        Assert.IsFalse(r.ContainsData(key));
    }

    [TestMethod]
    public void TrueNoCulture_UsesFallbackWithoutInventingCultureOrData()
    {
        using var r = new ObservableI18nResource<int, int>("r", (CultureInfo?)null);
        var source = new Model<int>(0);
        using var o = new I18nObserver<Model<int>, int, int>(r, source, (_, _) => { });
        o.Add(new(s => s.Key, "Text"));
        Assert.IsNull(r.CurrentCulture);
        Assert.AreEqual(0, r.Cultures.Count);
        Assert.AreEqual(
            42,
            o.GetDataOrDefault(
                0,
                getDefaultValue: (key, culture) =>
                {
                    Assert.IsNull(culture);
                    return 42;
                }
            )
        );
        Assert.ThrowsExactly<InvalidOperationException>(() => o.GetData(0));
        Assert.IsFalse(o.SetData(source.Key, 0));
        Assert.AreEqual(0, r.DatasByKey.Count);
        r.AddCulture(En);
        r.CurrentCulture = En;
        Assert.IsTrue(o.SetData(source.Key, 0));
        Assert.AreEqual(
            0,
            o.GetDataOrDefault(
                0,
                getDefaultValue: (_, _) => throw new ApplicationException("Unexpected fallback.")
            )
        );
    }

    [TestMethod]
    public void DisposedObserver_IsCollectibleWhileSourceAndResourceRemainAlive()
    {
        using var r = new ObservableI18nResource<int, string>("r", En) { RecordObserver = true };
        var source = new Model<int>(0);
        var reference = CreateDisposed(r, source);
        for (var i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        Assert.IsFalse(reference.IsAlive);
        GC.KeepAlive(source);
        GC.KeepAlive(r);
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining
    )]
    private static WeakReference CreateDisposed(
        ObservableI18nResource<int, string> resource,
        Model<int> source
    )
    {
        var observer = Observe(resource, source);
        var reference = new WeakReference(observer);
        observer.Dispose();
        return reference;
    }

    [TestMethod]
    public void CallbackReentryRejectsNoOpAndSnapshotsTargetsForEveryRegistration()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(0);
        var notices = new List<string>();
        I18nObserver<Model<int>, int, string>? observer = null;
        var second = new ObservedPropertyRegistration<Model<int>, int>(
            x => x.OtherKey,
            "OtherText"
        );
        using var o = new I18nObserver<Model<int>, int, string>(
            r,
            s,
            (_, name) =>
            {
                notices.Add(name);
                if (observer is null)
                    throw new InvalidOperationException("Observer is unavailable.");
                Assert.ThrowsExactly<InvalidOperationException>(() => observer.Resource = r);
                Assert.ThrowsExactly<InvalidOperationException>(() =>
                    observer.SetData(s.Key, "value")
                );
                Assert.ThrowsExactly<InvalidOperationException>(observer.Clear);
                Assert.ThrowsExactly<InvalidOperationException>(observer.Dispose);
                second.TargetPropertyNames.Clear();
                second.TargetPropertyNames.Add("Next");
            }
        );
        observer = o;
        o.Add(new(x => x.Key, "Text"));
        o.Add(second);
        r.SetData(0, "value");
        Assert.AreSequenceEqual(new[] { "Text", "OtherText" }, notices);
        notices.Clear();
        r.SetData(0, "next");
        Assert.AreSequenceEqual(new[] { "Text", "Next" }, notices);
    }

    [TestMethod]
    public void AllProperties_CommitAllReferencesBeforeNotification()
    {
        using var r = new ObservableI18nResource<int, string>("r", En);
        var s = new Model<int>(1);
        using var o = Observe(r, s);
        o.Add(new(x => x.OtherKey, "OtherText"));
        s.Key = 2;
        s.OtherKey = 3;
        s.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Text")
                Assert.AreSequenceEqual(
                    new[] { s.Key, s.OtherKey },
                    o.Registrations.Select(x => x.OldKey)
                );
        };
        s.Raise(null);
        Assert.AreEqual(3, s.Notices.Count);
        s.Key = 4;
        s.OtherKey = 5;
        s.Raise("");
        Assert.AreSequenceEqual(new[] { 4, 5 }, o.Registrations.Select(x => x.OldKey));
    }
}
