using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 双向字典
/// </summary>
/// <typeparam name="T1">项目类型1</typeparam>
/// <typeparam name="T2">项目类型2</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class BidirectionalDictionary<T1, T2> : IDictionary<T1, T2>, IReadOnlyDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> _dictionary1;
    private readonly Dictionary<T2, T1> _dictionary2;

    /// <inheritdoc/>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
        : this(16, comparer1, comparer2) { }

    /// <inheritdoc/>
    /// <param name="capacity">初始容量</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        int capacity,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        var capacity = keyValuePairs is ICollection<KeyValuePair<T1, T2>> collection
            ? collection.Count
            : 16;
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);
        foreach (var kv in keyValuePairs)
        {
            _dictionary1.Add(kv.Key, kv.Value);
            _dictionary2.Add(kv.Value, kv.Key);
        }
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对集合</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    public BidirectionalDictionary(
        IEnumerable<(T1, T2)> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        var capacity = keyValuePairs is ICollection<(T1, T2)> collection ? collection.Count : 16;
        _dictionary1 = new(capacity, comparer1);
        _dictionary2 = new(capacity, comparer2);

        foreach (var kv in keyValuePairs)
        {
            _dictionary1.Add(kv.Item1, kv.Item2);
            _dictionary2.Add(kv.Item2, kv.Item1);
        }
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => _dictionary1.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => _dictionary1.Values;

    /// <inheritdoc/>
    public int Count => _dictionary1.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// 字典1
    /// </summary>
    public ReadOnlyDictionary<T1, T2> Dictionary1 => field ??= _dictionary1.AsReadOnly();

    /// <summary>
    /// 字典2
    /// </summary>
    public ReadOnlyDictionary<T2, T1> Dictionary2 => field ??= _dictionary2.AsReadOnly();

    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => Keys;

    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => Values;

    #region Dictionary1
    /// <inheritdoc/>
    public T2 this[T1 key]
    {
        get => _dictionary1[key];
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <inheritdoc/>
    void IDictionary<T1, T2>.Add(T1 key, T2 value)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item)
    {
        throw new UseAlternativeMethodException(nameof(this.TryAdd));
    }

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否设置成功</returns>
    public bool TrySetValue(T1 key, T2 value)
    {
        ref var d1ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary1, key);
        if (Unsafe.IsNullRef(ref d1ValueRef))
            return false;
        var d1Value = d1ValueRef;
        if (_dictionary2.ContainsKey(value))
            return false;
        if (
            _dictionary2.TryGetValue(d1Value, out var d2v) is false
            || _dictionary1.Comparer.Equals(key, d2v) is false
        )
            return false;

        d1ValueRef = value;
        _dictionary2.Remove(d1Value);
        _dictionary2.Add(value, key);
        return true;
    }

    /// <inheritdoc/>
    public bool TryAdd(T1 key, T2 value)
    {
        var result = _dictionary1.TryAdd(key, value);
        if (result && _dictionary2.TryAdd(value, key) is false)
        {
            _dictionary1.Remove(key);
            return false;
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TryAdd(KeyValuePair<T1, T2> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool Remove(T1 key)
    {
        if (_dictionary1.Remove(key, out var value) is false)
            return false;

        _dictionary2.Remove(value);
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dictionary1.Clear();
        _dictionary2.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item)
    {
        return _dictionary1.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T1 key)
    {
        return _dictionary1.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T1, T2> item)
    {
        if (
            _dictionary1.TryGetValue(item.Key, out var value) is false
            || _dictionary2.Comparer.Equals(item.Value, value) is false
        )
            return false;

        _dictionary1.Remove(item.Key);
        _dictionary2.Remove(value);
        return true;
    }

    /// <inheritdoc/>
    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
    {
        return _dictionary1.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return _dictionary1.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary1.GetEnumerator();
    }
    #endregion

    #region Dictionary2
    /// <inheritdoc/>
    public T1 this[T2 key]
    {
        get => _dictionary2[key];
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否设置成功</returns>
    public bool TrySetValue(T2 key, T1 value)
    {
        ref var d2ValueRef = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary2, key);
        if (Unsafe.IsNullRef(ref d2ValueRef))
            return false;
        var d2Value = d2ValueRef;
        if (_dictionary1.ContainsKey(value))
            return false;
        if (
            _dictionary1.TryGetValue(d2Value, out var d1v) is false
            || _dictionary2.Comparer.Equals(key, d1v) is false
        )
            return false;

        d2ValueRef = value;
        _dictionary1.Remove(d2Value);
        _dictionary1.Add(value, key);
        return true;
    }

    /// <inheritdoc/>
    public bool TryAdd(T2 key, T1 value)
    {
        var result = _dictionary2.TryAdd(key, value);
        if (result && _dictionary1.TryAdd(value, key) is false)
        {
            _dictionary2.Remove(key);
            return false;
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TryAdd(KeyValuePair<T2, T1> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool Remove(T2 key)
    {
        if (_dictionary2.Remove(key, out var value) is false)
            return false;

        _dictionary1.Remove(value);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T2, T1> item)
    {
        if (
            _dictionary2.TryGetValue(item.Key, out var value) is false
            || _dictionary1.Comparer.Equals(item.Value, value) is false
        )
            return false;

        _dictionary1.Remove(value);
        _dictionary2.Remove(item.Key);
        return true;
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T2, T1> item)
    {
        return _dictionary2.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T2 key)
    {
        return _dictionary2.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value)
    {
        return _dictionary2.TryGetValue(key, out value);
    }

    #endregion
}
