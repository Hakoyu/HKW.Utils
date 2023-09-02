using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
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
    : IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IObservableDictionary<TKey, TValue> r_dictionary;

    /// <inheritdoc/>
    /// <param name="dictionary"></param>
    public ReadOnlyObservableDictionary(IObservableDictionary<TKey, TValue> dictionary)
    {
        r_dictionary = dictionary;
    }

    /// <inheritdoc/>
    public TValue this[TKey key] => ((IReadOnlyDictionary<TKey, TValue>)r_dictionary)[key];

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).Keys;

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).Count;

    /// <inheritdoc/>
    public IReadOnlyObservableList<TKey> ObservableKeys => r_dictionary.ObservableKeys;

    /// <inheritdoc/>
    public IReadOnlyObservableList<TValue> ObservableValues => r_dictionary.ObservableValues;

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IReadOnlyDictionary<TKey, TValue>)r_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>>? DictionaryChanged
    {
        add { r_dictionary.DictionaryChanged += value; }
        remove { r_dictionary.DictionaryChanged -= value; }
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<
        NotifyDictionaryChangingEventArgs<TKey, TValue>
    >? DictionaryChanging
    {
        add { r_dictionary.DictionaryChanging += value; }
        remove { r_dictionary.DictionaryChanging -= value; }
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add { r_dictionary.CollectionChanged += value; }
        remove { r_dictionary.CollectionChanged -= value; }
    }

    /// <inheritdoc/>

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add { r_dictionary.PropertyChanged += value; }
        remove { r_dictionary.PropertyChanged -= value; }
    }
}
