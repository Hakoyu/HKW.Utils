﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

//[Serializable]
/// <summary>
/// 可观测字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    private readonly Dictionary<TKey, TValue> r_dictionary;

    #region crot
    /// <inheritdoc/>
    public ObservableDictionary()
    {
        r_dictionary = new();
    }

    /// <inheritdoc/>
    /// <param name="dictionary">原始字典</param>
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        r_dictionary = new(dictionary);
    }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
        r_dictionary = new(comparer);
    }

    /// <inheritdoc/>
    /// <param name="dictionary">原始字典</param>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(
        IDictionary<TKey, TValue> dictionary,
        IEqualityComparer<TKey> comparer
    )
    {
        r_dictionary = new(dictionary, comparer);
    }
    #endregion
    #region IDictionary

    /// <inheritdoc/>
    public ICollection<TKey> Keys => r_dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => r_dictionary.Values;

    /// <inheritdoc/>
    public int Count => r_dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).IsReadOnly;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => r_dictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => r_dictionary.Values;

    #region Change
    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => r_dictionary[key];
        set
        {
            var oldValue = r_dictionary[key];
            if (
                oldValue.Equals(value)
                || OnDictionaryValueChanging(r_dictionary.GetEntry(key), value)
            )
                return;
            r_dictionary[key] = value;
            OnDictionaryValueChanged(r_dictionary.GetEntry(key), oldValue);
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        var entry = new KeyValuePair<TKey, TValue>(key, value);
        if (OnDictionaryAdding(entry))
            return;
        ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).Add(entry);
        OnDictionaryAdded(entry);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (OnDictionaryAdding(item))
            return;
        ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).Add(item);
        OnDictionaryAdded(item);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public bool TryAdd(TKey key, TValue value)
    {
        var entry = new KeyValuePair<TKey, TValue>(key, value);
        if (OnDictionaryAdding(entry))
            return false;
        var result = r_dictionary.TryAdd(entry.Key, entry.Value);
        if (result)
        {
            OnDictionaryAdded(entry);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public bool TryAdd(KeyValuePair<TKey, TValue> item)
    {
        if (OnDictionaryAdding(item))
            return false;
        var result = r_dictionary.TryAdd(item.Key, item.Value);
        if (result)
        {
            OnDictionaryAdded(item);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (r_dictionary.TryGetEntry(key, out var entry) is false)
            return false;
        if (OnDictionaryRemoving((KeyValuePair<TKey, TValue>)entry))
            return false;
        var result = r_dictionary.Remove(key);
        if (result)
        {
            OnDictionaryRemoved((KeyValuePair<TKey, TValue>)entry);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (OnDictionaryRemoving(item))
            return false;
        var result = ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).Remove(item);
        if (result)
        {
            OnDictionaryRemoved(item);
            OnCountPropertyChanged();
        }
        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var oldCount = Count;
        if (OnDictionaryClearing())
            return;
        r_dictionary.Clear();
        OnDictionaryCleared();
        if (oldCount != Count)
            OnCountPropertyChanged();
    }

    #endregion
    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return r_dictionary.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return r_dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return r_dictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return r_dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)r_dictionary).GetEnumerator();
    }
    #endregion
    #region DictionaryChanging
    /// <summary>
    /// 字典添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryAdding(KeyValuePair<TKey, TValue> item)
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(NotifyDictionaryChangeAction.Add, item));
        return false;
    }

    /// <summary>
    /// 字典删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryRemoving(KeyValuePair<TKey, TValue> item)
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(NotifyDictionaryChangeAction.Remove, item));
        return false;
    }

    /// <summary>
    /// 字典条目值改变时
    /// </summary>
    /// <param name="entry">旧条目</param>
    /// <param name="newValue">新值</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryValueChanging(KeyValuePair<TKey, TValue> entry, TValue newValue)
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(
                new(NotifyDictionaryChangeAction.ValueChange, entry, new(entry.Key, newValue))
            );
        return false;
    }

    /// <summary>
    /// 字典清理时
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryClearing()
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(NotifyDictionaryChangeAction.Clear));
        return false;
    }

    /// <summary>
    /// 字典改变时
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryChanging(
        NotifyDictionaryChangingEventArgs<TKey, TValue> args
    )
    {
        DictionaryChanging?.Invoke(this, args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event NotifyDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;
    #endregion
    #region DictionaryChanged
    /// <summary>
    /// 字典已添加条目时
    /// </summary>
    /// <param name="item">条目</param>
    private void OnDictionaryAdded(KeyValuePair<TKey, TValue> item)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(NotifyDictionaryChangeAction.Add, item));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
    }

    /// <summary>
    /// 字典已删除条目时
    /// </summary>
    /// <param name="item">条目</param>
    private void OnDictionaryRemoved(KeyValuePair<TKey, TValue> item)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(NotifyDictionaryChangeAction.Remove, item));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item));
    }

    /// <summary>
    /// 字典条目值已改变时
    /// </summary>
    /// <param name="entry">新条目</param>
    /// <param name="oldValue">旧值</param>
    private void OnDictionaryValueChanged(KeyValuePair<TKey, TValue> entry, TValue oldValue)
    {
        var oldEntry = new KeyValuePair<TKey, TValue>(entry.Key, oldValue);
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(NotifyDictionaryChangeAction.ValueChange, entry, oldEntry));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Replace,
                    entry,
                    new KeyValuePair<TKey, TValue>(entry.Key, oldValue)
                )
            );
    }

    /// <summary>
    /// 字典已清理时
    /// </summary>
    private void OnDictionaryCleared()
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(NotifyDictionaryChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// 字典已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangedEventArgs<TKey, TValue> args)
    {
        DictionaryChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    #endregion

    #region PropertyChanged

    /// <summary>
    /// 数量已改变时
    /// </summary>
    private void OnCountPropertyChanged()
    {
        if (PropertyChanged is not null)
            OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// 属性已改变时
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region

    /// <summary>
    /// 列表已改变时
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    #endregion
}
