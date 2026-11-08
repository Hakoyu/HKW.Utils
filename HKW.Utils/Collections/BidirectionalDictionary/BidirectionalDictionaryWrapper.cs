using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        if (ReferenceEquals(dictionary1, dictionary2))
            throw new ArgumentException("The backing dictionaries must not alias each other.");
        BidirectionalDictionaryOperations.ValidateWritable(dictionary1, dictionary2);
        _comparer1 = BidirectionalDictionaryOperations.GetComparer(
            dictionary1,
            dictionary1Comparer
        );
        _comparer2 = BidirectionalDictionaryOperations.GetComparer(
            dictionary2,
            dictionary2Comparer
        );
        BidirectionalDictionaryOperations.ValidateInverse(
            dictionary1,
            dictionary2,
            _comparer1,
            _comparer2
        );
        _dictionary1 = dictionary1;
        _dictionary2 = dictionary2;
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => _dictionary1.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => _dictionary1.Values;

    /// <inheritdoc/>
    public int Count => _dictionary1.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => _dictionary1.IsReadOnly || _dictionary2.IsReadOnly;

    /// <inheritdoc/>
    public IReadOnlyDictionary<T1, T2> Dictionary1 =>
        field ??= new ReadOnlyDictionary<T1, T2>(_dictionary1);

    /// <inheritdoc/>
    public IReadOnlyDictionary<T2, T1> Dictionary2 =>
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
        return BidirectionalDictionaryOperations.TryAdd(_dictionary1, _dictionary2, key1, value2);
    }

    /// <inheritdoc/>
    public bool TrySetValue(T1 key1, T2 value2)
    {
        // 保留原分支编号与语义, 具体判定和补偿操作由共享实现执行.
        // 4, 如果 d1Value 和 value 相等, 证明 dic2 存在 (value, key)
        // 3
        // 2
        // 3
        // 1
        return BidirectionalDictionaryOperations.TrySet(
            _dictionary1,
            _dictionary2,
            _comparer2,
            key1,
            value2
        );
    }

    /// <inheritdoc/>
    public bool Remove(T1 key1)
    {
        return BidirectionalDictionaryOperations.Remove(_dictionary1, _dictionary2, key1);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T1, T2> item1)
    {
        return BidirectionalDictionaryOperations.Remove(
            _dictionary1,
            _dictionary2,
            _comparer2,
            item1
        );
    }

    /// <inheritdoc/>
    public void Clear()
    {
        BidirectionalDictionaryOperations.Clear(_dictionary1, _dictionary2);
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
        return BidirectionalDictionaryOperations.TryAdd(_dictionary2, _dictionary1, key2, value1);
    }

    /// <inheritdoc/>
    public bool TrySetValue(T2 key2, T1 value1)
    {
        // 保留原分支编号与语义, 具体判定和补偿操作由共享实现执行.
        // 4, 如果 d2Value 和 value 相等, 证明 dic1 存在 (value, key)
        // 3
        // 2
        // 3
        // 1
        return BidirectionalDictionaryOperations.TrySet(
            _dictionary2,
            _dictionary1,
            _comparer1,
            key2,
            value1
        );
    }

    /// <inheritdoc/>
    public bool Remove(T2 key2)
    {
        return BidirectionalDictionaryOperations.Remove(_dictionary2, _dictionary1, key2);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<T2, T1> item2)
    {
        return BidirectionalDictionaryOperations.Remove(
            _dictionary2,
            _dictionary1,
            _comparer1,
            item2
        );
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
    /// <summary>
    /// 获取正向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public T2 GetForward(T1 key) => this[key];

    /// <summary>
    /// 获取反向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public T1 GetReverse(T2 key) => this[key];

    /// <summary>
    /// 检查正向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsKeyForward(T1 key) => ContainsKey(key);

    /// <summary>
    /// 检查反向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsKeyReverse(T2 key) => ContainsKey(key);

    /// <summary>
    /// 检查正向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsForward(KeyValuePair<T1, T2> item) => Contains(item);

    /// <summary>
    /// 检查反向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool ContainsReverse(KeyValuePair<T2, T1> item) => Contains(item);

    /// <summary>
    /// 尝试获取正向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryGetForward(T1 key, [MaybeNullWhen(false)] out T2 value) =>
        TryGetValue(key, out value);

    /// <summary>
    /// 尝试获取反向映射值; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryGetReverse(T2 key, [MaybeNullWhen(false)] out T1 value) =>
        TryGetValue(key, out value);

    /// <summary>
    /// 尝试添加正向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryAddForward(T1 key, T2 value) => TryAdd(key, value);

    /// <summary>
    /// 尝试添加反向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TryAddReverse(T2 key, T1 value) => TryAdd(key, value);

    /// <summary>
    /// 尝试设置正向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TrySetForward(T1 key, T2 value) => TrySetValue(key, value);

    /// <summary>
    /// 尝试设置反向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool TrySetReverse(T2 key, T1 value) => TrySetValue(key, value);

    /// <summary>
    /// 删除正向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool RemoveForward(T1 key) => Remove(key);

    /// <summary>
    /// 删除反向键; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool RemoveReverse(T2 key) => Remove(key);

    /// <summary>
    /// 删除正向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool RemoveForward(KeyValuePair<T1, T2> item) => Remove(item);

    /// <summary>
    /// 删除反向键值对; 类型相同时使用此明确方向入口.
    /// </summary>
    public bool RemoveReverse(KeyValuePair<T2, T1> item) => Remove(item);

    /// <summary>
    /// 复制正向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public void CopyToForward(KeyValuePair<T1, T2>[] array, int index) => CopyTo(array, index);

    /// <summary>
    /// 复制反向映射; 类型相同时使用此明确方向入口.
    /// </summary>
    public void CopyToReverse(KeyValuePair<T2, T1>[] array, int index) => CopyTo(array, index);
}
