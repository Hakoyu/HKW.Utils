using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;
using HKW.HKWUtils.Observable;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读过滤字典
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredDictionary"/></para>
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
#pragma warning disable S2436
public class ReadOnlyFilterDictionary<TKey, TValue, TFilteredDictionary>
#pragma warning restore S2436
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary,
        IFilterCollection<
            KeyValuePair<TKey, TValue>,
            IObservableDictionary<TKey, TValue>,
            TFilteredDictionary
        >,
        IDisposable
    where TKey : notnull
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    private readonly IObservableDictionary<TKey, TValue> _dictionary;

    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="filteredDictionary">过滤字典</param>
    /// <param name="filter">过滤器</param>
    public ReadOnlyFilterDictionary(
        IObservableDictionary<TKey, TValue> dictionary,
        TFilteredDictionary filteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter
    )
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(filteredDictionary);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(dictionary);
        ArgumentException.ThrowIfReadOnlyCollection(filteredDictionary);

        _dictionary = dictionary;
        FilteredDictionary = filteredDictionary;
        Filter = filter;

        _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        _dictionary.DictionaryChanged += Dictionary_DictionaryChanged;
    }

    private void Dictionary_DictionaryChanged(
        IObservableDictionary<TKey, TValue> sender,
        NotifyDictionaryChangeEventArgs<TKey, TValue> e
    )
    {
        if (e.Action is DictionaryChangeAction.Add)
        {
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            if (Filter(newPair) is false)
                return;
            FilteredDictionary.Add(newPair);
        }
        else if (e.Action is DictionaryChangeAction.Remove)
        {
            if (e.TryGetOldPair(out var oldPair) is false)
                return;
            FilteredDictionary.Remove(oldPair);
        }
        else if (e.Action is DictionaryChangeAction.Replace)
        {
            if (e.TryGetOldPair(out var oldPair) is false)
                return;
            if (e.TryGetNewPair(out var newPair) is false)
                return;
            if (Filter(newPair) is false)
            {
                FilteredDictionary.Remove(oldPair);
                return;
            }
            FilteredDictionary[oldPair.Key] = newPair.Value;
        }
        else if (e.Action is DictionaryChangeAction.Clear)
        {
            FilteredDictionary.Clear();
        }
    }
    #endregion

    #region IDisposable
    private bool _disposed;

    /// <inheritdoc/>
    ~ReadOnlyFilterDictionary()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _dictionary.DictionaryChanged -= Dictionary_DictionaryChanged;
        }

        _disposed = true;
    }
    #endregion
    /// <inheritdoc/>
    bool IFilterCollection<
        KeyValuePair<TKey, TValue>,
        IObservableDictionary<TKey, TValue>,
        TFilteredDictionary
    >.AutoFilter { get; set; } = true;

    /// <inheritdoc/>
    public Predicate<KeyValuePair<TKey, TValue>> Filter
    {
        get => field;
        set
        {
            field = value;
            Refresh();
        }
    }

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public TFilteredDictionary FilteredDictionary { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IObservableDictionary<TKey, TValue> IFilterCollection<
        KeyValuePair<TKey, TValue>,
        IObservableDictionary<TKey, TValue>,
        TFilteredDictionary
    >.SourceCollection => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredDictionary IFilterCollection<
        KeyValuePair<TKey, TValue>,
        IObservableDictionary<TKey, TValue>,
        TFilteredDictionary
    >.FilteredCollection => FilteredDictionary;

    /// <inheritdoc/>
    public void Refresh()
    {
        FilteredDictionary.Clear();
        if (Filter is null)
            FilteredDictionary.AddRange(_dictionary);
        else if (_dictionary.HasValue)
            FilteredDictionary.AddRange(_dictionary.Where(i => Filter(i)));
    }

    #region IDictionary
    /// <inheritdoc/>
    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys =>
        ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
        ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IDictionary)_dictionary).IsFixedSize;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Keys => ((IDictionary)_dictionary).Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Values => ((IDictionary)_dictionary).Values;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

    object? IDictionary.this[object key]
    {
        get => this[(TKey)key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)_dictionary)[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Add(object key, object? value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IDictionary.Contains(object key)
    {
        return ((IDictionary)_dictionary).Contains(key);
    }

    /// <inheritdoc/>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Remove(object key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void IDictionary.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_dictionary).CopyTo(array, index);
    }
    #endregion
}
