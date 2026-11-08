using System.Collections;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 冻结双向字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public sealed class FrozenBidirectionalDictionary<T1, T2>
    : IBidirectionalDictionary<T1, T2>,
        IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    /// <inheritdoc/>
    /// <param name="source">字典1</param>
    /// <param name="comparer1">T1比较器</param>
    /// <param name="comparer2">T2比较器</param>
    public FrozenBidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> source,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        var capacity = source.TryGetNonEnumeratedCount(out var count) ? count : 0;
        var forward = new Dictionary<T1, T2>(capacity, comparer1);
        var reverse = new Dictionary<T2, T1>(capacity, comparer2);
        foreach (var pair in source)
        {
            ArgumentNullException.ThrowIfNull(pair.Key);
            ArgumentNullException.ThrowIfNull(pair.Value);
            forward.Add(pair.Key, pair.Value);
            reverse.Add(pair.Value, pair.Key);
        }
        _dictionary1 = forward.ToFrozenDictionary(comparer1);
        _dictionary2 = reverse.ToFrozenDictionary(comparer2);
    }

    private readonly FrozenDictionary<T1, T2> _dictionary1;

    private readonly FrozenDictionary<T2, T1> _dictionary2;

    /// <inheritdoc/>
    public ICollection<T1> Keys => _dictionary1.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => _dictionary1.Values;

    /// <inheritdoc/>
    public int Count => _dictionary1.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <summary>
    /// 字典1
    /// </summary>
    public IReadOnlyDictionary<T1, T2> Dictionary1 =>
        field ??= new ReadOnlyDictionary<T1, T2>(_dictionary1);

    /// <summary>
    /// 字典2
    /// </summary>
    public IReadOnlyDictionary<T2, T1> Dictionary2 =>
        field ??= new ReadOnlyDictionary<T2, T1>(_dictionary2);

    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => Keys;

    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => Values;

    #region Dictionary1
    /// <inheritdoc/>
    public T2 this[T1 key1]
    {
        get => _dictionary1[key1];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IDictionary<T1, T2>.Add(T1 key1, T2 value2)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item1)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.TryAdd(T1 key1, T2 value2)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.TrySetValue(T1 key1, T2 value2)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.Remove(T1 key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IDictionary<T1, T2>.Remove(T1 key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.Remove(KeyValuePair<T1, T2> item1)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<T1, T2>>.Remove(KeyValuePair<T1, T2> item1)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<T1, T2>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T1, T2> item1)
    {
        return _dictionary1.TryGetValue(item1.Key, out var value2)
            && _dictionary2.Comparer.Equals(item1.Value, value2);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T1 key1)
    {
        return _dictionary1.ContainsKey(key1);
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
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.TryAdd(T2 key2, T1 value1)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.TrySetValue(T2 key2, T1 value1)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.Remove(T2 key2)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    bool IBidirectionalDictionary<T1, T2>.Remove(KeyValuePair<T2, T1> item2)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool ContainsKey(T2 key)
    {
        return _dictionary2.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<T2, T1> item2)
    {
        return _dictionary2.TryGetValue(item2.Key, out var value1)
            && _dictionary1.Comparer.Equals(item2.Value, value1);
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
    /// <inheritdoc/>
    public T2 GetForward(T1 key) => this[key];

    /// <inheritdoc/>
    public T1 GetReverse(T2 key) => this[key];

    /// <inheritdoc/>
    public bool ContainsKeyForward(T1 key) => ContainsKey(key);

    /// <inheritdoc/>
    public bool ContainsKeyReverse(T2 key) => ContainsKey(key);

    /// <inheritdoc/>
    public bool ContainsForward(KeyValuePair<T1, T2> item) => Contains(item);

    /// <inheritdoc/>
    public bool ContainsReverse(KeyValuePair<T2, T1> item) => Contains(item);

    /// <inheritdoc/>
    public bool TryGetForward(T1 key, [MaybeNullWhen(false)] out T2 value) =>
        TryGetValue(key, out value);

    /// <inheritdoc/>
    public bool TryGetReverse(T2 key, [MaybeNullWhen(false)] out T1 value) =>
        TryGetValue(key, out value);

    /// <inheritdoc/>
    public bool TryAddForward(T1 key, T2 value) =>
        ((IBidirectionalDictionary<T1, T2>)this).TryAdd(key, value);

    /// <inheritdoc/>
    public bool TryAddReverse(T2 key, T1 value) =>
        ((IBidirectionalDictionary<T1, T2>)this).TryAdd(key, value);

    /// <inheritdoc/>
    public bool TrySetForward(T1 key, T2 value) =>
        ((IBidirectionalDictionary<T1, T2>)this).TrySetValue(key, value);

    /// <inheritdoc/>
    public bool TrySetReverse(T2 key, T1 value) =>
        ((IBidirectionalDictionary<T1, T2>)this).TrySetValue(key, value);

    /// <inheritdoc/>
    public bool RemoveForward(T1 key) => ((IBidirectionalDictionary<T1, T2>)this).Remove(key);

    /// <inheritdoc/>
    public bool RemoveReverse(T2 key) => ((IBidirectionalDictionary<T1, T2>)this).Remove(key);

    /// <inheritdoc/>
    public bool RemoveForward(KeyValuePair<T1, T2> item) =>
        ((IBidirectionalDictionary<T1, T2>)this).Remove(item);

    /// <inheritdoc/>
    public bool RemoveReverse(KeyValuePair<T2, T1> item) =>
        ((IBidirectionalDictionary<T1, T2>)this).Remove(item);

    /// <inheritdoc/>
    public void CopyToForward(KeyValuePair<T1, T2>[] array, int index) => CopyTo(array, index);

    /// <inheritdoc/>
    public void CopyToReverse(KeyValuePair<T2, T1>[] array, int index) => CopyTo(array, index);
}
