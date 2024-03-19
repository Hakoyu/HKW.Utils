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
        IDictionary<TKey, TValue> comparisonDictionary,
        Func<KeyValuePair<TKey, TValue>> createNewPair
    )
        where TKey : notnull
    {
        ObservableCollectionTests.Test(dictionary, comparisonDictionary, createNewPair);
        DictionaryChangingOnAdd(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangingOnAdd(
            dictionary,
            comparisonDictionary,
            createNewPair().Key,
            createNewPair().Value
        );
        DictionaryChangingOnAddFalse(dictionary, comparisonDictionary);
        DictionaryChangingOnAddCancel(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangingOnTryAdd(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangingOnTryAddFalse(dictionary, comparisonDictionary);
        DictionaryChangingOnTryAddCancel(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangingOnRemove(dictionary, comparisonDictionary);
        DictionaryChangingOnRemoveFalse(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangingOnRemoveCancel(dictionary, comparisonDictionary);
        DictionaryChangingOnClear(dictionary, comparisonDictionary);
        DictionaryChangingOnClearCancel(dictionary, comparisonDictionary);
        DictionaryChangingOnReplace(dictionary, comparisonDictionary, createNewPair().Value);
        DictionaryChangingOnReplaceCancel(dictionary, comparisonDictionary, createNewPair().Value);

        DictionaryChangedOnAdd(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangedOnAdd(
            dictionary,
            comparisonDictionary,
            createNewPair().Key,
            createNewPair().Value
        );
        DictionaryChangedOnAddFalse(dictionary, comparisonDictionary);
        DictionaryChangedOnTryAdd(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangedOnTryAddFalse(dictionary, comparisonDictionary);
        DictionaryChangedOnRemove(dictionary, comparisonDictionary);
        DictionaryChangedOnRemoveFalse(dictionary, comparisonDictionary, createNewPair());
        DictionaryChangedOnClear(dictionary, comparisonDictionary);
        DictionaryChangedOnReplace(dictionary, comparisonDictionary, createNewPair().Value);
    }

    #region DictionaryChanging
    public static void DictionaryChangingOnAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnAddFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var existPair = cDictionary.Random();
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

    public static void DictionaryChangingOnAddCancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnTryAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnTryAddFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var existPair = cDictionary.Random();
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

    public static void DictionaryChangingOnTryAddCancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnRemove<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangingOnRemoveFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangingOnRemoveCancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangingOnClear<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangingOnClearCancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangingOnReplace<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.Random();
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary[oldPair.Key] = newValue;
        cDictionary[oldPair.Key] = newValue;

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
            Assert.IsTrue(e.NewPair.EqualsContent(new(oldPair.Key, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }

    public static void DictionaryChangingOnReplaceCancel<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.Random();
        dictionary.DictionaryChanging += Dictionary_DictionaryChanging;
        dictionary[oldPair.Key] = newValue;
        Assert.IsTrue(dictionary[oldPair.Key]?.Equals(cDictionary[oldPair.Key]));

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
            Assert.IsTrue(e.NewPair.EqualsContent(new(oldPair.Key, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
            e.Cancel = true;
        }
    }
    #endregion

    #region DictionaryChanged
    public static void DictionaryChangedOnAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangedOnAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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
        cDictionary[newKey] = newValue;
        dictionary[newKey] = newValue;

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

    public static void DictionaryChangedOnAddFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var existPair = cDictionary.Random();
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

    public static void DictionaryChangedOnTryAdd<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangedOnTryAddFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var existPair = cDictionary.Random();
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

    public static void DictionaryChangedOnRemove<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangedOnRemoveFalse<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
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

    public static void DictionaryChangedOnClear<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary
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

    public static void DictionaryChangedOnReplace<TKey, TValue>(
        IObservableDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> comparisonDictionary,
        TValue newValue
    )
        where TKey : notnull
    {
        dictionary.Clear();
        var cDictionary = comparisonDictionary.ToDictionary();
        dictionary.AddRange(cDictionary);

        var triggered = false;
        var oldPair = cDictionary.Random();
        dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
        cDictionary[oldPair.Key] = newValue;
        dictionary[oldPair.Key] = newValue;

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
            Assert.IsTrue(e.NewPair.EqualsContent(new(oldPair.Key, newValue)));
            Assert.IsTrue(e.OldPair.EqualsContent(oldPair));
            Assert.IsTrue(dictionary.SequenceEqual(cDictionary));
        }
    }
    #endregion
}
