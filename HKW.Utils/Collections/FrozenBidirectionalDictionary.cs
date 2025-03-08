using System.Collections;
using System.Collections.Frozen;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils;

/// <summary>
/// 冻结双向字典
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class FrozenBidirectionalDictionary
{
    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    public static FrozenBidirectionalDictionary<T1, T2> Create<T1, T2>(
        IEnumerable<KeyValuePair<T1, T2>> keyValuePairs
    )
        where T1 : notnull
        where T2 : notnull
    {
        return new(keyValuePairs);
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    public static FrozenBidirectionalDictionary<T1, T2> Create<T1, T2>(
        IEnumerable<(T1, T2)> keyValuePairs
    )
        where T1 : notnull
        where T2 : notnull
    {
        return new(keyValuePairs);
    }
}

/// <summary>
/// 冻结双向字典
/// </summary>
public class FrozenBidirectionalDictionary<T1, T2> : IDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly FrozenDictionary<T1, T2> _dictionary1;
    private readonly FrozenDictionary<T2, T1> _dictionary2;

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    internal FrozenBidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>> keyValuePairs)
    {
        _dictionary1 = FrozenDictionary.ToFrozenDictionary(keyValuePairs);
        _dictionary2 = FrozenDictionary.ToFrozenDictionary(
            keyValuePairs.Select(x => KeyValuePair.Create(x.Value, x.Key))
        );
    }

    /// <inheritdoc/>
    /// <param name="keyValuePairs">键值对</param>
    internal FrozenBidirectionalDictionary(IEnumerable<(T1, T2)> keyValuePairs)
    {
        _dictionary1 = FrozenDictionary.ToFrozenDictionary(
            keyValuePairs.Select(x => KeyValuePair.Create(x.Item1, x.Item2))
        );
        _dictionary2 = FrozenDictionary.ToFrozenDictionary(
            keyValuePairs.Select(x => KeyValuePair.Create(x.Item2, x.Item1))
        );
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
        set => throw new ReadOnlyException();
    }

    void IDictionary<T1, T2>.Add(T1 key, T2 value)
    {
        throw new ReadOnlyException();
    }

    void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item)
    {
        throw new ReadOnlyException();
    }

    void ICollection<KeyValuePair<T1, T2>>.Clear()
    {
        throw new ReadOnlyException();
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

    bool IDictionary<T1, T2>.Remove(T1 key)
    {
        throw new ReadOnlyException();
    }

    bool ICollection<KeyValuePair<T1, T2>>.Remove(KeyValuePair<T1, T2> item)
    {
        throw new ReadOnlyException();
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
        set => throw new ReadOnlyException();
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
    public bool TryGetValue(T2 key, [MaybeNullWhen(false)] out T1 value)
    {
        return ((IDictionary<T2, T1>)_dictionary2).TryGetValue(key, out value);
    }

    #endregion
}
