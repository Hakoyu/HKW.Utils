using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤字典
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredDictionary"/></para>
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredDictionaryWrapper<TKey, TValue, TDictionary, TFilteredDictionary>
#pragma warning restore S2436
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary,
        IFilteredCollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary, TFilteredDictionary>,
        IDictionaryWrapper<TKey, TValue, TDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="filteredDictionary">过滤字典</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredDictionaryWrapper(
        TDictionary dictionary,
        TFilteredDictionary filteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(filteredDictionary);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(dictionary);
        ArgumentException.ThrowIfReadOnlyCollection(filteredDictionary);

        SourceDictionary = dictionary;
        FilteredDictionary = filteredDictionary;
        Filter = filter;
        AutoFilter = autoFilter;
        Refresh();
    }
    #endregion

    /// <inheritdoc/>
    public bool AutoFilter { get; set; }

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

    /// <inheritdoc/>
    public TDictionary SourceDictionary { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TDictionary ICollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary>.SourceCollection =>
        SourceDictionary;

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public TFilteredDictionary FilteredDictionary { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredDictionary IFilteredCollectionWrapper<
        KeyValuePair<TKey, TValue>,
        TDictionary,
        TFilteredDictionary
    >.FilteredCollection => FilteredDictionary;

    /// <inheritdoc/>
    public void BatchUpdate(Action<TDictionary> updateAction)
    {
        ArgumentNullException.ThrowIfNull(updateAction);

        var autoFilter = AutoFilter;
        AutoFilter = false;
        try
        {
            updateAction(SourceDictionary);
        }
        finally
        {
            AutoFilter = autoFilter;
        }

        Refresh();
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        FilteredDictionary.Clear();
        if (SourceDictionary.Count > 0)
            FilteredDictionary.AddRange(SourceDictionary.Where(i => Filter(i)));
    }

    #region IDictionary
    /// <inheritdoc/>
    public ICollection<TKey> Keys => SourceDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => SourceDictionary.Values;

    /// <inheritdoc/>
    public int Count => SourceDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceDictionary.IsReadOnly;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => SourceDictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => SourceDictionary.Values;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IDictionary)SourceDictionary).IsFixedSize;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Keys => ((IDictionary)SourceDictionary).Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Values => ((IDictionary)SourceDictionary).Values;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)SourceDictionary).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)SourceDictionary).SyncRoot;

    object? IDictionary.this[object key]
    {
        get => this[(TKey)key];
        set => this[(TKey)key] = (TValue)value!;
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set
        {
            SourceDictionary[key] = value;
            if (AutoFilter is false)
                return;
            if (Filter(new(key, value)) is false)
            {
                FilteredDictionary.Remove(key);
                return;
            }

            FilteredDictionary[key] = value;
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        SourceDictionary.Add(key, value);
        if (AutoFilter is false)
            return;
        if (Filter(new(key, value)))
            FilteredDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public bool TryAdd(TKey key, TValue value)
    {
        var result = SourceDictionary.TryAddX(key, value);
        if (result is false)
            return result;
        if (AutoFilter is false)
            return result;
        if (Filter(new(key, value)))
            FilteredDictionary.Add(key, value);
        return result;
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return SourceDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var result = SourceDictionary.Remove(key);
        if (AutoFilter is false)
            return result;
        if (result)
            FilteredDictionary.Remove(key);
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return SourceDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        SourceDictionary.Add(item);
        if (AutoFilter is false)
            return;
        if (Filter(item))
            FilteredDictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceDictionary.Clear();
        FilteredDictionary.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return SourceDictionary.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        SourceDictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var result = SourceDictionary.Remove(item);
        if (AutoFilter is false)
            return result;
        if (result)
            FilteredDictionary.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return SourceDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceDictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Add(object key, object? value)
    {
        Add((TKey)key, (TValue)value!);
    }

    /// <inheritdoc/>
    bool IDictionary.Contains(object key)
    {
        return ((IDictionary)SourceDictionary).Contains(key);
    }

    /// <inheritdoc/>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)SourceDictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Remove(object key)
    {
        Remove((TKey)key);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)SourceDictionary).CopyTo(array, index);
    }
    #endregion
}
