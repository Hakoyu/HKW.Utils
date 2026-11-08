using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
[DoNotParallelize]
public class I18nCoreTests
{
    public static CultureInfo[] Cultures { get; } =
        CultureInfo.GetCultures(CultureTypes.NeutralCultures);
    public static CultureInfo CurrentCulture { get; } = CultureInfo.CurrentCulture;

    [TestMethod]
    public void ChangeSubResourceCulture()
    {
        Assert.IsTrue(Cultures.Length > 0);
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;
        try
        {
            var core = new I18nCore(Cultures[0]);
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(
                    core.AddResource(
                        new ObservableI18nResource<string, string>(
                            i.ToString(),
                            Cultures[0],
                            Cultures
                        )
                    )
                );
            }
            Assert.AreEqual(10, core.Resources.Count);
            foreach (var culture in Cultures)
            {
                core.CurrentCulture = culture;
                Assert.AreEqual(10, core.Resources.Count);
                foreach (var resource in core.Resources.Values)
                    Assert.AreEqual(culture, resource.CurrentCulture);
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }

    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr");

    private static ObservableI18nResource<int, string> Resource(string name) =>
        new(name, En, [En, Fr]);

    private sealed class ExternalResource(string name) : II18nResource
    {
        public string ResourceName { get; } = name;
        public ICollection<CultureInfo> Cultures { get; } = new List<CultureInfo> { En, Fr };
        public Action<CultureInfo?>? Setter { get; set; }
        public Exception? ReadError { get; set; }
        public CultureInfo? StoredCulture { get; set; } = En;
        public int Writes { get; private set; }
        public int Disposals { get; private set; }
        public CultureInfo? CurrentCulture
        {
            get => ReadError is { } error ? throw error : StoredCulture;
            set
            {
                Writes++;
                if (Setter is { } setter)
                    setter(value);
                else
                    StoredCulture = value;
            }
        }

        public void Dispose() => Disposals++;
    }

    [TestMethod]
    public void UnsupportedCulture_PrevalidatesEveryResource()
    {
        var core = new I18nCore(En);
        using var first = Resource("first");
        using var last = new ObservableI18nResource<int, string>("last", En);
        core.AddResource(first);
        core.AddResource(last);
        Assert.Throws<ArgumentException>(() => core.CurrentCulture = Fr);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(En, first.CurrentCulture);
        Assert.AreEqual(En, last.CurrentCulture);
    }

    [TestMethod]
    public void FailedAdd_DoesNotRegisterOrUndoExternalCommit()
    {
        var core = new I18nCore(Fr);
        var external = new ExternalResource("external");
        external.Setter = value =>
        {
            external.StoredCulture = value;
            throw new FormatException("After commit.");
        };
        Assert.Throws<Exception>(() => core.AddResource(external));
        Assert.AreEqual(0, core.Resources.Count);
        Assert.AreEqual(Fr, external.CurrentCulture);
        Assert.AreEqual(0, external.Disposals);
    }

    [TestMethod]
    public void RemoveByInstance_RejectsSameNameReplacement()
    {
        var core = new I18nCore(En);
        var registered = new ExternalResource("same");
        var replacement = new ExternalResource("same");
        core.AddResource(registered);
        Assert.IsFalse(core.AddResource(replacement));
        Assert.AreEqual(0, replacement.Writes);
        Assert.IsFalse(core.RemoveResource(replacement));
        Assert.AreSame(registered, core.Resources["same"]);
        Assert.IsTrue(core.RemoveResource(registered));
        Assert.AreEqual(0, registered.Disposals);
    }

    [TestMethod]
    public void ResourceCallbacks_ReadAllCommittedCultures()
    {
        var core = new I18nCore(En);
        using var first = Resource("first");
        using var last = Resource("last");
        core.AddResource(first);
        core.AddResource(last);
        first.CurrentCultureChanged += (_, _) =>
        {
            Assert.AreEqual(Fr, core.CurrentCulture);
            Assert.AreEqual(Fr, last.CurrentCulture);
        };
        core.CurrentCulture = Fr;
    }

    [TestMethod]
    public void ChangedFailure_KeepsEveryControlledResourceCommitted()
    {
        var core = new I18nCore(En);
        using var first = Resource("first");
        using var last = Resource("last");
        core.AddResource(first);
        core.AddResource(last);
        first.CurrentCultureChanged += (_, _) => throw new FormatException("Changed failed.");
        Assert.Throws<Exception>(() => core.CurrentCulture = Fr);
        Assert.AreEqual(Fr, core.CurrentCulture);
        Assert.AreEqual(Fr, first.CurrentCulture);
        Assert.AreEqual(Fr, last.CurrentCulture);
    }

    [TestMethod]
    public void Callback_RejectsReentrantCoreWritesIncludingNoOps()
    {
        var core = new I18nCore(En);
        using var r = Resource("r");
        core.AddResource(r);
        r.CurrentCultureChanged += (_, _) =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => core.CurrentCulture = Fr);
            Assert.ThrowsExactly<InvalidOperationException>(() => core.ClearResource());
            Assert.ThrowsExactly<InvalidOperationException>(() => core.RemoveResource("missing"));
            Assert.ThrowsExactly<InvalidOperationException>(() => core.AddResource(r));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                core.CultureChangeTypes = CultureChangeTypes.None
            );
        };
        core.CurrentCulture = Fr;
    }

    [TestMethod]
    public void CoreSubscriptions_AreCapturedBeforeResourceCallbacks()
    {
        var core = new I18nCore(En);
        using var r = Resource("r");
        core.AddResource(r);
        var oldCalls = 0;
        var lateCalls = 0;
        EventHandler<CultureInfo?> original = (_, _) => oldCalls++;
        core.CurrentCultureChanged += original;
        r.CurrentCultureChanged += (_, _) =>
        {
            core.CurrentCultureChanged -= original;
            core.CurrentCultureChanged += (_, _) => lateCalls++;
        };
        core.CurrentCulture = Fr;
        Assert.AreEqual(1, oldCalls);
        Assert.AreEqual(0, lateCalls);
    }

    [TestMethod]
    public void ExplicitNull_DoesNotInventCulture()
    {
        var core = new I18nCore(null);
        Assert.IsNull(core.CurrentCulture);
        using var empty = new ObservableI18nResource<int, string>("empty");
        Assert.IsTrue(core.AddResource(empty));
        Assert.IsNull(empty.CurrentCulture);
    }

    [TestMethod]
    public void ChangingFailure_PreservesAllStateAndReleasesGuards()
    {
        using var core = new I18nCore(En);
        using var r = Resource("r");
        var external = new ExternalResource("external");
        core.AddResource(r);
        core.AddResource(external);
        var error = new FormatException("Changing failed.");
        System.ComponentModel.PropertyChangingEventHandler handler = (_, args) =>
        {
            Assert.AreEqual(nameof(core.CurrentCulture), args.PropertyName);
            Assert.AreEqual(En, core.CurrentCulture);
            Assert.AreEqual(En, r.CurrentCulture);
            Assert.ThrowsExactly<InvalidOperationException>(() => core.Dispose());
            Assert.ThrowsExactly<InvalidOperationException>(() => r.CurrentCulture = En);
            Assert.ThrowsExactly<InvalidOperationException>(() => r.Dispose());
            throw error;
        };
        core.PropertyChanging += handler;
        Assert.AreSame(
            error,
            Assert.ThrowsExactly<FormatException>(() => core.CurrentCulture = Fr)
        );
        Assert.AreEqual(0, external.Writes);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(En, r.CurrentCulture);
        core.PropertyChanging -= handler;
        core.CurrentCulture = Fr;
        Assert.AreEqual(Fr, r.CurrentCulture);
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void ExternalFailure_ReportsPartialStateWithoutControlledCommit(
        int failing,
        bool commitFirst
    )
    {
        using var core = new I18nCore(En);
        using var controlled = Resource("controlled");
        core.AddResource(controlled);
        var external = Enumerable
            .Range(0, 3)
            .Select(i => new ExternalResource(i.ToString()))
            .ToArray();
        foreach (var r in external)
            core.AddResource(r);
        var original = new FormatException("External setter failed.");
        external[failing].Setter = value =>
        {
            if (commitFirst)
                external[failing].StoredCulture = value;
            throw original;
        };
        var notifications = 0;
        core.CurrentCultureChanged += (_, _) => notifications++;
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.CurrentCulture = Fr
        );
        Assert.AreSame(original, error.InnerException);
        Assert.AreSame(external[failing], error.FailedResource);
        Assert.AreEqual(I18nCultureChangePhase.ExternalSetter, error.Phase);
        Assert.IsFalse(error.CoreCommitted);
        Assert.AreEqual(En, error.CoreCulture);
        Assert.AreEqual(Fr, error.RequestedCulture);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(En, controlled.CurrentCulture);
        Assert.AreEqual(0, notifications);
        for (var i = 0; i < external.Length; i++)
        {
            var expected = i < failing || (i == failing && commitFirst) ? Fr : En;
            Assert.AreEqual(expected, external[i].CurrentCulture);
            var state = error.ResourceStates.Single(state =>
                ReferenceEquals(state.Resource, external[i])
            );
            Assert.AreEqual(expected, state.Culture);
            Assert.IsNull(state.ReadException);
            Assert.AreEqual(i <= failing ? 1 : 0, external[i].Writes);
        }
        external[failing].Setter = null;
        core.CurrentCulture = En;
        Assert.IsTrue(external.All(r => Equals(r.CurrentCulture, En)));
        core.CurrentCulture = Fr;
        Assert.AreEqual(Fr, controlled.CurrentCulture);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void ControlledNotificationFailure_AllResourcesAlreadyCommitted(int failing)
    {
        using var core = new I18nCore(En);
        var resources = Enumerable.Range(0, 3).Select(i => Resource(i.ToString())).ToArray();
        try
        {
            foreach (var r in resources)
                core.AddResource(r);
            var original = new FormatException("Resource notification failed.");
            EventHandler<CultureInfo?> handler = (_, _) => throw original;
            resources[failing].CurrentCultureChanged += handler;
            var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
                core.CurrentCulture = Fr
            );
            Assert.AreEqual(I18nCultureChangePhase.ResourceNotification, error.Phase);
            Assert.AreSame(resources[failing], error.FailedResource);
            Assert.AreSame(original, error.InnerException);
            Assert.IsTrue(error.CoreCommitted);
            Assert.AreEqual(Fr, core.CurrentCulture);
            Assert.IsTrue(error.ResourceStates.All(state => Equals(state.Culture, Fr)));
            Assert.IsTrue(resources.All(r => Equals(r.CurrentCulture, Fr)));
            resources[failing].CurrentCultureChanged -= handler;
            core.CurrentCulture = En;
        }
        finally
        {
            foreach (var r in resources)
                r.Dispose();
        }
    }

    [TestMethod]
    public void CoreChangedFailure_PreservesNewStateAndOriginalException()
    {
        using var core = new I18nCore(En);
        using var r = Resource("r");
        core.AddResource(r);
        var original = new FormatException("Core notification failed.");
        EventHandler<CultureInfo?> handler = (_, _) => throw original;
        core.CurrentCultureChanged += handler;
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.CurrentCulture = Fr
        );
        Assert.AreEqual(I18nCultureChangePhase.CoreNotification, error.Phase);
        Assert.IsNull(error.FailedResource);
        Assert.AreSame(original, error.InnerException);
        Assert.IsTrue(error.CoreCommitted);
        Assert.AreEqual(Fr, r.CurrentCulture);
        core.CurrentCultureChanged -= handler;
        core.CurrentCulture = En;
    }

    [TestMethod]
    public void FailedControlledAdd_RemovesRegistrationButPreservesCommittedCulture()
    {
        using var core = new I18nCore(Fr);
        using var candidate = Resource("candidate");
        EventHandler<CultureInfo?> handler = (_, _) =>
        {
            Assert.AreSame(candidate, core.Resources["candidate"]);
            Assert.AreEqual(Fr, candidate.CurrentCulture);
            Assert.ThrowsExactly<InvalidOperationException>(() => core.RemoveResource(candidate));
            throw new FormatException("Add notification failed.");
        };
        candidate.CurrentCultureChanged += handler;
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.AddResource(candidate)
        );
        Assert.AreEqual(I18nCultureChangePhase.ResourceNotification, error.Phase);
        Assert.IsFalse(error.CoreCommitted);
        Assert.AreEqual(0, core.Resources.Count);
        Assert.AreEqual(Fr, candidate.CurrentCulture);
        Assert.AreEqual(Fr, error.ResourceStates[0].Culture);
        candidate.CurrentCultureChanged -= handler;
        Assert.IsTrue(core.AddResource(candidate));
        Assert.IsFalse(core.AddResource(candidate));
    }

    [TestMethod]
    public void UnsupportedAdd_AndNullArguments_LeaveRegistrationEmpty()
    {
        using var core = new I18nCore(Fr);
        using var r = new ObservableI18nResource<int, string>("r", En);
        Assert.ThrowsExactly<ArgumentException>(() => core.AddResource(r));
        Assert.ThrowsExactly<ArgumentNullException>(() => core.AddResource(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            core.RemoveResource((II18nResource)null!)
        );
        Assert.ThrowsExactly<ArgumentNullException>(() => core.RemoveResource((string)null!));
        Assert.AreEqual(0, core.Resources.Count);
        Assert.AreEqual(En, r.CurrentCulture);
    }

    [TestMethod]
    public void RemoveClearAndDispose_BorrowResourcesWithoutDisposing()
    {
        var core = new I18nCore(En);
        var r = new ExternalResource("r");
        var view = core.Resources;
        core.AddResource(r);
        Assert.IsTrue(core.RemoveResource("r"));
        core.AddResource(r);
        core.ClearResource();
        Assert.AreEqual(0, view.Count);
        core.AddResource(r);
        core.Dispose();
        core.Dispose();
        Assert.AreEqual(0, view.Count);
        Assert.AreEqual(0, r.Disposals);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.AddResource(r));
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.RemoveResource(r));
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.RemoveResource("r"));
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.ClearResource());
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.CurrentCulture = En);
        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            core.CultureChangeTypes = CultureChangeTypes.None
        );
        r.Dispose();
        Assert.AreEqual(1, r.Disposals);
    }

    [TestMethod]
    public void SnapshotIncludesCoreAndAllResourceHandlersBeforeChanging()
    {
        using var core = new I18nCore(En);
        using var first = Resource("first");
        using var second = Resource("second");
        core.AddResource(first);
        core.AddResource(second);
        var calls = new List<string>();
        EventHandler<CultureInfo?> oldResource = (_, _) => calls.Add("old-resource");
        System.ComponentModel.PropertyChangedEventHandler oldCore = (_, _) => calls.Add("old-core");
        second.CurrentCultureChanged += oldResource;
        core.PropertyChanged += oldCore;
        core.PropertyChanging += (_, _) =>
        {
            second.CurrentCultureChanged -= oldResource;
            second.CurrentCultureChanged += (_, _) => calls.Add("late-resource");
            core.PropertyChanged -= oldCore;
            core.PropertyChanged += (_, _) => calls.Add("late-core");
            Assert.ThrowsExactly<InvalidOperationException>(() => second.SetData(0, "no"));
        };
        first.CurrentCultureChanged += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => second.Dispose());
        core.CurrentCulture = Fr;
        Assert.AreSequenceEqual(new[] { "old-resource", "old-core" }, calls);
    }

    [TestMethod]
    public void ExternalSilentRefusal_IsReportedAndReadFailuresAreNotHidden()
    {
        using var core = new I18nCore(En);
        var r = new ExternalResource("r");
        core.AddResource(r);
        r.Setter = _ => { };
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.CurrentCulture = Fr
        );
        Assert.AreEqual(I18nCultureChangePhase.ExternalSetter, error.Phase);
        Assert.AreEqual(En, error.ResourceStates[0].Culture);
        Assert.IsFalse(error.CoreCommitted);
        var getterError = new FormatException("Cannot read actual culture.");
        var setterError = new InvalidOperationException("Setter failed after commit.");
        r.Setter = value =>
        {
            r.StoredCulture = value;
            r.ReadError = getterError;
            throw setterError;
        };
        error = Assert.ThrowsExactly<I18nCultureChangeException>(() => core.CurrentCulture = Fr);
        Assert.AreSame(setterError, error.InnerException);
        Assert.AreSame(getterError, error.ResourceStates[0].ReadException);
        Assert.IsNull(error.ResourceStates[0].Culture);
        r.ReadError = null;
        Assert.AreEqual(Fr, r.CurrentCulture);
    }

    [TestMethod]
    public void CaptureFailure_ReleasesEarlierResourcesAndLeavesExternalUntouched()
    {
        using var core = new I18nCore(En);
        using var first = Resource("first");
        using var last = Resource("last");
        var external = new ExternalResource("external");
        core.AddResource(first);
        core.AddResource(external);
        core.AddResource(last);
        last.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => core.CurrentCulture = Fr);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(0, external.Writes);
        Assert.IsTrue(first.SetData(0, "not frozen"));
        Assert.IsTrue(core.RemoveResource(last));
        core.CurrentCulture = Fr;
        Assert.AreEqual(Fr, first.CurrentCulture);
    }

    [TestMethod]
    public void UnsupportedLastResource_IsCheckedBeforeAnyExternalSetter()
    {
        using var core = new I18nCore(En);
        var external = new ExternalResource("external");
        using var last = new ObservableI18nResource<int, string>("last", En);
        core.AddResource(external);
        core.AddResource(last);
        Assert.ThrowsExactly<ArgumentException>(() => core.CurrentCulture = Fr);
        Assert.AreEqual(0, external.Writes);
        core.RemoveResource(last);
        external.Setter = _ => throw new FormatException("Null rejected by external resource.");
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.CurrentCulture = null
        );
        Assert.IsNull(error.RequestedCulture);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(En, error.ResourceStates[0].Culture);
    }

    [TestMethod]
    public void SilentRefusal_StopsBeforeLaterExternalSetters()
    {
        using var core = new I18nCore(En);
        var first = new ExternalResource("first");
        var last = new ExternalResource("last");
        core.AddResource(first);
        core.AddResource(last);
        first.Setter = _ => { };
        Assert.ThrowsExactly<I18nCultureChangeException>(() => core.CurrentCulture = Fr);
        Assert.AreEqual(0, last.Writes);
    }

    [TestMethod]
    public void NullCultureAndContextFlags_DoNotInventOrChangeContextCulture()
    {
        var original = CultureInfo.CurrentCulture;
        var originalUI = CultureInfo.CurrentUICulture;
        try
        {
            using var core = new I18nCore(En)
            {
                CultureChangeTypes =
                    CultureChangeTypes.CultureInfoCurrentCulture
                    | CultureChangeTypes.CultureInfoCurrentUICulture,
            };
            using var r = Resource("r");
            core.AddResource(r);
            core.CurrentCulture = Fr;
            Assert.AreEqual(Fr, CultureInfo.CurrentCulture);
            Assert.AreEqual(Fr, CultureInfo.CurrentUICulture);
            core.CurrentCulture = null;
            Assert.IsNull(core.CurrentCulture);
            Assert.IsNull(r.CurrentCulture);
            Assert.AreEqual(Fr, CultureInfo.CurrentCulture);
            Assert.AreEqual(Fr, CultureInfo.CurrentUICulture);
            core.CurrentCulture = En;
            Assert.AreEqual(En, CultureInfo.CurrentCulture);
            Assert.AreEqual(En, CultureInfo.CurrentUICulture);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
            CultureInfo.CurrentUICulture = originalUI;
        }
    }

    [TestMethod]
    public void ExternalSuccess_PrecedesControlledCommitAndNotifications()
    {
        using var core = new I18nCore(En);
        using var controlled = Resource("controlled");
        var external = new ExternalResource("external");
        core.AddResource(controlled);
        core.AddResource(external);
        external.Setter = value =>
        {
            Assert.AreEqual(En, core.CurrentCulture);
            Assert.AreEqual(En, controlled.CurrentCulture);
            Assert.ThrowsExactly<InvalidOperationException>(() => core.ClearResource());
            Assert.ThrowsExactly<InvalidOperationException>(() => controlled.SetData(0, "blocked"));
            external.StoredCulture = value;
        };
        controlled.CurrentCultureChanged += (_, _) =>
        {
            Assert.AreEqual(Fr, core.CurrentCulture);
            Assert.AreEqual(Fr, external.CurrentCulture);
        };
        core.CurrentCulture = Fr;
        Assert.AreEqual(Fr, external.CurrentCulture);
    }

    [TestMethod]
    public void LaterExternalSetterChangingEarlierResource_IsDetectedBeforeControlledCommit()
    {
        using var core = new I18nCore(En);
        using var controlled = Resource("controlled");
        var first = new ExternalResource("first");
        var last = new ExternalResource("last");
        core.AddResource(first);
        core.AddResource(controlled);
        core.AddResource(last);
        last.Setter = value =>
        {
            last.StoredCulture = value;
            first.StoredCulture = En;
        };
        var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
            core.CurrentCulture = Fr
        );
        Assert.AreSame(first, error.FailedResource);
        Assert.IsFalse(error.CoreCommitted);
        Assert.AreEqual(En, core.CurrentCulture);
        Assert.AreEqual(En, controlled.CurrentCulture);
        Assert.AreEqual(Fr, last.CurrentCulture);
        Assert.AreEqual(En, first.CurrentCulture);
    }

    private sealed class InvalidUICulture() : CultureInfo("en")
    {
        public override string Name => "invalid/name";
    }

    [TestMethod]
    [DataRow(CultureChangeTypes.CultureInfoCurrentUICulture)]
    [DataRow(
        CultureChangeTypes.CultureInfoCurrentCulture
            | CultureChangeTypes.CultureInfoCurrentUICulture
    )]
    public void ContextSetterFailure_ReportsCommittedResourcesAndActualContext(
        CultureChangeTypes flags
    )
    {
        var original = CultureInfo.CurrentCulture;
        var originalUI = CultureInfo.CurrentUICulture;
        try
        {
            var invalid = new InvalidUICulture();
            using var core = new I18nCore(En) { CultureChangeTypes = flags };
            using var resource = new ObservableI18nResource<int, string>("r", En, [En, invalid]);
            core.AddResource(resource);
            var error = Assert.ThrowsExactly<I18nCultureChangeException>(() =>
                core.CurrentCulture = invalid
            );
            Assert.IsTrue(error.CoreCommitted);
            Assert.AreEqual(I18nCultureChangePhase.ContextCulture, error.Phase);
            Assert.AreSame(invalid, error.CoreCulture);
            Assert.AreSame(invalid, resource.CurrentCulture);
            Assert.AreEqual(originalUI, CultureInfo.CurrentUICulture);
            Assert.AreEqual(originalUI, error.ContextUICulture);
            Assert.AreEqual(
                (flags & CultureChangeTypes.CultureInfoCurrentCulture) != 0 ? invalid : original,
                error.ContextCulture
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
            CultureInfo.CurrentUICulture = originalUI;
        }
    }

    [TestMethod]
    public void SharedResource_NestedCoreOperationIsRejectedBeforeExternalWrites()
    {
        using var a = new I18nCore(En);
        using var b = new I18nCore(En);
        using var r = Resource("shared");
        a.AddResource(r);
        b.AddResource(r);
        a.PropertyChanging += (_, _) =>
            Assert.ThrowsExactly<InvalidOperationException>(() => b.CurrentCulture = Fr);
        a.CurrentCulture = Fr;
        Assert.AreEqual(En, b.CurrentCulture);
        Assert.AreEqual(Fr, r.CurrentCulture);
        b.CurrentCulture = En;
        Assert.AreEqual(En, r.CurrentCulture);
    }

    //[TestMethod]
    //public void ChangeCultureChangeTypes()
    //{
    //    var core = new I18nCore();
    //    Assert.AreEqual(
    //        core.CurrentCulture,
    //        [CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture]
    //    );

    //    var oldCulture = core.CurrentCulture;
    //    core.CultureChangeTypes = CultureChangeTypes.CultureInfoCurrentCulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(core.CurrentCulture, CultureInfo.CurrentCulture);
    //        Assert.AreEqual(oldCulture, CultureInfo.CurrentUICulture);
    //    }

    //    oldCulture = core.CurrentCulture;
    //    core.CultureChangeTypes = CultureChangeTypes.CultureInfoCurrentUICulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(core.CurrentCulture, CultureInfo.CurrentUICulture);
    //        Assert.AreEqual(oldCulture, CultureInfo.CurrentCulture);
    //    }

    //    core.CultureChangeTypes =
    //        CultureChangeTypes.CultureInfoCurrentCulture
    //        | CultureChangeTypes.CultureInfoCurrentUICulture;
    //    foreach (var culture in Cultures)
    //    {
    //        core.CurrentCulture = culture;
    //        Assert.AreEqual(
    //            core.CurrentCulture,
    //            [CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture]
    //        );
    //    }

    //    core.CurrentCulture = CurrentCulture;
    //}
}
