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
                var args = OnDictionaryAdding(pair);
                _dictionary.Add(key, value);
                OnDictionaryAdded(args, pair);
            }
            else
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                    return;
                var newPair = new KeyValuePair<TKey, TValue>(key, value);
                var oldPair = new KeyValuePair<TKey, TValue>(key, oldValue);
                var args = OnDictionaryReplacing(newPair, oldPair);
                _dictionary[key] = value;
                OnDictionaryReplaced(args, newPair, oldPair);
            }
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
            throw new ArgumentException(
                "An element with the same key already exists in the Dictionary."
            );

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        var args = OnDictionaryAdding(pair);
        _dictionary.Add(key, value);
        OnDictionaryAdded(args, pair);
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
        var args = OnDictionaryAdding(pair);
        _dictionary.Add(key, value);
        OnDictionaryAdded(args, pair);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetPair(key, out var pair) is false)
            return false;
        var args = OnDictionaryRemoving(pair, out var removeIndex);
        _dictionary.Remove(key);
        OnDictionaryRemoved(args, pair, removeIndex);
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

    private NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryAdding(
        KeyValuePair<TKey, TValue> pair
    )
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Add, pair));
        return null;
    }

    private NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryRemoving(
        KeyValuePair<TKey, TValue> pair,
        out int removeIndex
    )
    {
        removeIndex = -1;
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Remove, pair));
        if (CollectionChanged is not null)
            removeIndex = _dictionary.IndexOf(pair.Key);
        return null;
    }

    private NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryReplacing(
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (DictionaryChanging is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Replace, newPair, oldPair));
        return null;
    }

    private void OnDictionaryClearing()
    {
        if (DictionaryChanging is not null)
            OnDictionaryChanging(NotifyDictionaryChangeEventArgs<TKey, TValue>.Cache_Clear);
    }

    private NotifyDictionaryChangeEventArgs<TKey, TValue> OnDictionaryChanging(
        NotifyDictionaryChangeEventArgs<TKey, TValue> args
    )
    {
        DictionaryChanging?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    #endregion DictionaryChanging

    #region DictionaryChanged

    private void OnDictionaryAdded(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Add, pair));
        if (CollectionChanged is not null)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, pair));
            _observableKeys?.InvokeEvent(new(NotifyCollectionChangedAction.Add, pair.Key));
            _observableValues?.InvokeEvent(new(NotifyCollectionChangedAction.Add, pair.Value));
        }
        OnCountChanged();
    }

    private void OnDictionaryRemoved(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair,
        int removeIndex
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Remove, pair));
        if (CollectionChanged is not null)
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, pair, removeIndex));
            _observableKeys?.InvokeEvent(
                new(NotifyCollectionChangedAction.Remove, pair.Key, removeIndex)
            );
            _observableValues?.InvokeEvent(
                new(NotifyCollectionChangedAction.Remove, pair.Value, removeIndex)
            );
        }
        OnCountChanged();
    }

    private void OnDictionaryReplaced(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Replace, newPair, oldPair));
        if (CollectionChanged is not null)
        {
            var index = _dictionary.IndexOf(oldPair.Key);
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

    private void OnDictionaryCleared()
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

    private void OnDictionaryChanged(NotifyDictionaryChangeEventArgs<TKey, TValue> args)
    {
        DictionaryChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    #endregion DictionaryChanged

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void OnCountChanged()
    {
        PropertyChanged?.Invoke(this, PropertyChangedEventArgs.Cache_Count);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
