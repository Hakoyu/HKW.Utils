using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class ObservableDictionaryWrapper<TKey, TValue, TDictionary>
#pragma warning restore S2436
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="comparer">比较器, 必须与 <paramref name="dictionary"/> 的比较器相同</param>
    public ObservableDictionaryWrapper(TDictionary dictionary, IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        SourceDictionary = dictionary;
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    /// <inheritdoc/>
    protected TDictionary SourceDictionary { get; }

    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    #region IDictionaryT

    /// <inheritdoc/>
    public int Count => SourceDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceDictionary.IsReadOnly;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => SourceDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => SourceDictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyObservableKeyCollection<TKey, TValue>? _observableKeys;

    /// <inheritdoc/>
    public IObservableCollection<TKey> ObservableKeys => _observableKeys ??= new(SourceDictionary);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyObservableValueCollection<TKey, TValue>? _observableValues;

    /// <inheritdoc/>
    public IObservableCollection<TValue> ObservableValues =>
        _observableValues ??= new(SourceDictionary);

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => SourceDictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => SourceDictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _removeIndex = -1;

    #region Change

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set
        {
            if (SourceDictionary.TryGetValue(key, out var oldValue) is false)
            {
                var pair = KeyValuePair.Create(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                OnDictionaryAdding(pair);
                SourceDictionary[key] = value;
                OnDictionaryAdded(pair);
            }
            else
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                    return;
                var newPair = KeyValuePair.Create(key, value);
                var oldPair = KeyValuePair.Create(key, oldValue);
                OnDictionaryReplacing(newPair, oldPair);
                SourceDictionary[key] = value;
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
        if (SourceDictionary.ContainsKey(key))
            throw new ArgumentException(
                "An element with the same key already exists in the Dictionary."
            );

        var pair = KeyValuePair.Create(key, value);
        OnDictionaryAdding(pair);
        SourceDictionary.Add(key, value);
        OnDictionaryAdded(pair);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool TryAdd(TKey key, TValue value)
    {
        if (SourceDictionary.ContainsKey(key))
            return false;

        var pair = KeyValuePair.Create(key, value);
        OnDictionaryAdding(pair);
        SourceDictionary.Add(key, value);
        OnDictionaryAdded(pair);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (SourceDictionary.TryGetPair(key, out var pair) is false)
            return false;
        OnDictionaryRemoving(pair);
        SourceDictionary.Remove(key);
        OnDictionaryRemoved(pair);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (
            SourceDictionary.TryGetValue(item.Key, out var value)
            && EqualityComparer<TValue>.Default.Equals(item.Value, value)
        )
            return Remove(item.Key);
        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnDictionaryClearing();
        SourceDictionary.Clear();
        OnDictionaryCleared();
    }

    #endregion

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return SourceDictionary.TryGetValue(item.Key, out var value)
            && EqualityComparer<TValue>.Default.Equals(item.Value, value);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return SourceDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)SourceDictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return SourceDictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return SourceDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceDictionary).GetEnumerator();
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
            _removeIndex = SourceDictionary.Keys.IndexOf(pair.Key);
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
            OnDictionaryChanging(NotifyDictionaryChangeEventArgs<TKey, TValue>.Cache_Clear);
    }

    /// <summary>
    /// 字典改变前
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanging(NotifyDictionaryChangeEventArgs<TKey, TValue> args)
    {
        DictionaryChangeEventArgs = args;
        DictionaryChanging?.Invoke(this, args);
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
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, pair));
            _observableKeys?.InvokeEvent(new(NotifyCollectionChangedAction.Add, pair.Key));
            _observableValues?.InvokeEvent(new(NotifyCollectionChangedAction.Add, pair.Value));
        }
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
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, pair, _removeIndex));
            _observableKeys?.InvokeEvent(
                new(NotifyCollectionChangedAction.Remove, pair.Key, _removeIndex)
            );
            _observableValues?.InvokeEvent(
                new(NotifyCollectionChangedAction.Remove, pair.Value, _removeIndex)
            );
        }
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
            var index = SourceDictionary.IndexOf((p => p.Key.Equals(oldPair.Key)));
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newPair, oldPair, index)
            );
            // Replaced 不会改变 Key
            //_observableKeys?.InvokeEvent(new(NotifyCollectionChangedAction.Replace, newPair, oldPair, index));
            _observableValues?.InvokeEvent(
                new(NotifyCollectionChangedAction.Replace, newPair.Value, oldPair.Value, index)
            );
        }
        PropertyChanged?.InvokeIndexer(this);
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    protected virtual void OnDictionaryCleared()
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(DictionaryChangeEventArgs ?? new(DictionaryChangeAction.Clear));
        if (CollectionChanged is not null)
        {
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
            _observableKeys?.InvokeEvent(NotifyCollectionChangedEventArgs.Cache_Reset);
            _observableValues?.InvokeEvent(NotifyCollectionChangedEventArgs.Cache_Reset);
        }

        OnCountChanged();
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
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
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
