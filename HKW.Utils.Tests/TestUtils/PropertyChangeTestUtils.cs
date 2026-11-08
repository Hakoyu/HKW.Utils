using System.ComponentModel;
using HKW.HKWUtils;

namespace HKW.HKWUtilsTests;

public static class PropertyChangeTestUtils
{
    public static void AssertChanges(
        INotifyPropertyChanged source,
        Action operation,
        params string[] expected
    )
    {
        var actual = new List<string?>();
        void Changed(object? sender, PropertyChangedEventArgs e)
        {
            Assert.AreSame(source, sender);
            actual.Add(e.PropertyName);
        }
        source.PropertyChanged += Changed;
        try
        {
            operation();
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }
        finally
        {
            source.PropertyChanged -= Changed;
        }
    }
}

// 使用有限次数的调用检查通知数量与顺序，避免循环枚举器的无界遍历。
[TestClass]
public class ObservableCyclicEnumeratorEventContractTests
{
    [TestMethod]
    [DataRow("List")]
    [DataRow("Dictionary")]
    [DataRow("HashSet")]
    public void MoveNext_EmitsExactlyTwoOrderedNotifications(string kind)
    {
        var values = CreateValues(kind);
        var expected = values.ToArray();
        Assert.AreEqual(3, expected.Length);
        using var enumerator = ObservableCyclicEnumerator.Create(values);
        for (var index = 0; index < expected.Length; index++)
        {
            PropertyChangeTestUtils.AssertChanges(
                enumerator,
                () => Assert.IsTrue(enumerator.MoveNext()),
                "Current",
                "CurrentIndex"
            );
            Assert.AreEqual(index, enumerator.CurrentIndex);
            Assert.AreEqual(expected[index], enumerator.Current);
        }
        PropertyChangeTestUtils.AssertChanges(
            enumerator,
            () => Assert.IsFalse(enumerator.MoveNext())
        );
    }

    [TestMethod]
    [DataRow("List")]
    [DataRow("Dictionary")]
    [DataRow("HashSet")]
    public void Reset_NotifiesChangedIndex(string kind)
    {
        using var enumerator = ObservableCyclicEnumerator.Create(CreateValues(kind));
        Assert.IsTrue(enumerator.MoveNext());
        var indexChanges = 0;
        enumerator.PropertyChanged += (sender, e) =>
        {
            Assert.AreSame(enumerator, sender);
            if (e.PropertyName == nameof(enumerator.CurrentIndex))
                indexChanges++;
        };
        enumerator.Reset();
        Assert.AreEqual(-1, enumerator.CurrentIndex);
        Assert.AreEqual(1, indexChanges);
    }

    [TestMethod]
    public void EventAssertion_RejectsMissingAndExtraNotifications()
    {
        var source = new EventSource();
        Assert.ThrowsExactly<AssertFailedException>(() =>
            PropertyChangeTestUtils.AssertChanges(source, () => { }, "Value")
        );
        Assert.ThrowsExactly<AssertFailedException>(() =>
            PropertyChangeTestUtils.AssertChanges(
                source,
                () =>
                {
                    source.Raise();
                    source.Raise();
                },
                "Value"
            )
        );
        PropertyChangeTestUtils.AssertChanges(source, source.Raise, "Value");
        PropertyChangeTestUtils.AssertChanges(source, () => { });
    }

    private static IEnumerable<KeyValuePair<int, int>> CreateValues(string kind)
    {
        var pairs = new[]
        {
            KeyValuePair.Create(1, 10),
            KeyValuePair.Create(2, 20),
            KeyValuePair.Create(3, 30),
        };
        return kind switch
        {
            "List" => pairs.ToList(),
            "Dictionary" => pairs.ToDictionary(pair => pair.Key, pair => pair.Value),
            "HashSet" => pairs.ToHashSet(),
            _ => throw new ArgumentException("Unsupported test collection.", nameof(kind)),
        };
    }

    private sealed class EventSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Raise() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
    }
}
