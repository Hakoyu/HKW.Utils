using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测可选中字典包装器
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
#pragma warning disable S2436
public partial class ObservableSelectableDictionaryWrapper<TKey, TValue, TDictionary>
#pragma warning restore S2436
    : ReactiveObjectX,
        IDictionary<TKey, TValue>,
        IDictionaryWrapper<TKey, TValue, TDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    public ObservableSelectableDictionaryWrapper(TDictionary dictionary)
    {
        SourceDictionary = dictionary;
    }

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="seletedKey">选中的键</param>
    public ObservableSelectableDictionaryWrapper(TDictionary dictionary, TKey seletedKey)
        : this(dictionary)
    {
        SelectedItem = SourceDictionary.GetPair(seletedKey);
    }

    /// <inheritdoc/>
    public TDictionary SourceDictionary { get; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public KeyValuePair<TKey, TValue> SelectedItem { get; set; }

    #region IDictionary
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set
        {
            SourceDictionary[key] = value;
            SelectedItem = SourceDictionary.GetPair(key);
        }
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => SourceDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => SourceDictionary.Values;

    /// <inheritdoc/>
    public int Count => SourceDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceDictionary.IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        SourceDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        SourceDictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        SourceDictionary.Clear();
        SelectedItem = default;
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return SourceDictionary.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return SourceDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        SourceDictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var r = SourceDictionary.Remove(key);
        if (r && key.Equals(SelectedItem.Key))
            SelectedItem = default;
        return r;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var r = SourceDictionary.Remove(item);
        if (r && item.Key.Equals(SelectedItem.Key))
            SelectedItem = default;
        return r;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return SourceDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return SourceDictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceDictionary).GetEnumerator();
    }
    #endregion
}
