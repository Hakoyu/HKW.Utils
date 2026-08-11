using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选择集合
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public partial class ObservableSelectableDictionary<TKey, TValue>
    : ObservableSelectableDictionaryWrapper<TKey, TValue, ObservableDictionary<TKey, TValue>>,
        IDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    public ObservableSelectableDictionary()
        : base(new()) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableSelectableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : base(new(collection)) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableDictionary(IEqualityComparer<TKey> comparer)
        : base(new(comparer)) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableSelectableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
        : base(new(collection, comparer)) { }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => SourceDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => SourceDictionary.Values;

    /// <inheritdoc/>
    public int Count => SourceDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceDictionary.IsReadOnly;

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set => SourceDictionary[key] = value;
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        SourceDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return SourceDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        return SourceDictionary.Remove(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return SourceDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)SourceDictionary).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceDictionary.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)SourceDictionary).Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        SourceDictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)SourceDictionary).Remove(item);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return SourceDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceDictionary).GetEnumerator();
    }
}
