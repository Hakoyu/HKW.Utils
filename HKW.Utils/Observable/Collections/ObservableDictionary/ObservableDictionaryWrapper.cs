using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView))]
public class ObservableDictionaryWrapper<TKey, TValue, TDictionary>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>,
        IDictionaryWrapper<TKey, TValue, TDictionary>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    public ObservableDictionaryWrapper(TDictionary dictionary)
    {
        BaseDictionary = dictionary;
        _observableKeys.BaseList.AddRange(BaseDictionary.Keys);
        _observableValues.BaseList.AddRange(BaseDictionary.Values);
        ObservableKeys = new ReadOnlyObservableList<TKey>(_observableKeys);
        ObservableValues = new ReadOnlyObservableList<TValue>(_observableValues);
    }

    /// <inheritdoc/>
    public TDictionary BaseDictionary { get; }

    #region IDictionaryT

    /// <inheritdoc/>
    public int Count => BaseDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)BaseDictionary).IsReadOnly;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => BaseDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => BaseDictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ObservableList<TKey> _observableKeys = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ObservableList<TValue> _observableValues = [];

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TKey> ObservableKeys { get; }

    /// <inheritdoc/>
    public IReadOnlyObservableCollection<TValue> ObservableValues { get; }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => BaseDictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => BaseDictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _removeIndex = -1;

    #region Change

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => BaseDictionary[key];
        set
        {
            if (BaseDictionary.TryGetValue(key, out var oldValue) is false)
            {
                var pair = KeyValuePair.Create(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                OnDictionaryAdding(pair);
                BaseDictionary[key] = value;
                OnDictionaryAdded(pair);
            }
            else
            {
                if (oldValue?.Equals(value) is true)
                    return;
                var newPair = KeyValuePair.Create(key, value);
                var oldPair = KeyValuePair.Create(key, oldValue);
                OnDictionaryReplacing(newPair, oldPair);
                BaseDictionary[key] = value;
                OnDictionaryReplaced(newPair, oldPair);
            }
        }
    }

    /// <inheritdoc/>
    public TValue this[TKey key, bool skipCheck]
    {
        get => BaseDictionary[key];
        set
        {
            if (BaseDictionary.TryGetValue(key, out var oldValue) is false)
            {
                var pair = KeyValuePair.Create(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                OnDictionaryAdding(pair);
                BaseDictionary[key] = value;
                OnDictionaryAdded(pair);
            }
            else
            {
                if (skipCheck is false && oldValue?.Equals(value) is true)
                    return;
                var newPair = KeyValuePair.Create(key, value);
                var oldPair = KeyValuePair.Create(key, oldValue);
                OnDictionaryReplacing(newPair, oldPair);
                BaseDictionary[key] = value;
                OnDictionaryReplaced(newPair, oldPair);
            }
        }
    }

    /// <summary>
    /// 字典改变事件参数
    /// </summary>
    protected NotifyDictionaryChangeEventArgs<TKey, TValue>? DictionaryChangeEventArgs { get; set; }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        if (BaseDictionary.ContainsKey(key))
            BaseDictionary.Add(key, value);

        var pair = KeyValuePair.Create(key, value);
        OnDictionaryAdding(pair);
        BaseDictionary.Add(key, value);
        OnDictionaryAdded(pair);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (BaseDictionary.TryGetPair(key, out var pair) is false)
            return false;
        OnDictionaryRemoving(pair);
        var result = BaseDictionary.Remove(key);
        if (result)
            OnDictionaryRemoved(pair);
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnDictionaryClearing();
        BaseDictionary.Clear();
        OnDictionaryCleared();
    }

    #endregion

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return BaseDictionary.ContainsKey(item.Key);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return BaseDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)BaseDictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return BaseDictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return BaseDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)BaseDictionary).GetEnumerator();
    }

    #endregion IDictionaryT

    #region DictionaryChanging

    /// <summary>
    /// 字典添加键值对前
    /// </summary>
    /// <param name="pair">键值对</param>
    protected virtual void OnDictionaryAdding(KeyValuePair<TKey, TValue> pair)
    {
        if (DictionaryChanging is not null)
            OnDictionaryChanging(new(DictionaryChangeAction.Add, pair));
    }

    /// <summary>
    /// 字典删除键值对前
    /// </summary>
    /// <param name="pair">键值对</param>
    protected virtual void OnDictionaryRemoving(KeyValuePair<TKey, TValue> pair)
    {
        if (DictionaryChanging is not null)
            OnDictionaryChanging(new(DictionaryChangeAction.Remove, pair));
        if (CollectionChanged is not null)
            _removeIndex = BaseDictionary.IndexOf(pair);
    }

    /// <summary>
    /// 字典改变键值对值前
    /// </summary>
    /// <param name="newPair">新键值对</param>
    /// <param name="oldPair">旧键值对</param>
    protected virtual void OnDictionaryReplacing(
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (DictionaryChanging is not null)
            OnDictionaryChanging(new(DictionaryChangeAction.Replace, newPair, oldPair));
    }

    /// <summary>
    /// 字典清理前
    /// </summary>
    protected virtual void OnDictionaryClearing()
    {
        if (DictionaryChanging is not null)
            OnDictionaryChanging(new(DictionaryChangeAction.Clear));
    }

    /// <summary>
    /// 字典改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual void OnDictionaryChanging(NotifyDictionaryChangeEventArgs<TKey, TValue> args)
    {
        DictionaryChanging?.Invoke(this, DictionaryChangeEventArgs = args);
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    #endregion DictionaryChanging

    #region DictionaryChanged

    /// <summary>
    /// 字典添加键值对后
    /// </summary>
    /// <param name="pair">键值对</param>
    protected virtual void OnDictionaryAdded(KeyValuePair<TKey, TValue> pair)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(DictionaryChangeEventArgs ?? new(DictionaryChangeAction.Add, pair));
        if (CollectionChanged is not null)
        {
            var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(pair);
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, list, Count - 1));
        }
        _observableKeys.Add(pair.Key);
        _observableValues.Add(pair.Value);
        OnCountChanged();
    }

    /// <summary>
    /// 字典删除键值对后
    /// </summary>
    /// <param name="pair">键值对</param>
    protected virtual void OnDictionaryRemoved(KeyValuePair<TKey, TValue> pair)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(
                DictionaryChangeEventArgs ?? new(DictionaryChangeAction.Remove, pair)
            );
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, pair, _removeIndex));
        _observableKeys.Remove(pair.Key);
        _observableValues.Remove(pair.Value);
        OnCountChanged();
    }

    /// <summary>
    /// 字典键值对值改变后
    /// </summary>
    /// <param name="newPair">新键值对</param>
    /// <param name="oldPair">旧键值对</param>
    protected virtual void OnDictionaryReplaced(
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(
                DictionaryChangeEventArgs ?? new(DictionaryChangeAction.Replace, newPair, oldPair)
            );
        if (CollectionChanged is not null)
        {
            var index = BaseDictionary.IndexOf((p => p.Key.Equals(oldPair.Key)));
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newPair, oldPair, index)
            );
        }
        _observableValues[_observableKeys.IndexOf(oldPair.Key)] = newPair.Value;
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    protected virtual void OnDictionaryCleared()
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(DictionaryChangeEventArgs ?? new(DictionaryChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        OnCountChanged();
        _observableKeys.Clear();
        _observableValues.Clear();
    }

    /// <summary>
    /// 字典改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangeEventArgs<TKey, TValue> args)
    {
        DictionaryChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    #endregion DictionaryChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged

    #region PropertyChanged

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountChanged()
    {
        OnPropertyChanged(nameof(Count));
        DictionaryChangeEventArgs = null;
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged
}
