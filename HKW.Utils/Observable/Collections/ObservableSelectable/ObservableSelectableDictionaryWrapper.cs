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
public partial class ObservableSelectableDictionaryWrapper<TKey, TValue, TDictionary>
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
        BaseDictionary = dictionary;
    }

    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="seletedKey">选中的键</param>
    public ObservableSelectableDictionaryWrapper(TDictionary dictionary, TKey seletedKey)
        : this(dictionary)
    {
        SelectedItem = BaseDictionary.GetPair(seletedKey);
    }

    /// <inheritdoc/>
    public TDictionary BaseDictionary { get; }

    /// <summary>
    /// 选中的项目
    /// </summary>
    [ReactiveProperty]
    public KeyValuePair<TKey, TValue> SelectedItem { get; set; }

    #region IDictionary
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => BaseDictionary[key];
        set
        {
            BaseDictionary[key] = value;
            SelectedItem = BaseDictionary.GetPair(key);
        }
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => BaseDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => BaseDictionary.Values;

    /// <inheritdoc/>
    public int Count => BaseDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => BaseDictionary.IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        BaseDictionary.Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        BaseDictionary.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        BaseDictionary.Clear();
        SelectedItem = default;
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return BaseDictionary.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return BaseDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        BaseDictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var r = BaseDictionary.Remove(key);
        if (r && key.Equals(SelectedItem.Key))
            SelectedItem = default;
        return r;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var r = BaseDictionary.Remove(item);
        if (r && item.Key.Equals(SelectedItem.Key))
            SelectedItem = default;
        return r;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return BaseDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return BaseDictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseDictionary).GetEnumerator();
    }
    #endregion
}
