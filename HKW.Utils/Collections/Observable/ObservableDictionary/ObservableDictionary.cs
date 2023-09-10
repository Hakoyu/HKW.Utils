using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Extensions;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HKW.HKWUtils.Collections;

//[Serializable]
/// <summary>
/// 可观测字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView))]
public class ObservableDictionary<TKey, TValue>
    : IObservableDictionary<TKey, TValue>,
        IReadOnlyObservableDictionary<TKey, TValue>,
        IObservableDictionary
    where TKey : notnull
{
    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<TKey, TValue> r_dictionary;

    /// <inheritdoc/>
    public IEqualityComparer<TKey>? Comparer => r_dictionary.Comparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _observableKeysAndValues;

    /// <inheritdoc/>
    public bool ObservableKeysAndValues
    {
        get => _observableKeysAndValues;
        set
        {
            if (_observableKeysAndValues == value)
                return;
            if (value is true)
            {
                foreach (var entry in r_dictionary)
                {
                    r_observableKeys.Add(entry.Key);
                    r_observableValues.Add(entry.Value);
                }
            }
            else
            {
                r_observableKeys.Clear();
                r_observableValues.Clear();
            }
            _observableKeysAndValues = value;
        }
    }

    #region Ctor

    /// <inheritdoc/>
    public ObservableDictionary()
        : this(null!, null!) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : this(collection, null!) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(IEqualityComparer<TKey> comparer)
        : this(null!, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey> comparer
    )
    {
        if (collection is not null && comparer is not null)
            r_dictionary = new(collection, comparer);
        else if (collection is not null)
            r_dictionary = new(collection);
        else if (comparer is not null)
            r_dictionary = new(comparer);
        else
            r_dictionary = new();
        ObservableKeys = new ReadOnlyObservableList<TKey>(r_observableKeys);
        ObservableValues = new ReadOnlyObservableList<TValue>(r_observableValues);
    }

    #endregion Ctor

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ObservableList<TKey> r_observableKeys = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ObservableList<TValue> r_observableValues = new();

    /// <inheritdoc/>
    public IReadOnlyObservableList<TKey> ObservableKeys { get; }

    /// <inheritdoc/>
    public IReadOnlyObservableList<TValue> ObservableValues { get; }

    #region IDictionaryT

    /// <inheritdoc/>
    public int Count => r_dictionary.Count;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => r_dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => r_dictionary.Values;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)r_dictionary).IsReadOnly;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => r_dictionary.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => r_dictionary.Values;

    #region Change

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get => r_dictionary[key];
        set
        {
            if (r_dictionary.TryGetValue(key, out var oldValue) is false)
            {
                var entry = new KeyValuePair<TKey, TValue>(key, value);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                if (OnDictionaryAdding(entry))
                    return;
                r_dictionary[key] = value;
                OnDictionaryAdded(entry);
            }
            else
            {
                var oldEntry = new KeyValuePair<TKey, TValue>(key, oldValue);
                var newEntry = new KeyValuePair<TKey, TValue>(key, value);
                if (
                    oldValue?.Equals(value) is true || OnDictionaryValueChanging(newEntry, oldEntry)
                )
                    return;
                r_dictionary[key] = value;
                OnDictionaryValueChanged(newEntry, oldEntry);
            }
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

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd(TKey, TValue)" />
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

    #endregion Change

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

    #endregion IDictionaryT

    #region IDictionary

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IDictionary.IsFixedSize => ((IDictionary)r_dictionary).IsFixedSize;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Keys => ((IDictionary)r_dictionary).Keys;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Values => ((IDictionary)r_dictionary).Values;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => ((IDictionary)r_dictionary).IsSynchronized;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => ((IDictionary)r_dictionary).SyncRoot;

    /// <inheritdoc/>
    object? IDictionary.this[object key]
    {
        get => ((IDictionary)r_dictionary)[key];
        set => ((IDictionary)r_dictionary)[key] = value;
    }

    /// <inheritdoc/>
    void IDictionary.Add(object key, object? value)
    {
        ((IDictionary)r_dictionary).Add(key, value);
    }

    /// <inheritdoc/>
    bool IDictionary.Contains(object key)
    {
        return ((IDictionary)r_dictionary).Contains(key);
    }

    /// <inheritdoc/>
    void IDictionary.Remove(object key)
    {
        ((IDictionary)r_dictionary).Remove(key);
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        ((IDictionary)r_dictionary).CopyTo(array, index);
    }

    /// <inheritdoc/>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)r_dictionary).GetEnumerator();
    }

    /// <inheritdoc/>
    event XCancelEventHandler<
        NotifyDictionaryChangingEventArgs<object, object>
    >? INotifyDictionaryChanging.DictionaryChanging
    {
        add
        {
            DictionaryChanging += (v) =>
            {
                INotifyDictionaryChangingAction(v, value);
            };
        }
        remove
        {
            DictionaryChanging -= (v) =>
            {
                INotifyDictionaryChangingAction(v, value);
            };
        }
    }

    /// <inheritdoc/>
    event XEventHandler<
        NotifyDictionaryChangedEventArgs<object, object>
    >? INotifyDictionaryChanged.DictionaryChanged
    {
        add
        {
            DictionaryChanged += (v) =>
            {
                INotifyDictionaryChangedAction(v, value);
            };
        }
        remove
        {
            DictionaryChanged -= (v) =>
            {
                INotifyDictionaryChangedAction(v, value);
            };
        }
    }

    private static void INotifyDictionaryChangingAction(
        NotifyDictionaryChangingEventArgs<TKey, TValue> args,
        XCancelEventHandler<NotifyDictionaryChangingEventArgs<object, object>>? nonGenericEvent
    )
    {
        if (nonGenericEvent is null)
            return;
        NotifyDictionaryChangingEventArgs<object, object> newArgs;
        if (args.Action is DictionaryChangeAction.Clear)
            newArgs = new(args.Action);
        else if (args.Action is DictionaryChangeAction.Add)
            newArgs = new(
                args.Action,
                entry: new(args.NewEntries![0].Key!, args.NewEntries![0].Value!)
            );
        else if (args.Action is DictionaryChangeAction.Remove)
            newArgs = new(
                args.Action,
                entry: new(args.OldEntries![0].Key!, args.OldEntries![0].Value!)
            );
        else
            newArgs = new(
                args.Action,
                new(args.NewEntries![0].Key!, args.NewEntries![0].Value!),
                new(args.OldEntries![0].Key!, args.OldEntries![0].Value!)
            );
        nonGenericEvent?.Invoke(newArgs);
        args.Cancel = newArgs.Cancel;
    }

    private static void INotifyDictionaryChangedAction(
        NotifyDictionaryChangedEventArgs<TKey, TValue> args,
        XEventHandler<NotifyDictionaryChangedEventArgs<object, object>>? nonGenericEvent
    )
    {
        if (nonGenericEvent is null)
            return;
        if (args.Action is DictionaryChangeAction.Clear)
            nonGenericEvent?.Invoke(new(args.Action));
        else if (args.Action is DictionaryChangeAction.Add)
            nonGenericEvent?.Invoke(
                new(args.Action, entry: new(args.NewEntries![0].Key!, args.NewEntries![0].Value!))
            );
        else if (args.Action is DictionaryChangeAction.Remove)
            nonGenericEvent?.Invoke(
                new(args.Action, entry: new(args.OldEntries![0].Key!, args.OldEntries![0].Value!))
            );
        else
            nonGenericEvent?.Invoke(
                new(
                    args.Action,
                    new(args.NewEntries![0].Key!, args.NewEntries![0].Value!),
                    new(args.OldEntries![0].Key!, args.OldEntries![0].Value!)
                )
            );
    }

    #endregion IDictionary

    #region DictionaryChanging

    /// <summary>
    /// 字典添加条目前
    /// </summary>
    /// <param name="entry">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryAdding(KeyValuePair<TKey, TValue> entry)
    {
        return OnDictionaryChanging(new(DictionaryChangeAction.Add, entry));
    }

    /// <summary>
    /// 字典删除条目前
    /// </summary>
    /// <param name="entry">条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryRemoving(KeyValuePair<TKey, TValue> entry)
    {
        return OnDictionaryChanging(new(DictionaryChangeAction.Remove, entry));
    }

    /// <summary>
    /// 字典改变条目值前
    /// </summary>
    /// <param name="newEntry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryValueChanging(
        KeyValuePair<TKey, TValue> newEntry,
        KeyValuePair<TKey, TValue> oldEntry
    )
    {
        return OnDictionaryChanging(new(DictionaryChangeAction.ValueChange, newEntry, oldEntry));
    }

    /// <summary>
    /// 字典清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryClearing()
    {
        return OnDictionaryChanging(new(DictionaryChangeAction.Clear));
    }

    /// <summary>
    /// 字典改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryChanging(
        NotifyDictionaryChangingEventArgs<TKey, TValue> args
    )
    {
        DictionaryChanging?.Invoke(args);
        return args.Cancel;
    }

    /// <inheritdoc/>
    public event XCancelEventHandler<
        NotifyDictionaryChangingEventArgs<TKey, TValue>
    >? DictionaryChanging;

    #endregion DictionaryChanging

    #region DictionaryChanged

    /// <summary>
    /// 字典添加条目后
    /// </summary>
    /// <param name="entry">条目</param>
    private void OnDictionaryAdded(KeyValuePair<TKey, TValue> entry)
    {
        OnDictionaryChanged(new(DictionaryChangeAction.Add, entry));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, entry));
        if (ObservableKeysAndValues)
        {
            r_observableKeys.Add(entry.Key);
            r_observableValues.Add(entry.Value);
        }
    }

    /// <summary>
    /// 字典删除条目后
    /// </summary>
    /// <param name="entry">条目</param>
    private void OnDictionaryRemoved(KeyValuePair<TKey, TValue> entry)
    {
        OnDictionaryChanged(new(DictionaryChangeAction.Remove, entry));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, entry));
        if (ObservableKeysAndValues)
        {
            r_observableKeys.Remove(entry.Key);
            r_observableValues.Remove(entry.Value);
        }
    }

    /// <summary>
    /// 字典条目值改变后
    /// </summary>
    /// <param name="newEntry">新条目</param>
    /// <param name="oldEntry">旧条目</param>
    private void OnDictionaryValueChanged(
        KeyValuePair<TKey, TValue> newEntry,
        KeyValuePair<TKey, TValue> oldEntry
    )
    {
        OnDictionaryChanged(new(DictionaryChangeAction.ValueChange, newEntry, oldEntry));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Replace, newEntry, oldEntry));
        if (ObservableKeysAndValues)
        {
            r_observableValues[r_observableKeys.IndexOf(newEntry.Key)] = newEntry.Value;
        }
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    private void OnDictionaryCleared()
    {
        OnDictionaryChanged(new(DictionaryChangeAction.Clear));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        if (ObservableKeysAndValues)
        {
            r_observableKeys.Clear();
            r_observableValues.Clear();
        }
    }

    /// <summary>
    /// 字典改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangedEventArgs<TKey, TValue> args)
    {
        DictionaryChanged?.Invoke(args);
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>>? DictionaryChanged;

    #endregion DictionaryChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(null, args);
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged

    #region PropertyChanged

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountPropertyChanged()
    {
        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// 属性改变后
    /// </summary>
    /// <param name="name">参数</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(null, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged
}
