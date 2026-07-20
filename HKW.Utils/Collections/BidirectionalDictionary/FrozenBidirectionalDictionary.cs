using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 冻结双向字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class FrozenBidirectionalDictionary<T1, T2>
    : IDictionary<T1, T2>,
        IReadOnlyDictionary<T1, T2>,
        IDictionary
    where T1 : notnull
    where T2 : notnull
{
    private readonly FrozenDictionary<T1, T2> _dictionary1;
    private readonly FrozenDictionary<T2, T1> _dictionary2;

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    internal FrozenBidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        if (keyValuePairs is not ICollection<KeyValuePair<T1, T2>> collection)
            collection = keyValuePairs.ToArray();

        _dictionary1 = FrozenDictionary.ToFrozenDictionary(collection, comparer1);
        _dictionary2 = FrozenDictionary.ToFrozenDictionary(
            collection,
            kv => kv.Value,
            kv => kv.Key,
            comparer2
        );
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    /// <param name="comparer1">比较器1</param>
    /// <param name="comparer2">比较器2</param>
    internal FrozenBidirectionalDictionary(
        IEnumerable<(T1, T2)> keyValuePairs,
        IEqualityComparer<T1>? comparer1 = null,
        IEqualityComparer<T2>? comparer2 = null
    )
    {
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        if (keyValuePairs is not ICollection<(T1, T2)> collection)
            collection = keyValuePairs.ToArray();

        _dictionary1 = FrozenDictionary.ToFrozenDictionary(
            collection,
            x => x.Item1,
            x => x.Item2,
            comparer1
        );
        _dictionary2 = FrozenDictionary.ToFrozenDictionary(
            collection,
            x => x.Item2,
            x => x.Item1,
            comparer2
        );
    }

    /// <inheritdoc/>
    public ICollection<T1> Keys => _dictionary1.Keys;

    /// <inheritdoc/>
    public ICollection<T2> Values => _dictionary1.Values;

    /// <inheritdoc/>
    public int Count => _dictionary1.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => Keys;

    IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => Values;

    bool IDictionary.IsFixedSize => ((IDictionary)_dictionary1).IsFixedSize;

    ICollection IDictionary.Keys => ((IDictionary)_dictionary1).Keys;

    ICollection IDictionary.Values => ((IDictionary)_dictionary1).Values;

    bool ICollection.IsSynchronized => ((ICollection)_dictionary1).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection)_dictionary1).SyncRoot;

    object? IDictionary.this[object key]
    {
        get => ((IDictionary)_dictionary1)[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    #region Dictionary1
    /// <inheritdoc/>
    public T2 this[T1 key]
    {
        get => _dictionary1[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void IDictionary<T1, T2>.Add(T1 key, T2 value)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    void ICollection<KeyValuePair<T1, T2>>.Clear()
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
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
        _dictionary1.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return _dictionary1.GetEnumerator();
    }

    bool IDictionary<T1, T2>.Remove(T1 key)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    bool ICollection<KeyValuePair<T1, T2>>.Remove(KeyValuePair<T1, T2> item)
    {
        throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
    {
        return _dictionary1.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary1.GetEnumerator();
    }

    void IDictionary.Add(object key, object? value)
    {
        ((IDictionary)_dictionary1).Add(key, value);
    }

    void IDictionary.Clear()
    {
        ((IDictionary)_dictionary1).Clear();
    }

    bool IDictionary.Contains(object key)
    {
        return ((IDictionary)_dictionary1).Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)_dictionary1).GetEnumerator();
    }

    void IDictionary.Remove(object key)
    {
        ((IDictionary)_dictionary1).Remove(key);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)_dictionary1).CopyTo(array, index);
    }
    #endregion

    #region Dictionary2
    /// <inheritdoc/>
    public T1 this[T2 key]
    {
        get => _dictionary2[key];
        set => throw new NotSupportedException(ExceptionMessage.IsReadOnlyCollection);
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
        _dictionary2.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value)
    {
        return _dictionary2.TryGetValue(key, out value);
    }

    #endregion
}
