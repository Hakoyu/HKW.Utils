using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;

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
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class FilterDictionary<TKey, TValue, TDictionary, TFilteredDictionary>
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary,
        IFilterCollection<KeyValuePair<TKey, TValue>, TDictionary, TFilteredDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="filteredDictionary">过滤字典</param>
    /// <param name="filter">过滤器</param>
    public FilterDictionary(
        TDictionary dictionary,
        TFilteredDictionary filteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter
    )
    {
        Dictionary = dictionary;
        FilteredDictionary = filteredDictionary;
        Filter = filter;
    }

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="getFilteredDictionary">获取过滤字典</param>
    /// <param name="filter">过滤器</param>
    public FilterDictionary(
        TDictionary dictionary,
        Func<TDictionary, TFilteredDictionary> getFilteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter
    )
        : this(dictionary, getFilteredDictionary(dictionary), filter) { }
    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Predicate<KeyValuePair<TKey, TValue>> _filter = null!;

    /// <inheritdoc/>
    public Predicate<KeyValuePair<TKey, TValue>> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }

    /// <summary>
    /// 字典
    /// <para>使用此属性修改字典时不会同步至 <see cref="FilteredDictionary"/></para>
    /// </summary>
    public TDictionary Dictionary { get; }

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public TFilteredDictionary FilteredDictionary { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TDictionary IFilterCollection<
        KeyValuePair<TKey, TValue>,
        TDictionary,
        TFilteredDictionary
    >.Collection => Dictionary;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredDictionary IFilterCollection<
        KeyValuePair<TKey, TValue>,
        TDictionary,
        TFilteredDictionary
    >.FilteredCollection => FilteredDictionary;

    /// <inheritdoc/>
    public void Refresh()
    {
        if (FilteredDictionary.IsReadOnly)
            return;
        FilteredDictionary.Clear();
        if (Filter is null)
            FilteredDictionary.AddRange(Dictionary);
        else if (Dictionary.HasValue())
            FilteredDictionary.AddRange(Dictionary.Where(i => Filter(i)));
    }

    #region IDictionary
    /// <inheritdoc/>
    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)Dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)Dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).IsReadOnly;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys =>
        ((IReadOnlyDictionary<TKey, TValue>)Dictionary).Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
        ((IReadOnlyDictionary<TKey, TValue>)Dictionary).Values;

    /// <inheritdoc/>
    public bool IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Keys => ((IDictionary)Dictionary).Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Values => ((IDictionary)Dictionary).Values;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)Dictionary).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)Dictionary).SyncRoot;

    object? IDictionary.this[object key]
    {
        get => this[(TKey)key];
        set => this[(TKey)key] = (TValue)value!;
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)Dictionary)[key];
        set
        {
            ((IDictionary<TKey, TValue>)Dictionary)[key] = value;
            if (FilteredDictionary.IsReadOnly)
                return;
            if (Filter(new(key, value)) is false)
                return;
            if (FilteredDictionary.ContainsKey(key))
                FilteredDictionary[key] = value;
            else
                Refresh();
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        ((IDictionary<TKey, TValue>)Dictionary).Add(key, value);
        if (FilteredDictionary.IsReadOnly)
            return;
        if (Filter(new(key, value)))
            FilteredDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)Dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var result = ((IDictionary<TKey, TValue>)Dictionary).Remove(key);
        if (FilteredDictionary.IsReadOnly)
            return result;
        if (result)
            FilteredDictionary.Remove(key);
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IDictionary<TKey, TValue>)Dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Add(item);
        if (FilteredDictionary.IsReadOnly)
            return;
        if (Filter(item))
            FilteredDictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Clear();
        if (FilteredDictionary.IsReadOnly)
            return;
        FilteredDictionary.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var result = ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Remove(item);
        if (result || FilteredDictionary.IsReadOnly is false)
            FilteredDictionary.Remove(item);
        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)Dictionary).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Add(object key, object? value)
    {
        Add((TKey)key, (TValue)value!);
    }

    /// <inheritdoc/>
    bool IDictionary.Contains(object key)
    {
        return ((IDictionary)Dictionary).Contains(key);
    }

    /// <inheritdoc/>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)Dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    void IDictionary.Remove(object key)
    {
        Remove((TKey)key);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)Dictionary).CopyTo(array, index);
    }
    #endregion
}
