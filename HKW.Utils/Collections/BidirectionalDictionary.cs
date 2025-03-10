﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 双向字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class BidirectionalDictionary<T1, T2> : IDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> _dictionary1;
    private readonly Dictionary<T2, T1> _dictionary2;

    /// <inheritdoc/>
    /// <param name="dictionary1">字典1</param>
    /// <param name="dictionary2">字典2</param>
    public BidirectionalDictionary(Dictionary<T1, T2> dictionary1, Dictionary<T2, T1> dictionary2)
    {
        _dictionary1 = dictionary1;
        _dictionary2 = dictionary2;
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    public BidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>> keyValuePairs)
    {
        _dictionary1 = new(keyValuePairs);
        _dictionary2 = new(keyValuePairs.Select(x => KeyValuePair.Create(x.Value, x.Key)));
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    public BidirectionalDictionary(IEnumerable<(T1, T2)> keyValuePairs)
    {
        _dictionary1 = new(keyValuePairs.Select(x => KeyValuePair.Create(x.Item1, x.Item2)));
        _dictionary2 = new(keyValuePairs.Select(x => KeyValuePair.Create(x.Item2, x.Item1)));
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => ((IDictionary<T1, T2>)_dictionary1).Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => ((IDictionary<T1, T2>)_dictionary1).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).IsReadOnly;

    /// <summary>
    /// 字典1
    /// </summary>
    public IReadOnlyDictionary<T1, T2> Dictionary1 => _dictionary1;

    /// <summary>
    /// 字典2
    /// </summary>
    public IReadOnlyDictionary<T2, T1> Dictionary2 => _dictionary2;

    #region Dictionary1
    /// <inheritdoc/>
    public T2 this[T1 key]
    {
        get => ((IDictionary<T1, T2>)_dictionary1)[key];
        set => ((IDictionary<T1, T2>)_dictionary1)[key] = value;
    }

    /// <inheritdoc/>
    public void Add(T1 key, T2 value)
    {
        ((IDictionary<T1, T2>)_dictionary1).Add(key, value);
        ((IDictionary<T2, T1>)_dictionary2).Add(value, key);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<T1, T2> item)
    {
        ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).Add(item);
        ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).Add(
            KeyValuePair.Create(item.Value, item.Key)
        );
    }

    /// <inheritdoc/>
    public bool TryAdd(T1 key, T2 value)
    {
        var result = ((IDictionary<T1, T2>)_dictionary1).TryAdd(key, value);
        if (result)
        {
            if (((IDictionary<T2, T1>)_dictionary2).TryAdd(value, key) is false)
            {
                _dictionary1.Remove(key);
                return false;
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TryAdd(KeyValuePair<T1, T2> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).Clear();
        ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item)
    {
        return ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T1 key)
    {
        return ((IDictionary<T1, T2>)_dictionary1).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T1, T2>>)_dictionary1).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(T1 key)
    {
        var result = ((IDictionary<T1, T2>)_dictionary1).TryGetValue(key, out var value);
        if (result)
        {
            ((IDictionary<T1, T2>)_dictionary1).Remove(key);
            ((IDictionary<T2, T1>)_dictionary2).Remove(value!);
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T1, T2> item)
    {
        return Remove(item.Key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
    {
        return ((IDictionary<T1, T2>)_dictionary1).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary1).GetEnumerator();
    }
    #endregion

    #region Dictionary2
    /// <inheritdoc/>
    public T1 this[T2 key]
    {
        get => ((IDictionary<T2, T1>)_dictionary2)[key];
        set => ((IDictionary<T2, T1>)_dictionary2)[key] = value;
    }

    /// <inheritdoc/>
    public void Add(T2 key, T1 value)
    {
        ((IDictionary<T1, T2>)_dictionary2).Add(KeyValuePair.Create(value, key));
        ((IDictionary<T2, T1>)_dictionary2).Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<T2, T1> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T2, T1> item)
    {
        return ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T2 key)
    {
        return ((IDictionary<T2, T1>)_dictionary2).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(T2 key)
    {
        var result = ((IDictionary<T2, T1>)_dictionary2).TryGetValue(key, out var value);
        if (result)
        {
            ((IDictionary<T1, T2>)_dictionary1).Remove(value!);
            ((IDictionary<T2, T1>)_dictionary2).Remove(key);
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T2, T1> item)
    {
        return Remove(item.Key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value)
    {
        return ((IDictionary<T2, T1>)_dictionary2).TryGetValue(key, out value);
    }

    #endregion
}
