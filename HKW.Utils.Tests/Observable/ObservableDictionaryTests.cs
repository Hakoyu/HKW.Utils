using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Tests.Observable;

[TestClass]
public class ObservableDictionaryTests
{
    [TestMethod]
    public void Test()
    {
        Test<int, int>(
            new ObservableDictionary<int, int>(),
            Enumerable.Range(1, 10).ToDictionary(i => i, i => i),
            () =>
            {
                var value = Random.Shared.Next(100, 1000);
                return new(value, value);
            }
        );
    }

    public static void Test<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        Func<KeyValuePair<TKey, TValue>> createNewPair
    )
        where TKey : notnull
    {
        if (comparisonDictionary.HasValue() is false)
            throw new ArgumentException(
                "ComparisonDictionary must has value",
                nameof(comparisonDictionary)
            );
        //ObservableCollectionTests.Test(dictionary);
        Adding(dictionary, comparisonDictionary, createNewPair());
        Adding(dictionary, comparisonDictionary, createNewPair().Key, createNewPair().Value);
        Adding_False(dictionary, comparisonDictionary, comparisonDictionary.Random());
        Adding_Cancel(dictionary, comparisonDictionary, createNewPair());
        TryAdding(dictionary, comparisonDictionary, createNewPair());
        TryAdding_False(dictionary, comparisonDictionary, comparisonDictionary.Random());
        TryAdding_Cancel(dictionary, comparisonDictionary, createNewPair());
        Removing(dictionary, comparisonDictionary);
        Removing_False(dictionary, comparisonDictionary, createNewPair());
        Removing_Cancel(dictionary, comparisonDictionary);
        Clearing(dictionary, comparisonDictionary);
        Clearing_Cancel(dictionary, comparisonDictionary);
        Replacing(
            dictionary,
            comparisonDictionary,
            comparisonDictionary.Random().Key,
            createNewPair().Value
        );
        Replacing_Cancel(
            dictionary,
            comparisonDictionary,
            comparisonDictionary.Random().Key,
            createNewPair().Value
        );

        Added(dictionary, comparisonDictionary, createNewPair());
        Added_False(dictionary, comparisonDictionary, comparisonDictionary.Random());
        TryAdded(dictionary, comparisonDictionary, createNewPair());
        TryAdded_False(dictionary, comparisonDictionary, comparisonDictionary.Random());
        Removed(dictionary, comparisonDictionary);
        Removed_False(dictionary, comparisonDictionary, createNewPair());
        Cleared(dictionary, comparisonDictionary);
        Replaced(
            dictionary,
            comparisonDictionary,
            comparisonDictionary.Random().Key,
            createNewPair().Value
        );
    }

    #region DictionaryChanging
    public static void Adding<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Add(newPair.Key, newPair.Value);
        cDictionary.Add(newPair.Key, newPair.Value);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Adding<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        TKey newKey,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var newPair = KeyValuePair.Create(newKey, newValue);
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary[newKey] = newValue;
        cDictionary[newKey] = newValue;

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Adding_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> existPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        try
        {
            dictionary.Add(existPair.Key, existPair.Value);
            Assert.Fail();
        }
        catch { }

        try
        {
            cDictionary.Add(existPair.Key, existPair.Value);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void Adding_Cancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Add(newPair.Key, newPair.Value);
        cDictionary.Add(newPair.Key, newPair.Value);

        cDictionary.Remove(newPair.Key);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }

    public static void TryAdding<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.IsTrue(dictionary.TryAdd(newPair.Key, newPair.Value));
        Assert.IsTrue(cDictionary.TryAdd(newPair.Key, newPair.Value));

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void TryAdding_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> existPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.IsTrue(dictionary.TryAdd(existPair.Key, existPair.Value) is false);
        Assert.IsTrue(cDictionary.TryAdd(existPair.Key, existPair.Value) is false);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void TryAdding_Cancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.IsTrue(dictionary.TryAdd(newPair.Key, newPair.Value));
        Assert.IsTrue(cDictionary.TryAdd(newPair.Key, newPair.Value));

        cDictionary.Remove(newPair.Key);
        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }

    public static void Removing<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var removePair = cDictionary.Random();
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.IsTrue(dictionary.Remove(removePair.Key));
        Assert.IsTrue(cDictionary.Remove(removePair.Key));

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Remove);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair.EqualsContent(removePair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Removing_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> nonExeistPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        Assert.IsTrue(dictionary.Remove(nonExeistPair.Key) is false);
        Assert.IsTrue(cDictionary.Remove(nonExeistPair.Key) is false);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void Removing_Cancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var removePair = cDictionary.Random();
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Remove(removePair.Key);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Remove);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair.EqualsContent(removePair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }

    public static void Clearing<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Clear();
        cDictionary.Clear();

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Clear);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Clearing_Cancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary.Clear();

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Clear);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }

    public static void Replacing<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        TKey existKey,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.GetPair(existKey);
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary[existKey] = newValue;
        cDictionary[existKey] = newValue;

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewPair.EqualsContent(new(existKey, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Replacing_Cancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        TKey existKey,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.GetPair(existKey);
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary[existKey] = newValue;
        Assert.IsTrue(dictionary[existKey]?.Equals(cDictionary[existKey]));

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanging -= Dictionary_DictionaryChanging;
        dictionary.Clear();

        void Dictionary_DictionaryChanging(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangingEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewPair.EqualsContent(new(existKey, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }
    #endregion

    #region DictionaryChanged
    public static void Added<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        cDictionary.Add(newPair.Key, newPair.Value);
        dictionary.Add(newPair.Key, newPair.Value);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Added<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        TKey newKey,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var newPair = KeyValuePair.Create(newKey, newValue);
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        dictionary[newKey] = newValue;
        cDictionary[newKey] = newValue;

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Added_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> existPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        try
        {
            cDictionary.Add(existPair.Key, existPair.Value);
            Assert.Fail();
        }
        catch { }

        try
        {
            dictionary.Add(existPair.Key, existPair.Value);
            Assert.Fail();
        }
        catch { }

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void TryAdded<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        Assert.IsTrue(cDictionary.TryAdd(newPair.Key, newPair.Value));
        Assert.IsTrue(dictionary.TryAdd(newPair.Key, newPair.Value));

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Add);
            Assert.IsTrue(e.NewPair.EqualsContent(newPair));
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void TryAdded_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> existPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        Assert.IsTrue(cDictionary.TryAdd(existPair.Key, existPair.Value) is false);
        Assert.IsTrue(dictionary.TryAdd(existPair.Key, existPair.Value) is false);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void Removed<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var removePair = cDictionary.Random();
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        Assert.IsTrue(cDictionary.Remove(removePair.Key));
        Assert.IsTrue(dictionary.Remove(removePair.Key));

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Remove);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair.EqualsContent(removePair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Removed_False<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        KeyValuePair<TKey, TValue> newPair
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        Assert.IsTrue(cDictionary.Remove(newPair.Key) is false);
        Assert.IsTrue(dictionary.Remove(newPair.Key) is false);

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            Assert.Fail();
        }
    }

    public static void Cleared<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        cDictionary.Clear();
        dictionary.Clear();

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Clear);
            Assert.IsTrue(e.NewPair is null);
            Assert.IsTrue(e.OldPair is null);
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void Replaced<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> comparisonDictionary,
        TKey existKey,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.GetPair(existKey);
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        cDictionary[existKey] = newValue;
        dictionary[existKey] = newValue;

        Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        Assert.IsTrue(triggered);
        dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        dictionary.Clear();

        void Dictionary_DictionaryChanged(
            IObservableDictionary<TKey, TValue> sender,
            NotifyDictionaryChangedEventArgs<TKey, TValue> e
        )
        {
            triggered = true;
            Assert.IsTrue(e.Action is DictionaryChangeAction.Replace);
            Assert.IsTrue(e.NewPair.EqualsContent(new(existKey, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }
    #endregion
}
