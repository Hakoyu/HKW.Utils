using System.ComponentModel;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class I18nIntegrationTests
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr");

    private sealed class Consumer<TKey>(TKey key) : INotifyPropertyChanged where TKey : notnull
    {
        public TKey Key { get; set; } = key;
        public TKey OtherKey { get; set; } = key;
        public Dictionary<string, string?> Values { get; } = [];
        public List<string> Notices { get; } = [];
        public event PropertyChangedEventHandler? PropertyChanged;
        public void Raise(string? name) => PropertyChanged?.Invoke(this, new(name));
    }

    private static I18nObserver<Consumer<TKey>, TKey, string> Observe<TKey>(
        ObservableI18nResource<TKey, string> resource, Consumer<TKey> consumer) where TKey : notnull
    {
        var observer = new I18nObserver<Consumer<TKey>, TKey, string>(resource, consumer, (source, name) =>
        {
            source.Values[name] = resource.GetDataOrDefault(source.Key);
            source.Notices.Add(name);
        });
        observer.Add(new(source => source.Key, "Text", "Caption"));
        observer.Add(new(source => source.OtherKey, "OtherText"));
        return observer;
    }

    [TestMethod]
    public void CoreToRowsObserversAndIndexers_DefaultKeysAndSameKeyTargets()
    {
        Check(0); Check(Guid.Empty);
        static void Check<TKey>(TKey key) where TKey : notnull
        {
            using var core = new I18nCore(null);
            using var first = new ObservableI18nResource<TKey, string>("first", null, [En, Fr]) { GetDefaultValue = (_, _) => "fallback" };
            using var second = new ObservableI18nResource<TKey, string>("second", null, [En, Fr]);
            first.SetData(key, "English", En); first.SetData(key, "French", Fr);
            second.SetData(key, "Second", Fr);
            core.AddResource(first); core.AddResource(second);
            var a = new Consumer<TKey>(key); var b = new Consumer<TKey>(key);
            using var observer = Observe(first, a); using var other = Observe(first, b);
            var row = first.DatasByKey[key];
            var indexerValues = new List<string?>();
            first.GetCurrentCultureDataOrDefault.PropertyChanged += (_, _) =>
            {
                Assert.AreEqual(core.CurrentCulture, second.CurrentCulture);
                indexerValues.Add(first.GetCurrentCultureDataOrDefault[key]);
            };
            first.CurrentCultureChanged += (_, _) =>
            {
                Assert.AreEqual(core.CurrentCulture, first.CurrentCulture);
                Assert.AreEqual(core.CurrentCulture, second.CurrentCulture);
                Assert.AreSame(row, first.DatasByKey[key]);
            };
            core.CurrentCulture = Fr;
            Assert.IsTrue(a.Values.Values.All(value => value == "French"));
            Assert.AreEqual(3, a.Notices.Count); Assert.AreEqual(3, b.Notices.Count);
            row[first.GetCultureIndex(Fr)] = "Updated";
            Assert.IsTrue(a.Values.Values.All(value => value == "Updated"));
            Assert.AreEqual(6, a.Notices.Count); Assert.AreEqual(6, b.Notices.Count);
            Assert.IsTrue(observer.SetData(key, "Internal", keyPropertyName: "Key"));
            Assert.AreEqual(9, a.Notices.Count); Assert.AreEqual(9, b.Notices.Count);
            core.CurrentCulture = null;
            Assert.IsFalse(first.TryGetData(key, out _));
            Assert.IsFalse(observer.SetData(key, "Rejected", keyPropertyName: "Key"));
            Assert.ThrowsExactly<InvalidOperationException>(() => first.GetCurrentCultureData[key]);
            Assert.IsTrue(a.Values.Values.All(value => value == "fallback"));
            Assert.AreSequenceEqual(new[] { "French", "French", "Updated", "Updated", "Internal", "Internal", "fallback", "fallback" }, indexerValues);
            Assert.AreEqual("Internal", row[first.GetCultureIndex(Fr)]);
            core.CurrentCulture = En;
            Assert.IsTrue(a.Values.Values.All(value => value == "English"));
            Assert.IsTrue(b.Values.Values.All(value => value == "English"));
        }
    }

    [TestMethod]
    public void EmptyCultures_KeepNullAndForwardFallbackCulture()
    {
        using var core = new I18nCore(null);
        using var resource = new ObservableI18nResource<int, string>("empty")
        {
            GetDefaultValue = (key, culture) => { Assert.IsNull(culture); return $"missing:{key}"; }
        };
        core.AddResource(resource);
        var target = new Consumer<int>(0); using var observer = Observe(resource, target);
        Assert.AreEqual("missing:0", observer.GetDataOrDefault(0));
        Assert.AreEqual("missing:0", resource.GetCurrentCultureDataOrDefault[0]);
        Assert.IsFalse(observer.SetData(0, "unavailable", keyPropertyName: "Key"));
        Assert.AreEqual(0, resource.Cultures.Count); Assert.AreEqual(0, resource.DatasByKey.Count);
        Assert.ThrowsExactly<ArgumentException>(() => core.CurrentCulture = En);
        Assert.IsNull(core.CurrentCulture); Assert.IsNull(resource.CurrentCulture);
    }

    [TestMethod]
    public void OwnedRowKeyNotification_SurvivesCoreSwitchAndExplicitRename()
    {
        using var core = new I18nCore(En);
        using var resource = new ObservableI18nResource<int, string>("row", En, [En, Fr]) { GetDefaultValue = (_, _) => "missing" };
        resource.SetData(0, "English", En); resource.SetData(0, "French", Fr);
        core.AddResource(resource);
        var row = resource.DatasByKey[0];
        var keyNotifications = 0;
        row.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(row.Key))
            {
                keyNotifications++;
                Assert.AreSame(row, resource.DatasByKey[row.Key]);
                Assert.AreEqual("French", resource.GetCurrentCultureData[row.Key]);
            }
        };
        var rowValues = new List<string>();
        using var rowObserver = new I18nObserver<ObservableCultureDataList<int, string>, int, string>(
            resource, row, (source, _) => rowValues.Add(resource.GetDataOrDefault(source.Key)));
        rowObserver.Add(new(source => source.Key, "Text"));
        var ordinary = new Consumer<int>(0); using var observer = Observe(resource, ordinary);
        core.CurrentCulture = null; core.CurrentCulture = Fr;
        Assert.IsTrue(resource.RenameKey(0, 1));
        Assert.AreEqual(1, keyNotifications); Assert.AreEqual(1, row.Key);
        Assert.AreEqual(1, rowObserver.Registrations.Single().OldKey);
        Assert.AreEqual(0, ordinary.Key); Assert.AreEqual("missing", ordinary.Values["Text"]);
        Assert.AreEqual("French", rowValues[^1]);
        ordinary.Key = 1; ordinary.Raise(nameof(ordinary.Key));
        Assert.AreEqual("French", ordinary.Values["Text"]);
        Assert.IsFalse(resource.ContainsData(0)); Assert.AreSame(row, resource.DatasByKey[1]);
        Assert.IsTrue(rowObserver.SetData(1, "Changed", keyPropertyName: "Key"));
        Assert.AreEqual("Changed", ordinary.Values["Text"]);
        Assert.AreEqual("Changed", rowValues[^1]);
    }

    [TestMethod]
    public void CoreOperation_FreezesAllAttachedObserversAndTheirTargetSnapshots()
    {
        using var core = new I18nCore(En);
        using var first = new ObservableI18nResource<int, string>("first", En, [En, Fr]);
        using var second = new ObservableI18nResource<int, string>("second", En, [En, Fr]);
        core.AddResource(first); core.AddResource(second);
        var target = new Consumer<int>(0); using var observer = Observe(second, target);
        var registration = observer.Single(item => item.PropertyName == nameof(target.Key));
        core.PropertyChanging += (_, _) =>
        {
            Assert.AreEqual(0, second.Observers.Count);
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.SetData(0, "bad", keyPropertyName: "Key"));
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.Resource = first);
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.Resource = second);
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.Clear());
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.Remove("missing"));
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.TryAdd(registration));
            Assert.ThrowsExactly<InvalidOperationException>(() => observer.Dispose());
            Assert.ThrowsExactly<InvalidOperationException>(() => target.Raise(nameof(target.Key)));
            Assert.ThrowsExactly<InvalidOperationException>(() => new I18nObserver<Consumer<int>, int, string>(second, target, (_, _) => { }));
            registration.TargetPropertyNames.Clear(); registration.TargetPropertyNames.Add("Next");
        };
        first.CurrentCultureChanged += (_, _) => Assert.ThrowsExactly<InvalidOperationException>(() => observer.Dispose());
        core.CurrentCulture = Fr;
        Assert.AreSequenceEqual(new[] { "Text", "Caption", "OtherText" }, target.Notices);
        target.Notices.Clear(); core.CurrentCulture = En;
        Assert.AreSequenceEqual(new[] { "Next", "OtherText" }, target.Notices);
        Assert.IsTrue(observer.SetData(0, "works", keyPropertyName: "Key"));
    }

    [TestMethod]
    public void CoreRemovalClearDispose_DoNotDetachObserversOrDestroyRows()
    {
        var core = new I18nCore(En);
        using var resource = new ObservableI18nResource<int, string>("r", En) { RecordObserver = true };
        var target = new Consumer<int>(0); using var observer = Observe(resource, target);
        resource.SetData(0, "start"); var row = resource.DatasByKey[0];
        core.AddResource(resource); core.RemoveResource(resource);
        resource.SetData(0, "removed"); Assert.AreEqual("removed", target.Values["Text"]);
        core.AddResource(resource); core.ClearResource();
        resource.SetData(0, "cleared"); Assert.AreEqual("cleared", target.Values["Text"]);
        core.AddResource(resource); core.Dispose();
        row[0] = "disposed core"; Assert.AreEqual("disposed core", target.Values["Text"]);
        Assert.AreEqual(1, resource.Observers.Count); Assert.AreSame(row, resource.DatasByKey[0]);
        resource.Dispose(); Assert.AreEqual(2, observer.Count); Assert.AreEqual("disposed core", row[0]);
        using var replacement = new ObservableI18nResource<int, string>("replacement", En);
        observer.Resource = replacement;
        Assert.IsTrue(observer.SetData(0, "still alive", keyPropertyName: "Key"));
        Assert.AreEqual("still alive", replacement.GetData(0));
    }

    [TestMethod]
    public void IndexerAndConsumerCallbacks_ReadEveryCommittedResourceAndRejectReentry()
    {
        using var core = new I18nCore(En);
        using var first = new ObservableI18nResource<int, string>("first", En, [En, Fr]);
        using var second = new ObservableI18nResource<int, string>("second", En, [En, Fr]);
        first.SetData(0, "one", Fr); second.SetData(0, "two", Fr);
        core.AddResource(first); core.AddResource(second);
        var target = new Consumer<int>(0); var observed = new List<string>();
        using var observer = new I18nObserver<Consumer<int>, int, string>(first, target, (_, _) =>
        {
            observed.Add(first.GetData(0) + second.GetData(0));
            Assert.AreEqual(Fr, core.CurrentCulture);
            Assert.ThrowsExactly<InvalidOperationException>(() => core.ClearResource());
            Assert.ThrowsExactly<InvalidOperationException>(() => second.SetData(0, "blocked"));
        });
        observer.Add(new(source => source.Key, "Text", "Caption"));
        first.GetCurrentCultureData.PropertyChanged += (_, _) => observed.Add(first.GetCurrentCultureData[0] + second.GetCurrentCultureData[0]);
        core.CurrentCulture = Fr;
        Assert.AreSequenceEqual(new[] { "onetwo", "onetwo", "onetwo", "onetwo" }, observed);
    }
}
