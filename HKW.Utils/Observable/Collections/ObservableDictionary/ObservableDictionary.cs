using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Observable;

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
    /// <summary>
    /// 原始字典
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<TKey, TValue> _dictionary;

    #region Ctor
    /// <inheritdoc/>
    public ObservableDictionary()
        : this(null!, null) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : this(collection, null) { }

    /// <inheritdoc/>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(IEqualityComparer<TKey> comparer)
        : this(null!, comparer) { }

    /// <inheritdoc/>
    /// <param name="collection">键值对集合</param>
    /// <param name="comparer">比较器</param>
    public ObservableDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer
    )
    {
        if (collection is not null)
            _dictionary = new(collection, comparer);
        else
            _dictionary = new(comparer);
    }
    #endregion

    #region IDisposable
    void IReadOnlyObservableCollection<KeyValuePair<TKey, TValue>>.Close() { }

    void IDisposable.Dispose() { }
    #endregion

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
                if (OnDictionaryAdding(list) is false)
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
                if (
                    oldValue?.Equals(value) is true
                    || OnDictionaryReplacing(newList, oldList) is false
                )
                    return;
                _dictionary[key] = value;
                OnDictionaryReplaced(newList, oldList);
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
        if (OnDictionaryAdding(list) is false)
            return;
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        OnDictionaryAdded(list);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetPair(key, out var pair) is false)
            return false;
        var list = new SimpleSingleItemReadOnlyList<KeyValuePair<TKey, TValue>>(pair);
        if (OnDictionaryRemoving(list) is false)
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
        if (OnDictionaryAdding(list) is false)
            return;
        foreach (var pair in list)
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(pair);
        OnDictionaryAdded(list);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        if (OnDictionaryClearing() is false)
            return;
        _dictionary.Clear();
        OnDictionaryCleared();
    }

    #endregion

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
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryAdding(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanging is null)
            return true;
        return OnDictionaryChanging(new(DictionaryChangeAction.Add, pairs));
    }

    /// <summary>
    /// 字典删除键值对前
    /// </summary>
    /// <param name="pairs">键值对</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryRemoving(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanging is null)
            return true;
        return OnDictionaryChanging(new(DictionaryChangeAction.Remove, pairs));
    }

    /// <summary>
    /// 字典改变键值对值前
    /// </summary>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryReplacing(
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (DictionaryChanging is null)
            return true;
        return OnDictionaryChanging(new(DictionaryChangeAction.Replace, newPairs, oldPairs));
    }

    /// <summary>
    /// 字典清理前
    /// </summary>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryClearing()
    {
        if (DictionaryChanging is null)
            return true;
        return OnDictionaryChanging(new(DictionaryChangeAction.Clear));
    }

    /// <summary>
    /// 字典改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>不取消为 <see langword="true"/> 取消为 <see langword="false"/></returns>
    protected virtual bool OnDictionaryChanging(
        NotifyDictionaryChangingEventArgs<TKey, TValue> args
    )
    {
        DictionaryChanging?.Invoke(this, args);
        return args.Cancel is false;
    }

    /// <inheritdoc/>
    public event ObservableDictionaryChangingEventHandler<TKey, TValue>? DictionaryChanging;

    #endregion DictionaryChanging

    #region DictionaryChanged

    /// <summary>
    /// 字典添加键值对后
    /// </summary>
    /// <param name="pairs">键值对</param>
    protected virtual void OnDictionaryAdded(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Add, pairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)pairs));
        OnCountChanged();
    }

    /// <summary>
    /// 字典删除键值对后
    /// </summary>
    /// <param name="pairs">键值对</param>
    protected virtual void OnDictionaryRemoved(IList<KeyValuePair<TKey, TValue>> pairs)
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Remove, pairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (IList)pairs));
        OnCountChanged();
    }

    /// <summary>
    /// 字典键值对值改变后
    /// </summary>
    /// <param name="newPairs">新键值对</param>
    /// <param name="oldPairs">旧键值对</param>
    protected virtual void OnDictionaryReplaced(
        IList<KeyValuePair<TKey, TValue>> newPairs,
        IList<KeyValuePair<TKey, TValue>> oldPairs
    )
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Replace, newPairs, oldPairs));
        if (CollectionChanged is not null)
            OnCollectionChanged(
                new(
                    NotifyCollectionChangedAction.Replace,
                    newItems: (IList)newPairs,
                    oldItems: (IList)oldPairs
                )
            );
    }

    /// <summary>
    /// 字典清理后
    /// </summary>
    protected virtual void OnDictionaryCleared()
    {
        if (DictionaryChanged is not null)
            OnDictionaryChanged(new(DictionaryChangeAction.Clear));
        if (CollectionChanged is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        OnCountChanged();
    }

    /// <summary>
    /// 字典改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnDictionaryChanged(NotifyDictionaryChangedEventArgs<TKey, TValue> args)
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _lastCount = 0;

    /// <summary>
    /// 数量改变后
    /// </summary>
    private void OnCountChanged()
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
        PropertyChanged?.Invoke(this, new(name));
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion PropertyChanged
}
