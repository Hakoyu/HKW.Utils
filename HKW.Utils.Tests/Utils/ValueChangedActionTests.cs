using System.ComponentModel;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests.Utils;

[TestClass]
public class ValueChangedActionTests
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("Key")]
    public void AllPropertyNames_MoveIndexAndDisposeTwice(string? name)
    {
        var source = new I18nObserverTests.Model<int>(0);
        var index = new Dictionary<int, List<ValueChangedAction<int>>>();
        var action = new ValueChangedAction<int>(
            source,
            s => ((I18nObserverTests.Model<int>)s).Key,
            (_, _) => { },
            index
        );
        source.Key = 1;
        source.Raise(name);
        Assert.IsFalse(index.ContainsKey(0));
        Assert.AreSame(action, index[1].Single());
        source.Raise(name);
        Assert.AreEqual(1, index[1].Count);
        action.Dispose();
        action.Dispose();
        source.Key = 2;
        source.Raise(name);
        Assert.AreEqual(0, index.Count);
    }

    [TestMethod]
    public void ComparerEquivalentKeys_KeepSingleCanonicalBucketAndUnrelatedEventsDoNothing()
    {
        var source = new I18nObserverTests.Model<string>("a");
        var index = new Dictionary<string, List<ValueChangedAction<string>>>(
            StringComparer.OrdinalIgnoreCase
        );
        var calls = 0;
        using var action = new ValueChangedAction<string>(
            source,
            s => ((I18nObserverTests.Model<string>)s).Key,
            (_, _) => calls++,
            index
        );
        source.Key = "A";
        source.Raise(null);
        Assert.AreEqual(1, index.Count);
        Assert.AreEqual("a", action.Key);
        Assert.AreEqual(1, index["a"].Count);
        source.Key = "b";
        source.Raise("Unrelated");
        Assert.AreEqual("a", action.Key);
        source.Raise("");
        Assert.AreEqual("b", action.Key);
        Assert.AreEqual(0, calls);
    }

    private sealed class ThrowingSource : INotifyPropertyChanged
    {
        public Action? Reading { get; set; }
        public int Value { get; set; }
        public int Key
        {
            get
            {
                Reading?.Invoke();
                return Value;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Raise() => PropertyChanged?.Invoke(this, new(null));
    }

    [TestMethod]
    public void GetterFailureAndReentry_PreserveIndexAndRecover()
    {
        var source = new ThrowingSource();
        var index = new Dictionary<int, List<ValueChangedAction<int>>>();
        using var action = new ValueChangedAction<int>(
            source,
            s => ((ThrowingSource)s).Key,
            (_, _) => { },
            index
        );
        source.Value = 1;
        source.Reading = () =>
        {
            Assert.ThrowsExactly<InvalidOperationException>(source.Raise);
            Assert.ThrowsExactly<InvalidOperationException>(action.Dispose);
            throw new ApplicationException("Getter failure.");
        };
        Assert.ThrowsExactly<ApplicationException>(source.Raise);
        Assert.AreEqual(0, action.Key);
        Assert.AreSame(action, index[0].Single());
        Assert.AreEqual(1, index.Count);
        source.Reading = null;
        source.Raise();
        Assert.AreEqual(1, action.Key);
        Assert.AreSame(action, index[1].Single());
        Assert.AreEqual(1, index.Count);
    }

    [TestMethod]
    public void MultipleActions_LeaveOtherRegistrationsOnDispose()
    {
        var source = new I18nObserverTests.Model<Guid>(Guid.Empty);
        var index = new Dictionary<Guid, List<ValueChangedAction<Guid>>>();
        var first = new ValueChangedAction<Guid>(
            source,
            s => ((I18nObserverTests.Model<Guid>)s).Key,
            (_, _) => { },
            index
        );
        using var second = new ValueChangedAction<Guid>(
            source,
            s => ((I18nObserverTests.Model<Guid>)s).Key,
            (_, _) => { },
            index
        );
        Assert.AreEqual(2, index[Guid.Empty].Count);
        first.Dispose();
        first.Dispose();
        Assert.AreSame(second, index[Guid.Empty].Single());
        source.Key = Guid.NewGuid();
        source.Raise(null);
        Assert.AreEqual(1, index.Count);
        Assert.AreSame(second, index[source.Key].Single());
    }

    [TestMethod]
    public void NullArguments_DoNotLeaveRegistration()
    {
        var source = new I18nObserverTests.Model<int>(0);
        var index = new Dictionary<int, List<ValueChangedAction<int>>>();
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new ValueChangedAction<int>(
                source,
                s => ((I18nObserverTests.Model<int>)s).Key,
                null!,
                index
            )
        );
        Assert.AreEqual(0, index.Count);
    }
}
