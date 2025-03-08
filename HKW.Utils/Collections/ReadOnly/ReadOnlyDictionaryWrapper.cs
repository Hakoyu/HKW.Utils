using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 只读字典包装器
/// <para>用于 <see cref="HKWExtensions.AsReadOnlyOnWrapper{TKey, TValue, TReadOnlyValue}(IDictionary{TKey, TValue})"/></para>
/// </summary>
/// <typeparam name="TKey">键</typeparam>
/// <typeparam name="TValue">值</typeparam>
/// <typeparam name="TReadOnlyValue">只读值</typeparam>
[DebuggerDisplay("Count = {Count}")]
public class ReadOnlyDictionaryWrapper<TKey, TValue, TReadOnlyValue>
    : IDictionary<TKey, TReadOnlyValue>,
        IReadOnlyDictionary<TKey, TReadOnlyValue>
    where TKey : notnull
    where TValue : TReadOnlyValue
    where TReadOnlyValue : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    private readonly IDictionary<TKey, TValue> _dictionary;

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <exception cref="ArgumentNullException">iDictionary 为 null</exception>
    public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        _dictionary = dictionary;
    }

    #region IDictionary

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TReadOnlyValue> Values => _dictionary.Values.Cast<TReadOnlyValue>().ToList();

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TReadOnlyValue>.Keys => _dictionary.Keys;

    /// <inheritdoc/>
    IEnumerable<TReadOnlyValue> IReadOnlyDictionary<TKey, TReadOnlyValue>.Values =>
        _dictionary.Values.Cast<TReadOnlyValue>();

    /// <inheritdoc/>
    public TReadOnlyValue this[TKey key] => _dictionary[key];

    /// <inheritdoc/>
    TReadOnlyValue IDictionary<TKey, TReadOnlyValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator()
    {
        return _dictionary
            .Select(kv => new KeyValuePair<TKey, TReadOnlyValue>(kv.Key, kv.Value))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    void IDictionary<TKey, TReadOnlyValue>.Add(TKey key, TReadOnlyValue value)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TReadOnlyValue>.Remove(TKey key)
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Add(
        KeyValuePair<TKey, TReadOnlyValue> item
    )
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Clear()
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Remove(
        KeyValuePair<TKey, TReadOnlyValue> item
    )
    {
        throw new ReadOnlyException();
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TReadOnlyValue> item)
    {
        return _dictionary.ContainsKey(item.Key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TReadOnlyValue value)
    {
        var r = _dictionary.TryGetValue(key, out var v);
        value = v!;
        return r;
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TReadOnlyValue>[] array, int arrayIndex)
    {
        (
            (ICollection<KeyValuePair<TKey, TReadOnlyValue>>)
                _dictionary.ToDictionary(kv => kv.Key, kv => (TReadOnlyValue)kv.Value)
        ).CopyTo(array, arrayIndex);
    }

    #endregion IDictionary
}
