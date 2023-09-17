using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
    private readonly IDictionary<TKey, TValue> _iDictionary;

    /// <inheritdoc/>
    /// <param name="iDictionary">字典</param>
    /// <exception cref="ArgumentNullException">iDictionary 为 null</exception>
    public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> iDictionary)
    {
        ArgumentNullException.ThrowIfNull(iDictionary);
        _iDictionary = iDictionary;
    }

    #region IDictionary

    /// <inheritdoc/>
    public int Count => _iDictionary.Count;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _iDictionary.Keys;

    /// <inheritdoc/>
    /// <summary>此属性有性能问题, 少用</summary>
    public ICollection<TReadOnlyValue> Values =>
        _iDictionary.Values.Cast<TReadOnlyValue>().ToList();

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TReadOnlyValue>.Keys => _iDictionary.Keys;

    /// <inheritdoc/>
    IEnumerable<TReadOnlyValue> IReadOnlyDictionary<TKey, TReadOnlyValue>.Values =>
        _iDictionary.Values.Cast<TReadOnlyValue>();

    /// <inheritdoc/>
    public TReadOnlyValue this[TKey key] => _iDictionary[key];

    /// <inheritdoc/>
    TReadOnlyValue IDictionary<TKey, TReadOnlyValue>.this[TKey key]
    {
        get => _iDictionary[key];
        set => throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator() =>
        _iDictionary
            .Select(kv => new KeyValuePair<TKey, TReadOnlyValue>(kv.Key, kv.Value))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    void IDictionary<TKey, TReadOnlyValue>.Add(TKey key, TReadOnlyValue value) =>
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);

    /// <inheritdoc/>
    bool IDictionary<TKey, TReadOnlyValue>.Remove(TKey key) =>
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Add(
        KeyValuePair<TKey, TReadOnlyValue> item
    ) => throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Clear() =>
        throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TReadOnlyValue>>.Remove(
        KeyValuePair<TKey, TReadOnlyValue> item
    ) => throw new NotSupportedException(ExceptionMessage.ReadOnlyCollection);

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) => _iDictionary.ContainsKey(key);

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TReadOnlyValue> item) =>
        _iDictionary.ContainsKey(item.Key);

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TReadOnlyValue value)
    {
        var r = _iDictionary.TryGetValue(key, out var v);
        value = v!;
        return r;
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TReadOnlyValue>[] array, int arrayIndex) =>
        (
            (ICollection<KeyValuePair<TKey, TReadOnlyValue>>)
                _iDictionary.ToDictionary(kv => kv.Key, kv => (TReadOnlyValue)kv.Value)
        ).CopyTo(array, arrayIndex);

    #endregion IDictionary
}
