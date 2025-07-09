using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 缓存字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class CacheDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary
    where TKey : notnull
{
    /// <summary>
    /// 最大缓存大小
    /// </summary>
    [DefaultValue(16)]
    public int MaxCacheSize { get; set; } = 16;
    private readonly ConcurrentDictionary<TKey, TValue> _dictionary;
    private readonly ConcurrentQueue<TKey> _keyQueue = new();

    #region Ctor
    /// <inheritdoc/>
    public CacheDictionary()
        : this(null!, null, 16) { }

    /// <inheritdoc/>
    /// <param name="maxCacheSize">最大缓存大小</param>
    public CacheDictionary(int maxCacheSize)
        : this(null!, null, maxCacheSize) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="maxCacheSize">最大缓存大小</param>
    public CacheDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, int maxCacheSize)
        : this(collection, null, maxCacheSize) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    /// <param name="maxCacheSize">最大缓存大小</param>
    public CacheDictionary(IEqualityComparer<TKey> comparer, int maxCacheSize)
        : this(null!, comparer, maxCacheSize) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    /// <param name="maxCacheSize">最大缓存大小</param>
    public CacheDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer,
        int maxCacheSize
    )
    {
        if (collection is not null)
        {
            _dictionary = new(collection, comparer);
            foreach (var item in _dictionary)
                _keyQueue.Enqueue(item.Key);
        }
        else
            _dictionary = new(comparer);
        MaxCacheSize = maxCacheSize;
    }
    #endregion

    /// <summary>
    /// 尝试添加到队列
    /// </summary>
    /// <param name="key">键</param>
    public void TryAddToQueue(TKey key)
    {
        if (_keyQueue.Count >= MaxCacheSize)
        {
            if (_keyQueue.TryDequeue(out var dequeueKey))
                _dictionary.TryRemove(dequeueKey, out _);
        }
        _keyQueue.Enqueue(key);
    }

    #region IDictionary
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)_dictionary)[key];
        set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value;
    }

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

    ICollection IDictionary.Keys => ((IDictionary)_dictionary).Keys;

    ICollection IDictionary.Values => ((IDictionary)_dictionary).Values;

    /// <inheritdoc/>
    public bool IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

    /// <inheritdoc/>
    public object? this[object key]
    {
        get => ((IDictionary)_dictionary)[key];
        set => this[(TKey)key] = (TValue)value!;
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        var count = Count;
        ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
        if (count != Count)
            TryAddToQueue(key);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        var count = Count;
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        if (count != Count)
            TryAddToQueue(item.Key);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        _keyQueue.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        throw new NotSupportedException("Remove operation is not supported in CacheDictionary.");
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotSupportedException("Remove operation is not supported in CacheDictionary.");
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(object key, object? value)
    {
        var count = Count;
        ((IDictionary)_dictionary).Add(key, value);
        if (count != Count)
            TryAddToQueue((TKey)key);
    }

    /// <inheritdoc/>
    public bool Contains(object key)
    {
        return ((IDictionary)_dictionary).Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    public void Remove(object key)
    {
        throw new NotSupportedException("Remove operation is not supported in CacheDictionary.");
    }

    /// <inheritdoc/>
    public void CopyTo(Array array, int index)
    {
        ((ICollection)_dictionary).CopyTo(array, index);
    }
    #endregion
}
