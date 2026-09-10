using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 双向字典包装器
/// </summary>
/// <typeparam name="T1">项目类型1</typeparam>
/// <typeparam name="T2">项目类型2</typeparam>
/// <typeparam name="TDictionary1">字典1</typeparam>
/// <typeparam name="TDictionary2">字典2</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class BidirectionalDictionaryWrapper<T1, T2, TDictionary1, TDictionary2>
#pragma warning restore S2436
    : IBidirectionalDictionary<T1, T2>,
        IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
    where TDictionary1 : IDictionary<T1, T2>
    where TDictionary2 : IDictionary<T2, T1>
{
    /// <summary>
    /// 字典1
    /// </summary>
    protected readonly TDictionary1 _dictionary1;

    /// <summary>
    /// 字典2
    /// </summary>
    protected readonly TDictionary2 _dictionary2;

    /// <summary>
    /// 比较器1
    /// </summary>
    protected readonly IEqualityComparer<T1> _comparer1;

    /// <summary>
    /// 比较器2
    /// </summary>
    protected readonly IEqualityComparer<T2> _comparer2;

    /// <inheritdoc/>
    /// <param name="dictionary1">字典1</param>
    /// <param name="dictionary2">字典2</param>
    /// <param name="dictionary1Comparer">字典1比较器</param>
    /// <param name="dictionary2Comparer">字典2比较器</param>
    public BidirectionalDictionaryWrapper(
        TDictionary1 dictionary1,
        TDictionary2 dictionary2,
        IEqualityComparer<T1>? dictionary1Comparer,
        IEqualityComparer<T2>? dictionary2Comparer
    )
    {
        ArgumentNullException.ThrowIfNull(dictionary1);
        ArgumentNullException.ThrowIfNull(dictionary2);
        _dictionary1 = dictionary1;
        _dictionary2 = dictionary2;
        _comparer1 = dictionary1Comparer ?? EqualityComparer<T1>.Default;
        _comparer2 = dictionary2Comparer ?? EqualityComparer<T2>.Default;
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => _dictionary1.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => _dictionary1.Values;

    /// <inheritdoc/>
    public int Count => _dictionary1.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => _dictionary1.IsReadOnly;

    /// <inheritdoc/>
    public IDictionary<T1, T2> Dictionary1 =>
        field ??= new ReadOnlyDictionary<T1, T2>(_dictionary1);

    /// <inheritdoc/>
    public IDictionary<T2, T1> Dictionary2 =>
        field ??= new ReadOnlyDictionary<T2, T1>(_dictionary2);

    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => Keys;

    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => Values;

    #region Dictionary1
    /// <inheritdoc/>
    public T2 this[T1 key1]
    {
        get => _dictionary1[key1];
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

    /// <inheritdoc/>
    public bool TryAdd(T1 key1, T2 value2)
    {
        var result = _dictionary1.TryAdd(key1, value2);
        if (result && _dictionary2.TryAdd(value2, key1) is false)
        {
            _dictionary1.Remove(key1);
            return false;
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TrySetValue(T1 key1, T2 value2)
    {
        if (_dictionary1.TryGetValue(key1, out var d1Value))
        {
            // 4, 如果 d1Value 和 value 相等, 证明 dic2 存在 (value, key)
            if (_comparer2.Equals(d1Value, value2))
                return true;
            // 3
            if (_dictionary2.ContainsKey(value2))
                return false;
            // 2
            _dictionary1[key1] = value2;
            _dictionary2.Remove(d1Value);
            _dictionary2.Add(value2, key1);
            return true;
        }
        else
        {
            // 3
            if (_dictionary2.ContainsKey(value2))
                return false;
            // 1
            _dictionary1.Add(key1, value2);
            _dictionary2.Add(value2, key1);
            return true;
        }
    }

    /// <inheritdoc/>
    public bool Remove(T1 key1)
    {
        if (_dictionary1.Remove(key1, out var value) is false)
            return false;

        _dictionary2.Remove(value);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T1, T2> item1)
    {
        if (
            _dictionary1.TryGetValue(item1.Key, out var value) is false
            || _comparer2.Equals(item1.Value, value) is false
        )
            return false;

        _dictionary1.Remove(item1.Key);
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
    public bool ContainsKey(T1 key1)
    {
        return _dictionary1.ContainsKey(key1);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item1)
    {
        return _dictionary1.TryGetValue(item1.Key, out var value2)
            && _comparer2.Equals(item1.Value, value2);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T1, T2>[] array1, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T1, T2>>)_dictionary1).CopyTo(array1, arrayIndex);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T1 key1, [MaybeNullWhen(false)] out T2 value)
    {
        return _dictionary1.TryGetValue(key1, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return _dictionary1.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    #region Dictionary2
    /// <inheritdoc/>
    public T1 this[T2 key2]
    {
        get => _dictionary2[key2];
        set => throw new UseAlternativeMethodException(nameof(this.TrySetValue));
    }

    /// <inheritdoc/>
    public bool TryAdd(T2 key2, T1 value1)
    {
        var result = _dictionary2.TryAdd(key2, value1);
        if (result && _dictionary1.TryAdd(value1, key2) is false)
        {
            _dictionary2.Remove(key2);
            return false;
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TrySetValue(T2 key2, T1 value1)
    {
        if (_dictionary2.TryGetValue(key2, out var d2Value))
        {
            // 4, 如果 d2Value 和 value 相等, 证明 dic1 存在 (value, key)
            if (_comparer1.Equals(d2Value, value1))
                return true;
            // 3
            if (_dictionary1.ContainsKey(value1))
                return false;
            // 2
            _dictionary2[key2] = value1;
            _dictionary1.Remove(d2Value);
            _dictionary1.Add(value1, key2);
            return true;
        }
        else
        {
            // 3
            if (_dictionary1.ContainsKey(value1))
                return false;
            // 1
            _dictionary2.Add(key2, value1);
            _dictionary1.Add(value1, key2);
            return true;
        }
    }

    /// <inheritdoc/>
    public bool Remove(T2 key2)
    {
        if (_dictionary2.Remove(key2, out var value) is false)
            return false;

        _dictionary1.Remove(value);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T2, T1> item2)
    {
        if (
            _dictionary2.TryGetValue(item2.Key, out var value) is false
            || _comparer1.Equals(item2.Value, value) is false
        )
            return false;

        _dictionary1.Remove(value);
        _dictionary2.Remove(item2.Key);
        return true;
    }

    /// <inheritdoc/>
    public bool ContainsKey(T2 key2)
    {
        return _dictionary2.ContainsKey(key2);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T2, T1> item2)
    {
        return _dictionary2.TryGetValue(item2.Key, out var value1)
            && _comparer1.Equals(item2.Value, value1);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<T2, T1>[] array2, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T2, T1>>)_dictionary2).CopyTo(array2, arrayIndex);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key2, [MaybeNullWhen(false)] out T1 value)
    {
        return _dictionary2.TryGetValue(key2, out value);
    }
    #endregion
}
