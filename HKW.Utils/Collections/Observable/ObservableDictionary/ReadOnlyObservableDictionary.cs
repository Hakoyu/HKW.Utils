using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Natives;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读可观察字典接口
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ReadOnlyObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IObservableDictionary<TKey, TValue> _dictionary;

    /// <inheritdoc/>
    /// <param name="dictionary"></param>
    public ReadOnlyObservableDictionary(IObservableDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    /// <inheritdoc/>
    public TValue this[TKey key] => ((IReadOnlyDictionary<TKey, TValue>)_dictionary)[key];

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Count;

    /// <inheritdoc/>
    public IObservableList<TKey> ObservableKeys => _dictionary.ObservableKeys;

    /// <inheritdoc/>
    public IObservableList<TValue> ObservableValues => _dictionary.ObservableValues;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<TKey> IObservableDictionary<TKey, TValue>.Comparer => _dictionary.Comparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableDictionary<TKey, TValue>.ObservableKeysAndValues
    {
        get => _dictionary.ObservableKeysAndValues;
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObservableCollection<KeyValuePair<TKey, TValue>>.TriggerRemoveActionOnClear
    {
        get => _dictionary.TriggerRemoveActionOnClear;
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IReadOnlyDictionary<TKey, TValue>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    IList<KeyValuePair<TKey, TValue>> IObservableDictionary<TKey, TValue>.TryAddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> items
    )
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IObservableCollection<KeyValuePair<TKey, TValue>>.AddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> items
    )
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
        KeyValuePair<TKey, TValue>[] array,
        int arrayIndex
    )
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    #region Event
    event XCancelEventHandler<
        NotifyDictionaryChangingEventArgs<TKey, TValue>
    >? INotifyDictionaryChanging<TKey, TValue>.DictionaryChanging
    {
        add => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
        remove => throw new NotImplementedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyDictionaryChangingEventArgs<TKey, TValue>>? DictionaryChanged
    {
        add => _dictionary.DictionaryChanged += value;
        remove => _dictionary.DictionaryChanged -= value;
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _dictionary.CollectionChanged += value;
        remove => _dictionary.CollectionChanged -= value;
    }

    /// <inheritdoc/>

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _dictionary.PropertyChanged += value;
        remove => _dictionary.PropertyChanged -= value;
    }
    #endregion
}
