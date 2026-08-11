using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(IEqualityComparer<TKey>? comparer = null)
    {
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
        _dictionary = new(Comparer);
    }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
        Comparer = EqualityComparer<TKey>.Default;
        _dictionary = new(collection);
    }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
    {
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
        _dictionary = new(collection, comparer);
    }

    /// <inheritdoc/>
    private readonly OrderedDictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    #region IDictionaryT

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => _dictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyObservableKeyCollection<TKey, TValue>? _observableKeys;

    /// <inheritdoc/>
    public IObservableCollection<TKey> ObservableKeys => _observableKeys ??= new(_dictionary);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyObservableValueCollection<TKey, TValue>? _observableValues;

    /// <inheritdoc/>
    public IObservableCollection<TValue> ObservableValues => _observableValues ??= new(_dictionary);

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _removeIndex = -1;

    #region Change

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (_dictionary.TryGetValue(key, out var oldValue) is false)
            {
                var pair = new KeyValuePair<TKey, TValue>(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                OnDictionaryAdding(pair);
                _dictionary[key] = value;
                OnDictionaryAdded(pair);
            }
            else
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                    return;
                var newPair = new KeyValuePair<TKey, TValue>(key, value);
                var oldPair = new KeyValuePair<TKey, TValue>(key, oldValue);
                OnDictionaryReplacing(newPair, oldPair);
                _dictionary[key] = value;
                OnDictionaryReplaced(newPair, oldPair);
            }
        }
    }

    /// <summary>
    /// 字典改变事件参数
    /// </summary>
    private NotifyDictionaryChangeEventArgs<TKey, TValue>? _dictionaryChangeEventArgs;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
            throw new ArgumentException(
                "An element with the same key already exists in the Dictionary."
            );

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnDictionaryAdding(pair);
        _dictionary.Add(key, value);
        OnDictionaryAdded(pair);
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool TryAdd(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
            return false;

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnDictionaryAdding(pair);
        _dictionary.Add(key, value);
        OnDictionaryAdded(pair);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetPair(key, out var pair) is false)
            return false;
        OnDictionaryRemoving(pair);
        _dictionary.Remove(key);
        OnDictionaryRemoved(pair);
        return true;
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        if (((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item))
            return Remove(item.Key);
        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        OnDictionaryClearing();
        _dictionary.Clear();
        OnDictionaryCleared();
    }

    #endregion

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
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
            _removeIndex = _dictionary.IndexOf(pair.Key);
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
        _dictionaryChangeEventArgs = args;
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
            OnDictionaryChanged(
                _dictionaryChangeEventArgs ?? new(DictionaryChangeAction.Add, pair)
            );
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
                _dictionaryChangeEventArgs ?? new(DictionaryChangeAction.Remove, pair)
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
                _dictionaryChangeEventArgs ?? new(DictionaryChangeAction.Replace, newPair, oldPair)
            );
        if (CollectionChanged is not null)
        {
            var index = _dictionary.IndexOf((p => p.Key.Equals(oldPair.Key)));
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
            OnDictionaryChanged(NotifyDictionaryChangeEventArgs<TKey, TValue>.Cache_Clear);
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
        _dictionaryChangeEventArgs = null;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged
}
