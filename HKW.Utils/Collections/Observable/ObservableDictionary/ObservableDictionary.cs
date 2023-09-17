using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Events;
using HKW.HKWUtils.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        IReadOnlyObservableDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    public IEqualityComparer<TKey>? Comparer => _dictionary.Comparer;

    /// <inheritdoc/>
    public bool TriggerRemoveActionOnClear { get; set; }

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
                foreach (var pair in _dictionary)
                {
                    _observableKeys.Add(pair.Key);
                    _observableValues.Add(pair.Value);
                }
            }
            else
            {
                _observableKeys.Clear();
                _observableValues.Clear();
            }
            _observableKeysAndValues = value;
        }
    }

    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<TKey, TValue> _dictionary;

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
            _dictionary = new(collection, comparer);
        else if (collection is not null)
            _dictionary = new(collection);
        else if (comparer is not null)
            _dictionary = new(comparer);
        else
            _dictionary = new();
        ObservableKeys = new ReadOnlyObservableList<TKey>(_observableKeys);
        ObservableValues = new ReadOnlyObservableList<TValue>(_observableValues);
    }

    #endregion Ctor

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ObservableList<TKey> _observableKeys = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ObservableList<TValue> _observableValues = new();

    /// <inheritdoc/>
    public IObservableList<TKey> ObservableKeys { get; }

    /// <inheritdoc/>
    public IObservableList<TValue> ObservableValues { get; }

    #region IDictionaryT

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => _dictionary.Values;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
                var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(pair);
                // 字典允许不存在的 key 作为键,会创建新的键值对
                if (OnDictionaryAdding(list))
                    return;
                _dictionary[key] = value;
                OnDictionaryAdded(list);
            }
            else
            {
                var newPair = new KeyValuePair<TKey, TValue>(key, value);
                var oldPair = new KeyValuePair<TKey, TValue>(key, oldValue);
                var newList = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(newPair);
                var oldList = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(oldPair);
                if (oldValue?.Equals(value) is true || OnDictionaryValueChanging(newList, oldList))
                    return;
                _dictionary[key] = value;
                OnDictionaryValueChanged(newList, oldList);
            }
        }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        Add(pair);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(item);
        if (OnDictionaryAdding(list))
            return;
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        OnDictionaryAdded(list);
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd(TKey, TValue)" />
    public bool TryAdd(TKey key, TValue value)
    {
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(pair);
        if (OnDictionaryAdding(list))
            return false;
        var result = _dictionary.TryAdd(pair.Key, pair.Value);
        if (result)
        {
            OnDictionaryAdded(list);
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetPair(key, out var pair) is false)
            return false;
        var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(
            (KeyValuePair<TKey, TValue>)pair
        );
        if (OnDictionaryRemoving(list))
            return false;
        var result = _dictionary.Remove(key);
        if (result)
        {
            OnDictionaryRemoved(list);
        }
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        var list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(pairs);
        if (OnDictionaryAdding(list))
            return;
        foreach (var pair in list)
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(pair);
        OnDictionaryAdded(list);
    }

    /// <inheritdoc/>
    public IList<KeyValuePair<TKey, TValue>> TryAddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs
    )
    {
        var list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(pairs);
        var addList = new List<KeyValuePair<TKey, TValue>>();
        var resultList = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(addList);
        if (OnDictionaryAdding(list))
            return resultList;
        foreach (var pair in list)
        {
            if (_dictionary.TryAdd(pair.Key, pair.Value))
                addList.Add(pair);
        }
        if (addList.HasValue())
        {
            OnDictionaryAdded(list);
        }
        return resultList;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        if (TriggerRemoveActionOnClear)
        {
            var list = new SimpleReadOnlyList<KeyValuePair<TKey, TValue>>(_dictionary);
            var removeList = new List<KeyValuePair<TKey, TValue>>();
            if (OnDictionaryRemoving(list))
                return;
            foreach (var pair in list)
            {
                if (_dictionary.Remove(pair.Key))
                    removeList.Add(pair);
            }
            if (removeList.HasValue())
            {
                OnDictionaryRemoved(list);
            }
        }
        else
        {
            if (OnDictionaryClearing())
                return;
            _dictionary.Clear();
            OnDictionaryCleared();
        }
    }

    #endregion Change

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
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
    /// <param name="pairs">键值对</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryAdding(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanging is null)
            return false;
        return OnDictionaryChanging(new(DictionaryChangeAction.Add, pairs));
    }

    /// <summary>
    /// 字典删除键值对前
    /// </summary>
    /// <param name="pairs">键值对</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryRemoving(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanging is null)
            return false;
        return OnDictionaryChanging(new(DictionaryChangeAction.Remove, pairs));
    }

    /// <summary>
    /// 字典改变键值对值前
    /// </summary>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryValueChanging(
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (DictionaryChanging is null)
            return false;
        return OnDictionaryChanging(new(DictionaryChangeAction.ValueChange, newPairs, oldPairs));
    }

    /// <summary>
    /// 字典清理前
    /// </summary>
    /// <returns>取消为 <see langword="true"/> 不取消为 <see langword="false"/></returns>
    private bool OnDictionaryClearing()
    {
        if (DictionaryChanging is null)
            return false;
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
    /// 字典添加键值对后
    /// </summary>
    /// <param name="pairs">键值对</param>
    private void OnDictionaryAdded(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Add, pairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)pairs));
        if (ObservableKeysAndValues)
        {
            _observableKeys.AddRange(pairs.Select(p => p.Key));
            _observableValues.AddRange(pairs.Select(p => p.Value));
        }
    }

    /// <summary>
    /// 字典删除键值对后
    /// </summary>
    /// <param name="pairs">键值对</param>
    private void OnDictionaryRemoved(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Remove, pairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (IList)pairs));
        if (ObservableKeysAndValues)
        {
            foreach (var pair in pairs)
            {
                _observableKeys.Remove(pair.Key);
                _observableValues.Remove(pair.Value);
            }
        }
    }

    /// <summary>
    /// 字典键值对值改变后
    /// </summary>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    private void OnDictionaryValueChanged(
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.ValueChange, newPairs, oldPairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Replace,
                    newItems: (IList)newPairs,
                    oldItems: (IList)oldPairs
                )
            );
        if (ObservableKeysAndValues)
        {
            foreach (var pair in newPairs)
            {
                _observableValues[_observableKeys.IndexOf(pair.Key)] = pair.Value;
            }
        }
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    private void OnDictionaryCleared()
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        if (ObservableKeysAndValues)
        {
            _observableKeys.Clear();
            _observableValues.Clear();
        }
    }

    /// <summary>
    /// 字典改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangingEventArgs<TKey, TValue> args)
    {
        DictionaryChanged?.Invoke(args);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public event XEventHandler<NotifyDictionaryChangingEventArgs<TKey, TValue>>? DictionaryChanged;

    #endregion DictionaryChanged

    #region CollectionChanged

    /// <summary>
    /// 集合改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(null, args);
        OnCountPropertyChanged();
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion CollectionChanged

    #region PropertyChanged

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _lastCount = 0;

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountPropertyChanged()
    {
        if (_lastCount != Count)
            OnPropertyChanged(nameof(Count));
        _lastCount = Count;
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
