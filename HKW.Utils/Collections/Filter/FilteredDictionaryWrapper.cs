using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Natives;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤字典
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredDictionary"/></para>
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
/// <typeparam name="TDictionary">字典类型</typeparam>
/// <typeparam name="TFilteredDictionary">已过滤字典类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredDictionaryWrapper<TKey, TValue, TDictionary, TFilteredDictionary>
#pragma warning restore S2436
    : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary,
        IFilteredCollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary, TFilteredDictionary>,
        IDictionaryWrapper<TKey, TValue, TDictionary>,
        IFilterStorage
    where TKey : notnull
    where TDictionary : IDictionary<TKey, TValue>
    where TFilteredDictionary : IDictionary<TKey, TValue>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="dictionary">字典</param>
    /// <param name="filteredDictionary">过滤字典</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredDictionaryWrapper(
        TDictionary dictionary,
        TFilteredDictionary filteredDictionary,
        Predicate<KeyValuePair<TKey, TValue>> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(filteredDictionary);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(dictionary);
        ArgumentException.ThrowIfReadOnlyCollection(filteredDictionary);

        FilterLifecycle.Validate(dictionary, filteredDictionary);
        _filteredDictionary = filteredDictionary;
        _sourceDictionary = dictionary;
        _filter = filter;
        _autoFilter = autoFilter;
        Refresh();
    }
    #endregion

    private readonly FilterLifecycle _lifecycle = new();
    private readonly TFilteredDictionary _filteredDictionary;
    private bool _autoFilter;

    /// <inheritdoc/>
    public bool IsFilterValid => _lifecycle.IsValid;

    /// <inheritdoc/>
    public bool AutoFilter
    {
        get => _autoFilter;
        set
        {
            using var operation = _lifecycle.Enter();
            _autoFilter = value;
        }
    }
    private Predicate<KeyValuePair<TKey, TValue>> _filter;

    /// <inheritdoc/>
    public Predicate<KeyValuePair<TKey, TValue>> Filter
    {
        get => _filter;
        set
        {
            using var operation = _lifecycle.Enter();
            ArgumentNullException.ThrowIfNull(value);
            var result = Compute(_sourceDictionary, value);
            _filter = value;
            Publish(result);
        }
    }
    private TDictionary _sourceDictionary;
    object IFilterStorage.SourceStorage => _sourceDictionary;

    /// <inheritdoc/>
    public TDictionary SourceDictionary
    {
        get => _sourceDictionary;
        set
        {
            using var operation = _lifecycle.Enter();
            FilterLifecycle.Validate(value, _filteredDictionary, this);
            var result = Compute(value, _filter);
            var previous = _sourceDictionary;
            _sourceDictionary = value;
            OnSourceChanged(previous);
            Publish(result);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TDictionary ICollectionWrapper<KeyValuePair<TKey, TValue>, TDictionary>.SourceCollection =>
        SourceDictionary;

    /// <summary>
    /// 过滤完成的字典
    /// </summary>
    public TFilteredDictionary FilteredDictionary
    {
        get
        {
            _lifecycle.EnsureReadable();
            return _filteredDictionary;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredDictionary IFilteredCollectionWrapper<
        KeyValuePair<TKey, TValue>,
        TDictionary,
        TFilteredDictionary
    >.FilteredCollection => FilteredDictionary;

    /// <inheritdoc/>
    public virtual void BatchUpdate(Action<TDictionary> updateAction)
    {
        using var operation = _lifecycle.Enter();
        ArgumentNullException.ThrowIfNull(updateAction);
        var autoFilter = _autoFilter;
        _autoFilter = false;
        try
        {
            _lifecycle.Update(() => updateAction(_sourceDictionary), RefreshCore);
        }
        finally
        {
            _autoFilter = autoFilter;
        }
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        using var operation = _lifecycle.Enter(recovery: true);
        RefreshCore();
    }

    private static List<KeyValuePair<TKey, TValue>> Compute(
        TDictionary source,
        Predicate<KeyValuePair<TKey, TValue>> filter
    )
    {
        var result = new List<KeyValuePair<TKey, TValue>>(source.Count);
        foreach (var item in source)
        {
            if (filter(item))
                result.Add(item);
        }
        return result;
    }

    private void RefreshCore()
    {
        List<KeyValuePair<TKey, TValue>> result;
        try
        {
            result = Compute(_sourceDictionary, _filter);
        }
        catch
        {
            _lifecycle.IsValid = false;
            throw;
        }
        Publish(result);
    }

    private void Publish(List<KeyValuePair<TKey, TValue>> result)
    {
        _lifecycle.IsValid = false;
        try
        {
            _lifecycle.EnsureSourceUnchanged();
            if (Matches(result))
            {
                _lifecycle.IsValid = true;
                return;
            }
            _filteredDictionary.Clear();
            foreach (var item in result)
                _filteredDictionary.Add(item);
            _lifecycle.EnsureSourceUnchanged();
            if (Matches(result) is false)
                throw new InvalidOperationException(
                    "The filtered backend must preserve every source key and value."
                );
            _lifecycle.IsValid = true;
        }
        catch
        {
            try
            {
                _lifecycle.IsValid = _lifecycle.SourceInvalidated is false && Matches(result);
            }
            catch
            {
                _lifecycle.IsValid = false;
            }
            throw;
        }
    }

    private bool Matches(List<KeyValuePair<TKey, TValue>> result)
    {
        if (_filteredDictionary.Count != result.Count)
            return false;
        return new HashSet<KeyValuePair<TKey, TValue>>(
            _filteredDictionary,
            FilterLifecycle.PairComparer<TKey, TValue>.Instance
        ).SetEquals(result);
    }

    private void Update(Action update, bool forceRefresh = false)
    {
        using var operation = _lifecycle.Enter();
        _lifecycle.Update(update, _autoFilter || forceRefresh ? RefreshCore : null);
    }

    /// <summary>
    /// 源切换完成后更新订阅.
    /// </summary>
    /// <param name="previous">先前的源</param>
    protected virtual void OnSourceChanged(TDictionary previous) { }

    /// <summary>
    /// 处理外部源通知. 包装器自身写入在操作结束时统一刷新.
    /// </summary>
    /// <param name="cleared">是否清空源</param>
    protected void SourceChanged(bool cleared)
    {
        if (_lifecycle.IsUpdatingSource)
            return;
        if (_lifecycle.IsActive)
        {
            _lifecycle.InvalidateSource();
            throw new InvalidOperationException(
                "The source changed during filtered view publication."
            );
        }
        if (_autoFilter || cleared)
            Refresh();
    }

    /// <summary>
    /// 释放订阅并拒绝后续写入及过滤视图读取.
    /// </summary>
    /// <param name="unsubscribe">取消订阅操作</param>
    protected void DisposeFilter(Action unsubscribe) => _lifecycle.Dispose(unsubscribe);

    #region IDictionaryT
    /// <inheritdoc/>
    public int Count => SourceDictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceDictionary.IsReadOnly;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => SourceDictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => SourceDictionary.Values;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => SourceDictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => SourceDictionary.Values;

    /// <inheritdoc/>
    public bool IsSynchronized => CollectionInterfaceBridge.IsSynchronized(SourceDictionary);

    /// <inheritdoc/>
    public object SyncRoot => CollectionInterfaceBridge.SyncRoot(SourceDictionary, this);

    /// <inheritdoc/>
    public virtual TValue this[TKey key]
    {
        get => SourceDictionary[key];
        set => Update(() => _sourceDictionary[key] = value);
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        Add(new(key, value));
    }

    /// <inheritdoc/>
    public virtual bool TryAdd(TKey key, TValue value)
    {
        var result = false;
        Update(() => result = _sourceDictionary.TryAddX(key, value));
        return result;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var result = false;
        Update(() => result = _sourceDictionary.Remove(key));
        return result;
    }

    /// <inheritdoc/>
    public virtual void Clear() => Update(_sourceDictionary.Clear, forceRefresh: true);

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return SourceDictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return SourceDictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        SourceDictionary.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    protected virtual void Add(KeyValuePair<TKey, TValue> item) =>
        Update(() => _sourceDictionary.Add(item));

    /// <inheritdoc/>
    protected virtual bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var result = false;
        Update(() => result = _sourceDictionary.Remove(item));
        return result;
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item);
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return SourceDictionary.Contains(item);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return SourceDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceDictionary).GetEnumerator();
    }
    #endregion
    #region IDictionary
    private void EnsureInterfaceWriteAllowed()
    {
        if (_lifecycle.IsActive)
            throw new InvalidOperationException(
                "Reentrant writes to the same filtered collection are not allowed."
            );
        _lifecycle.EnsureReadable();
    }

    /// <inheritdoc/>
    bool IDictionary.IsFixedSize =>
        SourceDictionary is IDictionary dictionary ? dictionary.IsFixedSize : IsReadOnly;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Keys => CollectionInterfaceBridge.AsCollection(Keys);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection IDictionary.Values => CollectionInterfaceBridge.AsCollection(Values);
    object? IDictionary.this[object key]
    {
        get =>
            CollectionInterfaceBridge.IsCompatibleKey<TKey>(key)
            && TryGetValue((TKey)key, out var value)
                ? value
                : null;
        set
        {
            EnsureInterfaceWriteAllowed();
            this[CollectionInterfaceBridge.GetKey<TKey>(key)] =
                CollectionInterfaceBridge.GetValue<TValue>(value, nameof(value));
        }
    }

    /// <inheritdoc/>
    void IDictionary.Add(object key, object? value)
    {
        EnsureInterfaceWriteAllowed();
        Add(
            CollectionInterfaceBridge.GetKey<TKey>(key),
            CollectionInterfaceBridge.GetValue<TValue>(value, nameof(value))
        );
    }

    /// <inheritdoc/>
    bool IDictionary.Contains(object key)
    {
        return CollectionInterfaceBridge.IsCompatibleKey<TKey>(key) && ContainsKey((TKey)key);
    }

    /// <inheritdoc/>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return new CollectionInterfaceBridge.DictionaryEnumerator<TKey, TValue>(GetEnumerator());
    }

    /// <inheritdoc/>
    void IDictionary.Remove(object key)
    {
        EnsureInterfaceWriteAllowed();
        if (CollectionInterfaceBridge.IsCompatibleKey<TKey>(key))
            Remove((TKey)key);
        else
        {
            using var operation = _lifecycle.Enter();
        }
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        CollectionInterfaceBridge.CopyDictionaryTo(SourceDictionary, array, index);
    }
    #endregion
}
