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
    : IDictionary<T1, T2>,
        IReadOnlyDictionary<T1, T2>
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

    /// <summary>
    /// 字典1
    /// </summary>
    public ReadOnlyDictionary<T1, T2> Dictionary1 =>
        field ??= new ReadOnlyDictionary<T1, T2>(_dictionary1);

    /// <summary>
    /// 字典2
    /// </summary>
    public ReadOnlyDictionary<T2, T1> Dictionary2 =>
        field ??= new ReadOnlyDictionary<T2, T1>(_dictionary2);

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
    /// <remarks>
    /// <para>
    /// 当 key 和 value 都不存在时, 添加新值, 返回 true.
    /// </para>
    /// <para>
    /// 当 key 存在 value 不存在时, 替换 dic2 的 value, 删除 dic1 的 value 再添加新的 (value, key), 返回 true.
    /// </para>
    /// <para>
    /// 当 key 不存在 value 存在时, 返回 false. 基于仅依据 key 替换 value 的原则不予替换, 可以使用 TrySetValue(T1,T2)
    /// </para>
    /// <para>
    /// 当 key 和 value 都存在时, 返回 true.
    /// </para>
    /// </remarks>
    public bool TrySetValue(T1 key, T2 value)
    {
        if (_dictionary1.TryGetValue(key, out var d1Value))
        {
            // 4, 如果 d1Value 和 value 相等, 证明 dic2 存在 (value, key)
            if (_comparer2.Equals(d1Value, value))
                return true;
            // 3
            if (_dictionary2.ContainsKey(value))
                return false;
            // 2
            _dictionary1[key] = value;
            _dictionary2.Remove(d1Value);
            _dictionary2.Add(value, key);
            return true;
        }
        else
        {
            // 3
            if (_dictionary2.ContainsKey(value))
                return false;
            // 1
            _dictionary1.Add(key, value);
            _dictionary2.Add(value, key);
            return true;
        }
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
            || _comparer2.Equals(item.Value, value) is false
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

    /// <summary>
    /// 尝试设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否设置成功</returns>
    /// <remarks>
    /// <para>
    /// 当 key 和 value 都不存在时, 添加新值, 返回 true.
    /// </para>
    /// <para>
    /// 当 key 存在 value 不存在时, 替换 dic2 的 value, 删除 dic1 的 value 再添加新的 (value, key), 返回 true.
    /// </para>
    /// <para>
    /// 当 key 不存在 value 存在时, 返回 false. 基于仅依据 key 替换 value 的原则不予替换, 可以使用 TrySetValue(T1,T2)
    /// </para>
    /// <para>
    /// 当 key 和 value 都存在时, 返回 true.
    /// </para>
    /// </remarks>
    public bool TrySetValue(T2 key, T1 value)
    {
        if (_dictionary2.TryGetValue(key, out var d2Value))
        {
            // 4, 如果 d2Value 和 value 相等, 证明 dic1 存在 (value, key)
            if (_comparer1.Equals(d2Value, value))
                return true;
            // 3
            if (_dictionary1.ContainsKey(value))
                return false;
            // 2
            _dictionary2[key] = value;
            _dictionary1.Remove(d2Value);
            _dictionary1.Add(value, key);
            return true;
        }
        else
        {
            // 3
            if (_dictionary1.ContainsKey(value))
                return false;
            // 1
            _dictionary2.Add(key, value);
            _dictionary1.Add(value, key);
            return true;
        }
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
            || _comparer1.Equals(item.Value, value) is false
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
