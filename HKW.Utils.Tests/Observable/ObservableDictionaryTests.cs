using System.Collections.ObjectModel;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtilsTests.Observable;

[TestClass]
public sealed class ObservableDictionaryTests : ObservableDictionaryTestsBase
{
    protected override IObservableDictionary<int, string> CreateDictionary() =>
        new ObservableDictionary<int, string>(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString())
        );
}

[TestClass]
public sealed class ObservableDictionaryWrapperTests : ObservableDictionaryTestsBase
{
    protected override IObservableDictionary<int, string> CreateDictionary() =>
        new ObservableDictionaryWrapper<int, string, Dictionary<int, string>>(
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i.ToString()),
            null
        );
}

public abstract class ObservableDictionaryTestsBase
{
    protected abstract IObservableDictionary<int, string> CreateDictionary();

    static IReadOnlyCollection<KeyValuePair<int, string>> _newItems = new ReadOnlyCollection<
        KeyValuePair<int, string>
    >(Enumerable.Range(100, 10).Select(i => KeyValuePair.Create(i, i.ToString())).ToList());

    [TestMethod]
    public void IDictionaryTest()
    {
        IDictionaryTTestUtils.Test(() => (IDictionary<int, string>)CreateDictionary(), _newItems);
    }

    [TestMethod]
    public void ObservableCollectionTest()
    {
        ObservableCollectionUtils.Test(
            () => (IObservableCollection<KeyValuePair<int, string>>)CreateDictionary(),
            _newItems
        );
    }

    [TestMethod]
    public void ChangingOnAdd()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        foreach (var pair in _newItems)
        {
            newPair = pair;
            dictionary.Add(pair.Key, pair.Value);
            cDictionary.Add(pair.Key, pair.Value);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Add, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangingOnAddFail()
    {
        var dictionary = CreateDictionary();

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.Throws<ArgumentException>(() =>
        {
            dictionary.Add(dictionary.First().Key, default!);
        });
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnTryAdd()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        foreach (var pair in _newItems)
        {
            newPair = pair;
            Assert.AreEqual(
                true,
                [
                    dictionary.TryAdd(newPair.Key, newPair.Value),
                    cDictionary.TryAdd(newPair.Key, newPair.Value),
                ]
            );
            Assert.AreEqual(
                false,
                [
                    dictionary.TryAdd(newPair.Key, newPair.Value),
                    cDictionary.TryAdd(newPair.Key, newPair.Value),
                ]
            );
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Add, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangingOnReplace()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();
        var replaceDictionary = dictionary.Reverse().ToDictionary();

        var triggerCount = 0;
        var oldPair = default(KeyValuePair<int, string>);
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        foreach (var pair in dictionary.Keys.Zip(replaceDictionary.Values))
        {
            oldPair = cDictionary.GetPair(pair.Item1)!;
            newPair = KeyValuePair.Create(pair);
            dictionary[newPair.Key] = newPair.Value;
            cDictionary[newPair.Key] = newPair.Value;
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        Assert.HasCount(triggerCount, dictionary);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Replace, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.AreEqual(oldPair, e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangingOnRemove()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();
        var removeDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var removePair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        foreach (var pair in removeDictionary)
        {
            removePair = pair;
            Assert.AreEqual(
                true,
                [dictionary.Remove(removePair.Key), cDictionary.Remove(removePair.Key)]
            );
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        Assert.HasCount(triggerCount, removeDictionary);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Remove, e.Action);
            Assert.IsNull(e.NewPair);
            Assert.AreEqual(removePair, e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangingOnRemoveFail()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        foreach (var pair in _newItems)
        {
            Assert.AreEqual(false, [dictionary.Remove(pair.Key), cDictionary.Remove(pair.Key)]);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangingOnClear()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Clear();
        cDictionary.Clear();
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;

        Assert.AreEqual(1, triggerCount);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanging(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Clear, e.Action);
            Assert.IsNull(e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    #region DictionaryChanged
    [TestMethod]
    public void ChangedOnAdd()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        foreach (var pair in _newItems)
        {
            newPair = pair;
            cDictionary.Add(pair.Key, pair.Value);
            dictionary.Add(pair.Key, pair.Value);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Add, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangedOnAddFail()
    {
        var dictionary = CreateDictionary();

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        Assert.Throws<ArgumentException>(() =>
        {
            dictionary.Add(dictionary.First().Key, default!);
        });
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnTryAdd()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        foreach (var pair in _newItems)
        {
            newPair = pair;
            Assert.AreEqual(
                true,
                [
                    cDictionary.TryAdd(newPair.Key, newPair.Value),
                    dictionary.TryAdd(newPair.Key, newPair.Value),
                ]
            );
            Assert.AreEqual(
                false,
                [
                    cDictionary.TryAdd(newPair.Key, newPair.Value),
                    dictionary.TryAdd(newPair.Key, newPair.Value),
                ]
            );
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        Assert.HasCount(triggerCount, _newItems);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Add, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangedOnReplace()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();
        var replaceDictionary = dictionary.Reverse().ToDictionary();

        var triggerCount = 0;
        var oldPair = default(KeyValuePair<int, string>);
        var newPair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        foreach (var pair in dictionary.Keys.Zip(replaceDictionary.Values))
        {
            oldPair = cDictionary.GetPair(pair.Item1)!;
            newPair = KeyValuePair.Create(pair);
            cDictionary[newPair.Key] = newPair.Value;
            dictionary[newPair.Key] = newPair.Value;
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        Assert.HasCount(triggerCount, dictionary);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Replace, e.Action);
            Assert.AreEqual(newPair, e.NewPair);
            Assert.AreEqual(oldPair, e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangedOnRemove()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();
        var removeDictionary = dictionary.ToDictionary();

        var triggerCount = 0;
        var removePair = default(KeyValuePair<int, string>);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        foreach (var pair in removeDictionary)
        {
            removePair = pair;
            Assert.AreEqual(
                true,
                [cDictionary.Remove(removePair.Key), dictionary.Remove(removePair.Key)]
            );
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        Assert.HasCount(triggerCount, removeDictionary);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Remove, e.Action);
            Assert.IsNull(e.NewPair);
            Assert.AreEqual(removePair, e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    [TestMethod]
    public void ChangedOnRemoveFail()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        foreach (var pair in _newItems)
        {
            Assert.AreEqual(false, [cDictionary.Remove(pair.Key), dictionary.Remove(pair.Key)]);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void ChangedOnClear()
    {
        var dictionary = CreateDictionary();
        var cDictionary = dictionary.ToDictionary();

        var triggerCount = 0;

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        cDictionary.Clear();
        dictionary.Clear();
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;

        Assert.AreEqual(1, triggerCount);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));

        void Dictionary_DictionaryChanged(
            IObservableDictionary<int, string> sender,
            NotifyDictionaryChangeEventArgs<int, string> e
        )
        {
            triggerCount++;
            Assert.AreEqual(DictionaryChangeAction.Clear, e.Action);
            Assert.IsNull(e.NewPair);
            Assert.IsNull(e.OldPair);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    #endregion
}
