using System.Collections;
using System.ComponentModel;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class I18nMatrixTests
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr");
    private static readonly CultureInfo De = CultureInfo.GetCultureInfo("de");
    private static readonly CultureInfo Ja = CultureInfo.GetCultureInfo("ja");

    private static ObservableI18nResource<string, string> Create()
    {
        var resource = new ObservableI18nResource<string, string>("matrix", En, [En, Fr, De]);
        foreach (var culture in resource.Cultures)
            resource.SetDatas(
                [new("a", "a-" + culture.Name), new("b", "b-" + culture.Name)],
                culture
            );
        return resource;
    }

    private static void AssertMatrix(ObservableI18nResource<string, string> resource)
    {
        Assert.AreEqual(resource.Cultures.Count, resource.IndexByCulture.Count);
        Assert.AreSequenceEqual(resource.ObservableCultures, resource.Cultures);
        for (var i = 0; i < resource.Cultures.Count; i++)
        {
            var culture = resource.GetCulture(i);
            Assert.AreEqual(i, resource.GetCultureIndex(culture));
            Assert.AreEqual(i, resource.GetCultureIndex(culture.Name));
            foreach (var pair in resource.DatasByKey)
            {
                Assert.AreEqual(pair.Key, pair.Value.Key);
                Assert.AreEqual(resource.Cultures.Count, pair.Value.Count);
                Assert.AreEqual(pair.Value[i], resource.GetData(pair.Key, culture));
            }
        }
        foreach (var row in resource.DatasByKey.Values)
            Assert.AreEqual(resource.Cultures.Count, row.Count);
        Assert.IsTrue(
            resource.CurrentCulture is null || resource.HasCulture(resource.CurrentCulture)
        );
    }

    [TestMethod]
    public void DeleteMiddleAddAndDeleteAgain_UsesNumericOrder()
    {
        using var r = Create();
        r.RemoveCulture(Fr);
        r.AddCulture(Ja);
        r.RemoveCulture(En);
        AssertMatrix(r);
        Assert.AreEqual("a-de", r.GetData("a", De));
        Assert.AreEqual(0, r.GetCultureIndex("de"));
        Assert.AreEqual(-1, r.GetCultureIndex("missing"));
    }

    [TestMethod]
    public void EveryCallbackSeesCompleteMatrix()
    {
        using var r = Create();
        var calls = 0;
        void Read()
        {
            calls++;
            AssertMatrix(r);
        }
        r.CultureDataChanged += (_, _) => Read();
        r.CurrentCultureChanged += (_, _) => Read();
        r.PropertyChanged += (_, _) => Read();
        r.KeyRenamed += (_, _) => Read();
        r.GetCurrentCultureData.PropertyChanged += (_, _) => Read();
        r.GetCurrentCultureDataOrDefault.PropertyChanged += (_, _) => Read();
        r.ObservableCultures.ListChanging += (_, _) => Read();
        r.ObservableCultures.ListChanged += (_, _) => Read();
        r.ObservableCultures.CollectionChanged += (_, _) => Read();
        r.ObservableCultures.PropertyChanged += (_, _) => Read();
        r.DatasByKey.DictionaryChanging += (_, _) => Read();
        r.DatasByKey.DictionaryChanged += (_, _) => Read();
        foreach (var row in r.DatasByKey.Values)
        {
            row.ListChanging += (_, _) => Read();
            row.ListChanged += (_, _) => Read();
            row.CollectionChanged += (_, _) => Read();
            row.PropertyChanged += (_, _) => Read();
        }
        r.RemoveCulture(Fr);
        r.AddCulture(Ja);
        r.ReplaceCulture(En, Fr);
        r.RenameKey("a", "c");
        r.SetDatas([new("c", "x"), new("b", "y")], De);
        r.RemoveCulture(Fr);
        r.ClearCulture();
        r.ClearData();
        Assert.IsGreaterThan(20, calls);
    }

    [TestMethod]
    public void RenameKeepsRowAndItsNotifications()
    {
        using var r = Create();
        var row = r.DatasByKey["a"];
        var keys = new List<string>();
        var keyNotifications = 0;
        r.CultureDataChanged += (_, e) => keys.Add(e.Key);
        row.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Key")
                keyNotifications++;
        };
        Assert.IsFalse(r.RenameKey("a", "b"));
        Assert.IsFalse(r.RenameKey("a", "a"));
        Assert.IsTrue(r.RenameKey("a", "c"));
        Assert.AreSame(row, r.DatasByKey["c"]);
        Assert.AreEqual(1, keyNotifications);
        keys.Clear();
        row[0] = "changed";
        Assert.AreSequenceEqual(new[] { "c" }, keys);
        Assert.IsTrue(r.RenameKey("c", "b", true));
        AssertMatrix(r);
    }

    [TestMethod]
    public void CurrentAndLastCultureRemoval_NotifiesNullAndHasDefinedFallback()
    {
        using var r = Create();
        var cultures = new List<CultureInfo?>();
        r.CurrentCultureChanged += (_, c) => cultures.Add(c);
        r.RemoveCulture(En);
        Assert.IsNull(r.CurrentCulture);
        Assert.AreEqual(1, cultures.Count);
        Assert.IsNull(cultures[0]);
        Assert.IsFalse(r.SetData("a", "ignored"));
        Assert.Throws<InvalidOperationException>(() => r.GetData("a"));
        Assert.AreEqual("fallback", r.GetDataOrDefault("a", "fallback"));
        r.GetDefaultValue = (_, c) => c is null ? "none" : "other";
        Assert.AreEqual("none", r.GetDataOrDefault("a"));
        r.RemoveCulture(Fr);
        r.RemoveCulture(De);
        AssertMatrix(r);
        Assert.Throws<ArgumentOutOfRangeException>(() => r.GetCulture(0));
        Assert.Throws<KeyNotFoundException>(() => r.GetCulture("en"));
        r.AddCulture(En);
        Assert.IsNull(r.CurrentCulture);
    }

    [TestMethod]
    public void ChangingFailureLeavesEverythingUnchanged()
    {
        using var r = Create();
        r.DatasByKey["b"].ListChanging += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => r.RemoveCulture(En));
        AssertMatrix(r);
        Assert.AreEqual(En, r.CurrentCulture);
        Assert.AreEqual("a-en", r.GetData("a"));
        Assert.AreEqual(3, r.Cultures.Count);
    }

    [TestMethod]
    public void ChangedFailureKeepsCompleteNewState()
    {
        using var r = Create();
        r.DatasByKey["a"].ListChanged += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => r.RemoveCulture(En));
        AssertMatrix(r);
        Assert.IsNull(r.CurrentCulture);
        Assert.AreEqual(2, r.Cultures.Count);
        Assert.AreEqual("b-fr", r.GetData("b", Fr));
    }

    [TestMethod]
    public void OperationSnapshotsAndRejectsResourceAndSiblingRowReentry()
    {
        using var r = Create();
        var oldCalls = 0;
        var lateCalls = 0;
        CultureDataChangedEventHandler<string, string> old = (_, _) => oldCalls++;
        r.CultureDataChanged += old;
        r.DatasByKey["a"].ListChanging += (_, _) =>
        {
            r.CultureDataChanged -= old;
            r.CultureDataChanged += (_, _) => lateCalls++;
            Assert.Throws<InvalidOperationException>(() => r.AddCulture(Ja));
            Assert.Throws<InvalidOperationException>(() => r.DatasByKey["b"][0] = "reentrant");
        };
        r.SetData("a", "new");
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, lateCalls);
        Assert.AreEqual("b-en", r.GetData("b"));
    }

    [TestMethod]
    public void OwnedRowsRejectStructureAndAlias()
    {
        using var r = Create();
        var row = r.DatasByKey["a"];
        Assert.Throws<InvalidOperationException>(() => row.Add("bad"));
        Assert.Throws<InvalidOperationException>(() => row.RemoveAt(0));
        Assert.Throws<InvalidOperationException>(() => row.Clear());
        Assert.Throws<InvalidOperationException>(() => ((IList)row).Insert(0, "bad"));
        Assert.Throws<ArgumentException>(() => r.AddData("wrong", new("a")));
        using var other = new ObservableI18nResource<string, string>("other", En, [En, Fr, De]);
        Assert.Throws<InvalidOperationException>(() => other.AddData("a", row));
        AssertMatrix(r);
        Assert.IsEmpty(other.DatasByKey);
    }

    [TestMethod]
    public void RemovalReplacementClearAndDisposeDetachRows()
    {
        var r = Create();
        var old = r.DatasByKey["a"];
        var invalidations = 0;
        r.CultureDataChanged += (_, _) => invalidations++;
        r.RemoveData("a");
        Assert.IsGreaterThan(0, invalidations);
        var before = invalidations;
        old[0] = "detached";
        Assert.AreEqual(before, invalidations);
        var remaining = r.DatasByKey["b"];
        r.Dispose();
        before = invalidations;
        remaining[0] = "after-dispose";
        Assert.AreEqual(before, invalidations);
        r.Dispose();
    }

    [TestMethod]
    public void ExplicitNullConstructorsAndDefaultValuesHaveNoSentinelAmbiguity()
    {
        using var r = new ObservableI18nResource<int, int>(
            "zero",
            null,
            [CultureInfo.InvariantCulture, En]
        );
        Assert.IsNull(r.CurrentCulture);
        Assert.Throws<ArgumentException>(() => r.GetCultureIndex(""));
        Assert.Throws<ArgumentException>(() => r.GetCulture(""));
        var changes = 0;
        r.CultureDataChanged += (_, e) =>
        {
            changes++;
            Assert.AreEqual(0, e.Key);
        };
        Assert.IsTrue(r.SetData(0, 0, En));
        Assert.AreEqual(1, changes);
        Assert.IsFalse(r.TryGetData(0, out _));
        Assert.IsTrue(r.TryGetData(0, out var value, En));
        Assert.AreEqual(0, value);
        Assert.IsFalse(r.TryGetData(1, out _, En));
        Assert.IsFalse(r.TryGetData(0, out _, Ja));
        Assert.Throws<KeyNotFoundException>(() => r.GetData(0, Ja));
        Assert.Throws<ArgumentOutOfRangeException>(() => r.GetCulture(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => r.GetCulture(r.Cultures.Count));
        using var empty = new ObservableI18nResource<int, int>("empty", (CultureInfo?)null);
        Assert.IsEmpty(empty.Cultures);
        II18nResource contract = r;
        contract.CurrentCulture = En;
        contract.CurrentCulture = null;
        Assert.IsNull(r.CurrentCulture);
    }

    [TestMethod]
    public void ReplacementAndClearInvalidateAndDetachWithoutDestroyingRows()
    {
        using var r = Create();
        var old = r.DatasByKey["a"];
        var replacement = new ObservableCultureDataList<string, string>("a", ["replacement"]);
        var keys = new List<string>();
        var refresh = 0;
        r.CultureDataChanged += (_, e) =>
        {
            keys.Add(e.Key);
            Assert.IsNull(e.CultureInfo);
            AssertMatrix(r);
        };
        r.GetCurrentCultureData.PropertyChanged += (_, _) => refresh++;
        Assert.IsTrue(r.ReplaceData("a", replacement));
        Assert.AreEqual(3, replacement.Count);
        Assert.AreEqual("replacement", r.GetData("a"));
        Assert.AreSequenceEqual(new[] { "a" }, keys);
        old[0] = "detached";
        Assert.HasCount(1, keys);
        r.ClearData();
        Assert.AreSequenceEqual(new[] { "a", "a", "b" }, keys);
        Assert.AreEqual(4, refresh);
        replacement[0] = "also-detached";
        Assert.HasCount(3, keys);
        Assert.AreEqual(3, replacement.Count);
    }

    [TestMethod]
    public void RowAdditionCancellationKeepsInputAndOwnershipUnchanged()
    {
        using var r = Create();
        var row = new ObservableCultureDataList<string, string>("c", ["initial"]);
        ObservableDictionaryChangingEventHandler<
            string,
            ObservableCultureDataList<string, string>
        > reject = (_, _) => throw new ApplicationException("stop");
        r.DatasByKey.DictionaryChanging += reject;
        Assert.Throws<ApplicationException>(() => r.AddData("c", row));
        Assert.AreSequenceEqual(new[] { "initial" }, row);
        Assert.IsFalse(r.ContainsData("c"));
        row.Add("still-free");
        r.DatasByKey.DictionaryChanging -= reject;
        Assert.IsTrue(r.AddData("c", row));
        AssertMatrix(r);
        Assert.Throws<InvalidOperationException>(() =>
            ((ICollection<string>)row).Remove("missing")
        );
        Assert.Throws<InvalidOperationException>(() => ((IList)row).Clear());
        Assert.IsTrue(((IList)row).IsFixedSize);
        Assert.Throws<NotSupportedException>(() =>
            ((IDictionary<string, ObservableCultureDataList<string, string>>)r.DatasByKey).Remove(
                "c"
            )
        );
    }

    [TestMethod]
    public void NewlyAddedRowCannotBeClaimedDuringChanging()
    {
        using var r = Create();
        using var other = Create();
        var row = new ObservableCultureDataList<string, string>("c", ["initial"]);
        r.DatasByKey.DictionaryChanging += (_, _) =>
        {
            Assert.Throws<InvalidOperationException>(() => other.AddData("c", row));
            Assert.Throws<InvalidOperationException>(() => row[0] = "changed");
            Assert.Throws<InvalidOperationException>(() => row.Add("changed"));
        };
        Assert.IsTrue(r.AddData("c", row));
        AssertMatrix(r);
        Assert.IsFalse(other.ContainsData("c"));
    }

    [TestMethod]
    public void BatchInputFailureAndChangingFailureAreAtomic()
    {
        using var r = Create();
        IEnumerable<KeyValuePair<string, string>> Failing()
        {
            yield return new("a", "new");
            throw new ApplicationException("input");
        }
        Assert.Throws<ApplicationException>(() => r.SetDatas(Failing()));
        Assert.AreEqual("a-en", r.GetData("a"));
        ObservableListChangingEventHandler<string> reject = (_, _) =>
            throw new ApplicationException("changing");
        r.DatasByKey["b"].ListChanging += reject;
        Assert.Throws<ApplicationException>(() =>
            r.SetDatas([new("new", "created"), new("a", "new"), new("b", "new")])
        );
        Assert.IsFalse(r.ContainsData("new"));
        Assert.AreEqual("a-en", r.GetData("a"));
        r.DatasByKey["b"].ListChanging -= reject;
        r.SetDatas([new("a", "first"), new("a", "last"), new("b", "done")]);
        Assert.AreEqual("last", r.GetData("a"));
        Assert.AreEqual("done", r.GetData("b"));
    }

    [TestMethod]
    public void BatchChangedFailureRetainsAllCellsAndNewRows()
    {
        using var r = Create();
        r.DatasByKey["a"].ListChanged += (_, _) => throw new ApplicationException("changed");
        Assert.Throws<ApplicationException>(() =>
            r.SetDatas([new("a", "new"), new("b", "new"), new("c", "new")])
        );
        Assert.AreEqual("new", r.GetData("a"));
        Assert.AreEqual("new", r.GetData("b"));
        Assert.AreEqual("new", r.GetData("c"));
        AssertMatrix(r);
    }

    [TestMethod]
    public void RowAndViewSubscribersAreFrozenAcrossTheWholeOperation()
    {
        using var r = Create();
        var b = r.DatasByKey["b"];
        var old = 0;
        var late = 0;
        var lateView = 0;
        ObservableListChangedEventHandler<string> oldHandler = (_, _) => old++;
        b.ListChanged += oldHandler;
        r.DatasByKey["a"].ListChanging += (_, _) =>
        {
            b.ListChanged -= oldHandler;
            b.ListChanged += (_, _) => late++;
            r.ObservableCultures.ListChanged += (_, _) => lateView++;
            r.GetCurrentCultureData.PropertyChanged += (_, _) => lateView++;
        };
        r.RemoveCulture(En);
        Assert.AreEqual(1, old);
        Assert.AreEqual(0, late);
        Assert.AreEqual(0, lateView);
        r.SetData("b", "next", Fr);
        Assert.AreEqual(1, late);
    }

    [TestMethod]
    public void DirectCellWritesUseResourceSnapshotsAndRejectSiblingNoOpWrites()
    {
        using var r = Create();
        var a = r.DatasByKey["a"];
        var b = r.DatasByKey["b"];
        var calls = 0;
        CultureDataChangedEventHandler<string, string> handler = (_, _) => calls++;
        r.CultureDataChanged += handler;
        a.ListChanging += (_, _) =>
        {
            r.CultureDataChanged -= handler;
            Assert.Throws<InvalidOperationException>(() => ((IList<string>)b)[0] = b[0]);
            Assert.Throws<InvalidOperationException>(() => ((IList)b)[0] = b[0]);
            Assert.Throws<InvalidOperationException>(() => r.CurrentCulture = En);
            Assert.Throws<InvalidOperationException>(() => r.DefaultValue = "new-default");
            Assert.Throws<InvalidOperationException>(() => r.Dispose());
        };
        a[0] = "next";
        Assert.AreEqual(1, calls);
        Assert.AreEqual("next", r.GetData("a"));
        AssertMatrix(r);
    }

    [TestMethod]
    public void DirectCellCallbackFailuresReleaseResourceWriteGuard()
    {
        using var r = Create();
        var row = r.DatasByKey["a"];
        ObservableListChangingEventHandler<string> changing = (_, _) =>
            throw new ApplicationException("changing");
        row.ListChanging += changing;
        Assert.Throws<ApplicationException>(() => row[0] = "rejected");
        Assert.AreEqual("a-en", r.GetData("a"));
        row.ListChanging -= changing;
        ObservableListChangedEventHandler<string> changed = (_, _) =>
            throw new ApplicationException("changed");
        row.ListChanged += changed;
        Assert.Throws<ApplicationException>(() => row[0] = "committed");
        Assert.AreEqual("committed", r.GetData("a"));
        row.ListChanged -= changed;
        Assert.IsTrue(r.AddCulture(Ja));
    }

    [TestMethod]
    public void RenameCallbackFailurePreservesIdentityOwnershipAndKeyNotification()
    {
        using var r = Create();
        var row = r.DatasByKey["a"];
        var displaced = r.DatasByKey["b"];
        PropertyChangedEventHandler reject = (_, e) =>
        {
            if (e.PropertyName == "Key")
            {
                AssertMatrix(r);
                throw new ApplicationException("key");
            }
        };
        row.PropertyChanged += reject;
        Assert.Throws<ApplicationException>(() => r.RenameKey("a", "b", true));
        Assert.AreSame(row, r.DatasByKey["b"]);
        Assert.AreEqual("b", row.Key);
        Assert.IsFalse(r.ContainsData("a"));
        row.PropertyChanged -= reject;
        var events = 0;
        r.CultureDataChanged += (_, _) => events++;
        displaced[0] = "detached";
        Assert.AreEqual(0, events);
        row[0] = "subscribed";
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void CopyDataAndCopyDataToUseOneCompleteCommit()
    {
        using var source = Create();
        Assert.IsFalse(source.CopyData("a", "a", true));
        Assert.IsFalse(source.CopyData("a", "b", false));
        Assert.IsTrue(source.CopyData("a", "c", false));
        Assert.AreNotSame(source.DatasByKey["a"], source.DatasByKey["c"]);
        using var target = new ObservableI18nResource<string, string>("target");
        var calls = 0;
        target.CultureDataChanged += (_, _) =>
        {
            calls++;
            AssertMatrix(target);
            Assert.AreEqual("a-de", target.GetData("a", De));
            Assert.AreEqual("b-fr", target.GetData("b", Fr));
        };
        source.CopyDataTo(target);
        Assert.IsGreaterThan(0, calls);
        Assert.IsNull(target.CurrentCulture);
        Assert.AreEqual(3, target.DatasByKey.Count);
        source.CopyDataTo(source);
        AssertMatrix(source);
    }

    [TestMethod]
    [DataRow("add")]
    [DataRow("remove")]
    [DataRow("replace")]
    [DataRow("clear")]
    public void CultureChangingFailureLeavesOriginalMatrix(string action)
    {
        using var r = Create();
        r.ObservableCultures.ListChanging += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => ChangeCulture(r, action));
        AssertMatrix(r);
        Assert.AreSequenceEqual(new[] { En, Fr, De }, r.Cultures);
        Assert.AreEqual(En, r.CurrentCulture);
        Assert.AreEqual("a-en", r.GetData("a"));
    }

    [TestMethod]
    [DataRow("add", 4)]
    [DataRow("remove", 2)]
    [DataRow("replace", 3)]
    [DataRow("clear", 0)]
    public void CultureChangedFailureLeavesCompleteNewMatrix(string action, int count)
    {
        using var r = Create();
        r.ObservableCultures.ListChanged += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => ChangeCulture(r, action));
        AssertMatrix(r);
        Assert.AreEqual(count, r.Cultures.Count);
        if (action == "remove" || action == "clear")
            Assert.IsNull(r.CurrentCulture);
    }

    private static void ChangeCulture(ObservableI18nResource<string, string> r, string action)
    {
        switch (action)
        {
            case "add":
                r.AddCulture(Ja);
                break;
            case "remove":
                r.RemoveCulture(En);
                break;
            case "replace":
                r.ReplaceCulture(En, Ja);
                break;
            case "clear":
                r.ClearCulture();
                break;
        }
    }

    [TestMethod]
    public void ObserverConsumerReceivesStructuralInvalidationsAfterCommit()
    {
        using var r = Create();
        var probe = new ResourceProbe(r);
        using var observer = new I18nObserver<ResourceProbe, string, string>(
            r,
            probe,
            (s, name) => s.Notify(name)
        )
        {
            new(s => s.Key, nameof(ResourceProbe.Display)),
        };
        var seen = new List<string>();
        probe.PropertyChanged += (_, _) =>
        {
            AssertMatrix(r);
            seen.Add(probe.Display);
        };
        r.RemoveData("a");
        Assert.AreEqual("missing", seen[^1]);
        r.SetData("a", "added");
        Assert.AreEqual("added", seen[^1]);
        r.CopyData("b", "a", true);
        Assert.AreEqual("b-en", seen[^1]);
        r.CurrentCulture = De;
        var beforeUnrelatedRemoval = seen.Count;
        r.RemoveCulture(Fr);
        Assert.AreEqual(beforeUnrelatedRemoval, seen.Count);
        Assert.AreEqual("b-de", seen[^1]);
        r.ClearData();
        Assert.AreEqual("missing", seen[^1]);
        Assert.AreEqual(5, seen.Count);
    }

    private sealed class ResourceProbe(ObservableI18nResource<string, string> resource)
        : INotifyPropertyChanged
    {
        public string Key => "a";
        public string Display => resource.GetDataOrDefault(Key, "missing");
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Notify(string name) => PropertyChanged?.Invoke(this, new(name));
    }

    [TestMethod]
    public void RetainedDisposedRowDoesNotKeepResourceAliveAndCanBeReowned()
    {
        var row = CreateDetachedRow(out var weak);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.IsFalse(weak.IsAlive);
        using var other = new ObservableI18nResource<string, string>("new-owner", En, [En, Fr, De]);
        Assert.IsTrue(other.AddData("a", row));
        row[0] = "new-owner-value";
        Assert.AreEqual("new-owner-value", other.GetData("a"));
        GC.KeepAlive(row);
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining
    )]
    private static ObservableCultureDataList<string, string> CreateDetachedRow(
        out WeakReference weak
    )
    {
        var resource = Create();
        var row = resource.DatasByKey["a"];
        weak = new(resource);
        resource.Dispose();
        Assert.Throws<ObjectDisposedException>(() => resource.AddCulture(Ja));
        return row;
    }

    [TestMethod]
    public void RandomCultureOperationsMatchIndependentMatrix()
    {
        using var r = Create();
        var expected = new Dictionary<CultureInfo, string?[]>
        {
            [En] = ["a-en", "b-en"],
            [Fr] = ["a-fr", "b-fr"],
            [De] = ["a-de", "b-de"],
        };
        var order = new List<CultureInfo> { En, Fr, De };
        CultureInfo? current = En;
        CultureInfo[] pool = [En, Fr, De, Ja];
        var random = new Random(1908);
        for (var step = 0; step < 300; step++)
        {
            var culture = pool[random.Next(pool.Length)];
            var replacement = pool[random.Next(pool.Length)];
            switch (random.Next(4))
            {
                case 0:
                    var added = expected.TryAdd(culture, [null, null]);
                    Assert.AreEqual(added, r.AddCulture(culture));
                    if (added)
                        order.Add(culture);
                    break;
                case 1:
                    var removed = expected.Remove(culture);
                    Assert.AreEqual(removed, r.RemoveCulture(culture));
                    if (removed)
                        order.Remove(culture);
                    if (Equals(current, culture))
                        current = null;
                    break;
                case 2:
                    var replaced =
                        expected.ContainsKey(culture) && expected.ContainsKey(replacement) is false;
                    Assert.AreEqual(replaced, r.ReplaceCulture(culture, replacement));
                    if (replaced)
                    {
                        expected[replacement] = expected[culture];
                        expected.Remove(culture);
                        order[order.IndexOf(culture)] = replacement;
                        if (Equals(current, culture))
                            current = replacement;
                    }
                    break;
                case 3:
                    r.ClearCulture();
                    expected.Clear();
                    order.Clear();
                    current = null;
                    break;
                case 4:
                    if (expected.ContainsKey(culture))
                    {
                        r.CurrentCulture = culture;
                        current = culture;
                        r.SetData("a", step.ToString());
                        expected[culture][0] = step.ToString();
                    }
                    break;
            }
            AssertMatrix(r);
            Assert.AreEqual(current, r.CurrentCulture);
            Assert.AreSequenceEqual(order, r.Cultures);
            foreach (var pair in expected)
            {
                Assert.AreEqual(pair.Value[0], r.GetData("a", pair.Key));
                Assert.AreEqual(pair.Value[1], r.GetData("b", pair.Key));
            }
        }
    }

    [TestMethod]
    [DataRow("add")]
    [DataRow("remove")]
    [DataRow("replace")]
    [DataRow("clear")]
    [DataRow("rename")]
    public void DictionaryChangingFailureDoesNotChangeRowsOrOwnership(string action)
    {
        using var r = Create();
        var a = r.DatasByKey["a"];
        var b = r.DatasByKey["b"];
        r.DatasByKey.DictionaryChanging += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => ChangeData(r, action));
        AssertMatrix(r);
        Assert.AreSame(a, r.DatasByKey["a"]);
        Assert.AreSame(b, r.DatasByKey["b"]);
        Assert.AreEqual("a", a.Key);
        Assert.AreEqual("a-en", r.GetData("a"));
        Assert.AreEqual(2, r.DatasByKey.Count);
    }

    [TestMethod]
    [DataRow("add", 3)]
    [DataRow("remove", 1)]
    [DataRow("replace", 2)]
    [DataRow("clear", 0)]
    [DataRow("rename", 1)]
    public void DictionaryChangedFailureRetainsCompleteCommit(string action, int count)
    {
        using var r = Create();
        r.DatasByKey.DictionaryChanged += (_, _) => throw new ApplicationException("stop");
        Assert.Throws<ApplicationException>(() => ChangeData(r, action));
        AssertMatrix(r);
        Assert.AreEqual(count, r.DatasByKey.Count);
        if (action == "rename")
            Assert.AreEqual("a-en", r.GetData("b"));
        if (action == "replace")
            Assert.AreEqual("replacement", r.GetData("a"));
        if (action == "add")
            Assert.AreEqual("new", r.GetData("c"));
    }

    private static void ChangeData(ObservableI18nResource<string, string> r, string action)
    {
        switch (action)
        {
            case "add":
                r.AddData("c", new("c", ["new"]));
                break;
            case "remove":
                r.RemoveData("a");
                break;
            case "replace":
                r.ReplaceData("a", new("a", ["replacement"]));
                break;
            case "clear":
                r.ClearData();
                break;
            case "rename":
                r.RenameKey("a", "b", true);
                break;
        }
    }

    [TestMethod]
    public void ComparerCanonicalKeysSurviveReplacementRenameAndCopy()
    {
        var comparer = EqualityComparer<string>.Create(
            (a, b) => StringComparer.OrdinalIgnoreCase.Equals(a, b),
            value => StringComparer.OrdinalIgnoreCase.GetHashCode(value)
        );
        using var r = new ObservableI18nResource<string, string>("comparer", En, comparer);
        Assert.IsTrue(r.AddData("A", new("a", ["first"])));
        Assert.AreEqual("A", r.DatasByKey["a"].Key);
        Assert.IsTrue(r.ReplaceData("a", new("a", ["second"])));
        Assert.AreEqual("A", r.DatasByKey["a"].Key);
        Assert.IsFalse(r.RenameKey("a", "A"));
        Assert.IsTrue(r.CopyData("a", "B", false));
        Assert.IsTrue(r.RenameKey("a", "b", true));
        Assert.AreEqual("B", r.DatasByKey["b"].Key);
        var changedKey = "";
        r.CultureDataChanged += (_, e) => changedKey = e.Key;
        r.SetData("b", "third");
        Assert.AreEqual("B", changedKey);
        Assert.IsTrue(r.RemoveData("b"));
        Assert.IsEmpty(r.DatasByKey);
    }

    [TestMethod]
    public void RenameDictionaryEventsReplayTheFinalKeyOrder()
    {
        using var r = Create();
        r.SetData("c", "third");
        var replay = r.DatasByKey.Keys.ToList();
        r.DatasByKey.CollectionChanged += (_, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                replay.RemoveAt(e.OldStartingIndex);
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                replay.Insert(
                    e.NewStartingIndex,
                    (
                        (KeyValuePair<string, ObservableCultureDataList<string, string>>)
                            e.NewItems![0]!
                    ).Key
                );
            AssertMatrix(r);
        };
        r.RenameKey("a", "renamed");
        Assert.AreSequenceEqual(r.DatasByKey.Keys, replay);
        r.RenameKey("renamed", "c", true);
        Assert.AreSequenceEqual(r.DatasByKey.Keys, replay);
    }

    [TestMethod]
    public void IndexersRefreshForRelevantDataOnly()
    {
        using var r = Create();
        var strict = 0;
        var fallback = 0;
        r.GetCurrentCultureData.PropertyChanged += (_, _) => strict++;
        r.GetCurrentCultureDataOrDefault.PropertyChanged += (_, _) => fallback++;
        r.SetData("a", "other", Fr);
        Assert.AreEqual(0, strict);
        r.SetData("a", "current", En);
        Assert.IsGreaterThan(0, strict);
        Assert.AreEqual(strict, fallback);
        var before = strict;
        r.RemoveData("a");
        Assert.IsGreaterThan(before, strict);
    }
}
