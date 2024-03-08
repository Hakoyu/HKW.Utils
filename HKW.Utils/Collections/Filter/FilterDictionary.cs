using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class FilterDictionary<TKey, TValue, TFilteredDictionary> : IDictionary<TKey, TValue>
    where TKey : notnull
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    private Predicate<KeyValuePair<TKey, TValue>> _filter = null!;

    /// <summary>
    /// 过滤器
    /// </summary>
    public required Predicate<KeyValuePair<TKey, TValue>> Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            Refresh();
        }
    }
    private TFilteredDictionary _filteredDictionary = default!;

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public required TFilteredDictionary FilteredDictionary
    {
        get => _filteredDictionary;
        init
        {
            _filteredDictionary = value;
            Refresh();
        }
    }

    #region Ctor
    /// <inheritdoc/>
    public FilterDictionary()
        : this(null!, null) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public FilterDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : this(collection, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public FilterDictionary(IEqualityComparer<TKey> comparer)
        : this(null!, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public FilterDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
    {
        if (collection is not null)
            _dictionary = new(collection, comparer);
        else
            _dictionary = new(comparer);
    }
    #endregion
    /// <summary>
    /// 刷新过滤字典
    /// </summary>
    public void Refresh()
    {
        if (FilteredDictionary is null || Filter is null)
            return;
        FilteredDictionary.Clear();
        if (_dictionary.HasValue())
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

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)_dictionary)[key];
        set
        {
            ((IDictionary<TKey, TValue>)_dictionary)[key] = value;
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
        ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
        if (Filter(new(key, value)))
            FilteredDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var result = ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
        if (result)
            FilteredDictionary.Remove(key);
        return result;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        if (Filter(item))
            FilteredDictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        FilteredDictionary.Clear();
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
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var result = ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
        if (result)
            FilteredDictionary.Remove(item);
        return result;
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
    #endregion
}
