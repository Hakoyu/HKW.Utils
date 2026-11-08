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
/// <remarks>
/// 推荐使用顺序字典 <see cref="OrderedDictionary{TKey, TValue}"/>
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class ObservableDictionaryWrapper<TKey, TValue, TDictionary>
#pragma warning restore S2436
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>,
        ICollectionNotificationState
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="comparer">比较器; 已知后端采用源比较器, 自定义后端须传入与源语义相同的比较器</param>
    public ObservableDictionaryWrapper(TDictionary dictionary, IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        SourceDictionary = dictionary;
        Comparer = CollectionSourceComparer.Dictionary(dictionary, comparer);
        _canSkipCanonicalization =
            GetType() == typeof(ObservableDictionary<TKey, TValue>)
            || GetType() == typeof(ObservableDictionaryWrapper<TKey, TValue, TDictionary>);
    }

    /// <inheritdoc/>
    protected TDictionary SourceDictionary { get; }

    /// <summary>
    /// 比较器
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    private readonly CollectionNotificationScope<
        ObservableDictionaryChangingEventHandler<TKey, TValue>,
        ObservableDictionaryChangedEventHandler<TKey, TValue>
    > _notifications = new();

    bool ICollectionNotificationState.IsNotifying => _notifications.IsActive;

    private protected CollectionNotificationScope<
        ObservableDictionaryChangingEventHandler<TKey, TValue>,
        ObservableDictionaryChangedEventHandler<TKey, TValue>
    >.Lease BeginOperation()
    {
        var operation = _notifications.Enter(
            DictionaryChanging,
            DictionaryChanged,
            CollectionChanged,
            PropertyChanged
        );
        _operationKeys = _observableKeys;
        _operationValues = _observableValues;
        _notifications.Include(_operationKeys);
        _notifications.Include(_operationValues);
        return operation;
    }

    private ReadOnlyObservableKeyCollection<TKey, TValue>? _operationKeys;
    private ReadOnlyObservableValueCollection<TKey, TValue>? _operationValues;

    private bool NeedsIndex =>
        _notifications.Collection is not null
        || _operationKeys?.HasCollectionObservers is true
        || _operationValues?.HasCollectionObservers is true;

    private readonly bool _canSkipCanonicalization;

    private KeyValuePair<TKey, TValue> GetStoredPair(TKey key, TValue value)
    {
        if (
            _canSkipCanonicalization
            && _notifications.Changing is null
            && _notifications.Changed is null
            && NeedsIndex is false
        )
            return new(key, value);
        if (SourceDictionary is OrderedDictionary<TKey, TValue> ordered)
            return ordered.GetAt(ordered.IndexOf(key));
        foreach (var pair in SourceDictionary)
            if (Comparer.Equals(pair.Key, key))
                return pair;
        throw new InvalidOperationException(
            "The source dictionary comparer must match the supplied comparer."
        );
    }

    private int GetKeyIndex(TKey key)
    {
        if (SourceDictionary is OrderedDictionary<TKey, TValue> ordered)
            return ordered.IndexOf(key);
        var index = 0;
        foreach (var storedKey in SourceDictionary.Keys)
        {
            if (Comparer.Equals(storedKey, key))
                return index;
            index++;
        }
        return -1;
    }

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

    #region Change

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set
        {
            using var operation = BeginOperation();
            if (SourceDictionary.TryGetValue(key, out var oldValue) is false)
            {
                var pair = new KeyValuePair<TKey, TValue>(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                var args = OnDictionaryAdding(pair);
                SourceDictionary.Add(key, value);
                OnDictionaryAdded(args, pair);
            }
            else
            {
                if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
                    return;
                var oldPair = GetStoredPair(key, oldValue);
                var newPair = new KeyValuePair<TKey, TValue>(oldPair.Key, value);
                var args = OnDictionaryReplacing(newPair, oldPair);
                SourceDictionary[key] = value;
                OnDictionaryReplaced(args, newPair, oldPair);
            }
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        using var operation = BeginOperation();
        if (SourceDictionary.ContainsKey(key))
            throw new ArgumentException(
                "An element with the same key already exists in the Dictionary."
            );

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        var args = OnDictionaryAdding(pair);
        SourceDictionary.Add(key, value);
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
        using var operation = BeginOperation();
        if (SourceDictionary.ContainsKey(key))
            return false;

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        var args = OnDictionaryAdding(pair);
        SourceDictionary.Add(key, value);
        OnDictionaryAdded(args, pair);
        return true;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        using var operation = BeginOperation();
        return RemoveCore(key);
    }

    private bool RemoveCore(TKey key)
    {
        if (SourceDictionary.TryGetValue(key, out var value) is false)
            return false;
        var pair = GetStoredPair(key, value);
        var args = OnDictionaryRemoving(pair, out var removeIndex);
        SourceDictionary.Remove(key);
        OnDictionaryRemoved(args, pair, removeIndex);
        return true;
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        using var operation = BeginOperation();
        if (
            SourceDictionary.TryGetValue(item.Key, out var value)
            && EqualityComparer<TValue>.Default.Equals(item.Value, value)
        )
            return RemoveCore(item.Key);
        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var operation = BeginOperation();
        if (Count == 0)
            return;
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
    /// <returns>事件参数</returns>
    protected virtual NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryAdding(
        KeyValuePair<TKey, TValue> pair
    )
    {
        if (_notifications.Changing is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Add, pair));
        return null;
    }

    /// <summary>
    /// 字典删除键值对前
    /// </summary>
    /// <param name="pair">键值对</param>
    /// <param name="removeIndex">删除项的索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryRemoving(
        KeyValuePair<TKey, TValue> pair,
        out int removeIndex
    )
    {
        removeIndex = NeedsIndex ? GetKeyIndex(pair.Key) : -1;
        if (_notifications.Changing is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Remove, pair));
        return null;
    }

    /// <summary>
    /// 字典改变键值对值前
    /// </summary>
    /// <param name="newPair">新键值对</param>
    /// <param name="oldPair">旧键值对</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyDictionaryChangeEventArgs<TKey, TValue>? OnDictionaryReplacing(
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (_notifications.Changing is not null)
            return OnDictionaryChanging(new(DictionaryChangeAction.Replace, newPair, oldPair));
        return null;
    }

    /// <summary>
    /// 字典清理前
    /// </summary>
    protected virtual void OnDictionaryClearing()
    {
        if (_notifications.Changing is not null)
            OnDictionaryChanging(NotifyDictionaryChangeEventArgs<TKey, TValue>.Cache_Clear);
    }

    /// <summary>
    /// 字典改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyDictionaryChangeEventArgs<TKey, TValue> OnDictionaryChanging(
        NotifyDictionaryChangeEventArgs<TKey, TValue> args
    )
    {
        _notifications.Changing?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    #endregion DictionaryChanging

    #region DictionaryChanged

    /// <summary>
    /// 字典添加键值对后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="pair">键值对</param>
    protected virtual void OnDictionaryAdded(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair
    )
    {
        if (_notifications.Changed is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Add, pair));
        var index = NeedsIndex ? GetKeyIndex(pair.Key) : -1;
        if (_notifications.Collection is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, pair, index));
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
        _operationKeys?.Notify(NotifyCollectionChangedAction.Add, pair.Key, index: index);
        _operationValues?.Notify(NotifyCollectionChangedAction.Add, pair.Value, index: index);
    }

    /// <summary>
    /// 字典删除键值对后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="pair">键值对</param>
    /// <param name="removeIndex">删除项索引</param>
    protected virtual void OnDictionaryRemoved(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> pair,
        int removeIndex
    )
    {
        if (_notifications.Changed is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Remove, pair));
        if (_notifications.Collection is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, pair, removeIndex));
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
        _operationKeys?.Notify(
            NotifyCollectionChangedAction.Remove,
            oldItem: pair.Key,
            index: removeIndex
        );
        _operationValues?.Notify(
            NotifyCollectionChangedAction.Remove,
            oldItem: pair.Value,
            index: removeIndex
        );
    }

    /// <summary>
    /// 字典键值对值改变后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="newPair">新键值对</param>
    /// <param name="oldPair">旧键值对</param>
    protected virtual void OnDictionaryReplaced(
        NotifyDictionaryChangeEventArgs<TKey, TValue>? args,
        KeyValuePair<TKey, TValue> newPair,
        KeyValuePair<TKey, TValue> oldPair
    )
    {
        if (_notifications.Changed is not null)
            OnDictionaryChanged(args ?? new(DictionaryChangeAction.Replace, newPair, oldPair));
        var index = NeedsIndex ? GetKeyIndex(oldPair.Key) : -1;
        if (_notifications.Collection is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newPair, oldPair, index)
            );
        _notifications.Property?.InvokeIndexer(this);
        // 替换不会改变键
        //_observableKeys?.InvokeEvent(new(NotifyCollectionChangedAction.Replace, newPair, oldPair, index));
        _operationValues?.Notify(
            NotifyCollectionChangedAction.Replace,
            newPair.Value,
            oldPair.Value,
            index
        );
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    protected virtual void OnDictionaryCleared()
    {
        if (_notifications.Changed is not null)
            OnDictionaryChanged(NotifyDictionaryChangeEventArgs<TKey, TValue>.Cache_Clear);
        if (_notifications.Collection is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
        _operationKeys?.Notify(NotifyCollectionChangedAction.Reset);
        _operationValues?.Notify(NotifyCollectionChangedAction.Reset);
    }

    /// <summary>
    /// 当字典改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangeEventArgs<TKey, TValue> args)
    {
        _notifications.Changed?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

    #endregion DictionaryChanged

    /// <summary>
    /// 当集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        _notifications.Collection?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// 当属性改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        _notifications.Property?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
