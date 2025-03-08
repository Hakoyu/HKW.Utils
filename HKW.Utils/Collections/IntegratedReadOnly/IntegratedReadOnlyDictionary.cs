using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 集成只读字典的字典
/// <para>
/// 示例:
/// <code><![CDATA[
/// IntegratedReadOnlyDictionary<int, int, Dictionary<int, int>, ReadOnlyDictionary<int, int>> Dictionary { get; } = new(new (), l => new (l));
/// ReadOnlyDictionary<int, int> ReadOnlyDictionary => Dictionary.ReadOnlyDictionary;
/// ]]></code>
/// </para>
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
/// <typeparam name="TReadOnlyDictionary">只读字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class IntegratedReadOnlyDictionary<TKey, TValue, TDictionary, TReadOnlyDictionary>
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
    where TReadOnlyDictionary : IReadOnlyDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="readOnlyDictionary">只读字典</param>
    public IntegratedReadOnlyDictionary(
        TDictionary dictionary,
        TReadOnlyDictionary readOnlyDictionary
    )
    {
        Dictionary = dictionary;
        ReadOnlyDictionary = readOnlyDictionary;
    }

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="getReadOnlyDictionary">获取只读字典</param>
    public IntegratedReadOnlyDictionary(
        TDictionary dictionary,
        Func<TDictionary, TReadOnlyDictionary> getReadOnlyDictionary
    )
        : this(dictionary, getReadOnlyDictionary(dictionary)) { }

    /// <summary>
    /// 字典
    /// </summary>
    public TDictionary Dictionary { get; }

    /// <summary>
    /// 只读字典
    /// </summary>
    public TReadOnlyDictionary ReadOnlyDictionary { get; }

    #region IDictionary
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => Dictionary[key];
        set => Dictionary[key] = value;
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => Dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => Dictionary.Values;

    /// <inheritdoc/>
    public int Count => Dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => Dictionary.IsReadOnly;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ReadOnlyDictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ReadOnlyDictionary.Values;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        Dictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Dictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Dictionary.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return Dictionary.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return Dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Dictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        return Dictionary.Remove(key);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Dictionary.Remove(item);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return Dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Dictionary).GetEnumerator();
    }
    #endregion
}
