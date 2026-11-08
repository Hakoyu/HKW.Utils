using System.Collections;
using System.Data;
using System.Diagnostics;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Collections;

/// <summary>
/// 过滤列表
/// <para>基于 <see cref="Filter"/> 维护一个实时过滤的 <see cref="FilteredList"/></para>
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
/// <typeparam name="TFilteredList">已过滤列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
#pragma warning disable S2436
public class FilteredListWrapper<TItem, TList, TFilteredList>
#pragma warning restore S2436
    : IList<TItem>,
        IReadOnlyList<TItem>,
        IFilteredCollectionWrapper<TItem, TList, TFilteredList>,
        IList,
        IListWrapper<TItem, TList>,
        IFilterStorage
    where TList : IList<TItem>
    where TFilteredList : IList<TItem>
{
    #region Ctor
    /// <inheritdoc/>
    /// <param name="list">列表</param>
    /// <param name="filteredList">过滤列表</param>
    /// <param name="filter">过滤器</param>
    /// <param name="autoFilter">自动过滤</param>
    public FilteredListWrapper(
        TList list,
        TFilteredList filteredList,
        Predicate<TItem> filter,
        bool autoFilter = true
    )
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(filteredList);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfReadOnlyCollection(list);
        ArgumentException.ThrowIfReadOnlyCollection(filteredList);

        FilterLifecycle.Validate(list, filteredList);
        _filteredList = filteredList;
        _sourceList = list;
        _filter = filter;
        _autoFilter = autoFilter;
        Refresh();
    }
    #endregion

    private readonly FilterLifecycle _lifecycle = new();
    private readonly TFilteredList _filteredList;
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

    private Predicate<TItem> _filter;

    /// <summary>
    /// 过滤器
    /// </summary>
    public Predicate<TItem> Filter
    {
        get => _filter;
        set
        {
            using var operation = _lifecycle.Enter();
            ArgumentNullException.ThrowIfNull(value);
            var result = Compute(_sourceList, value);
            _filter = value;
            Publish(result);
        }
    }

    private TList _sourceList;
    object IFilterStorage.SourceStorage => _sourceList;

    /// <inheritdoc/>
    public TList SourceList
    {
        get => _sourceList;
        set
        {
            using var operation = _lifecycle.Enter();
            FilterLifecycle.Validate(value, _filteredList, this);
            var result = Compute(value, _filter);
            var previous = _sourceList;
            _sourceList = value;
            OnSourceChanged(previous);
            Publish(result);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TList ICollectionWrapper<TItem, TList>.SourceCollection => SourceList;

    /// <summary>
    /// 过滤完成的列表
    /// </summary>
    public TFilteredList FilteredList
    {
        get
        {
            _lifecycle.EnsureReadable();
            return _filteredList;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TFilteredList IFilteredCollectionWrapper<TItem, TList, TFilteredList>.FilteredCollection =>
        FilteredList;

    /// <inheritdoc/>
    public virtual void BatchUpdate(Action<TList> updateAction)
    {
        using var operation = _lifecycle.Enter();
        ArgumentNullException.ThrowIfNull(updateAction);
        var autoFilter = _autoFilter;
        _autoFilter = false;
        try
        {
            _lifecycle.Update(() => updateAction(_sourceList), RefreshCore);
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

    private static List<TItem> Compute(TList source, Predicate<TItem> filter)
    {
        var result = new List<TItem>(source.Count);
        foreach (var item in source)
        {
            if (filter(item))
                result.Add(item);
        }
        return result;
    }

    private void RefreshCore()
    {
        List<TItem> result;
        try
        {
            result = Compute(_sourceList, _filter);
        }
        catch
        {
            _lifecycle.IsValid = false;
            throw;
        }
        Publish(result);
    }

    private void Publish(List<TItem> result)
    {
        _lifecycle.IsValid = false;
        try
        {
            _lifecycle.EnsureSourceUnchanged();
            // 按位置更新, 保留重复值的对应关系并避免无变化时清空重建.
            var sharedCount = Math.Min(_filteredList.Count, result.Count);
            for (var index = 0; index < sharedCount; index++)
            {
                if (FilterLifecycle.SameItem(_filteredList[index], result[index]) is false)
                    _filteredList[index] = result[index];
            }
            for (var index = _filteredList.Count - 1; index >= result.Count; index--)
                _filteredList.RemoveAt(index);
            for (var index = sharedCount; index < result.Count; index++)
                _filteredList.Add(result[index]);
            _lifecycle.EnsureSourceUnchanged();
            if (FilterLifecycle.Matches(_filteredList, result) is false)
                throw new InvalidOperationException(
                    "The filtered backend did not accept the complete result."
                );
            _lifecycle.IsValid = true;
        }
        catch
        {
            // 后端可能在最后一项提交后的通知中抛错, 仍保留已完成的新态.
            try
            {
                _lifecycle.IsValid =
                    _lifecycle.SourceInvalidated is false
                    && FilterLifecycle.Matches(_filteredList, result);
            }
            catch
            {
                _lifecycle.IsValid = false;
            }
            throw;
        }
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
    protected virtual void OnSourceChanged(TList previous) { }

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

    #region IList
    /// <inheritdoc/>
    public virtual TItem this[int index]
    {
        get => SourceList[index];
        set => Update(() => _sourceList[index] = value);
    }

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    /// <inheritdoc/>
    public bool IsFixedSize => CollectionInterfaceBridge.IsFixedSize(SourceList);

    /// <inheritdoc/>
    public bool IsSynchronized => CollectionInterfaceBridge.IsSynchronized(SourceList);

    /// <inheritdoc/>
    public object SyncRoot => CollectionInterfaceBridge.SyncRoot(SourceList, this);

    /// <inheritdoc/>
    public virtual void Add(TItem item) => Update(() => _sourceList.Add(item));

    /// <inheritdoc/>
    public virtual void Insert(int index, TItem item) =>
        Update(() => _sourceList.Insert(index, item));

    /// <inheritdoc/>
    public virtual bool Remove(TItem item)
    {
        var result = false;
        Update(() => result = _sourceList.Remove(item));
        return result;
    }

    /// <inheritdoc/>
    public virtual void RemoveAt(int index) => Update(() => _sourceList.RemoveAt(index));

    /// <inheritdoc/>
    public virtual void Clear() => Update(_sourceList.Clear, forceRefresh: true);

    /// <inheritdoc/>
    public bool Contains(TItem item)
    {
        return SourceList.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex)
    {
        SourceList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }
    #endregion

    #region IList
    private TItem GetInterfaceValue(object? value)
    {
        if (_lifecycle.IsActive)
            throw new InvalidOperationException(
                "Reentrant writes to the same filtered collection are not allowed."
            );
        _lifecycle.EnsureReadable();
        return CollectionInterfaceBridge.GetValue<TItem>(value, nameof(value));
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = GetInterfaceValue(value);
    }

    /// <inheritdoc/>
    int IList.Add(object? value)
    {
        Add(GetInterfaceValue(value));
        return Count - 1;
    }

    /// <inheritdoc/>
    bool IList.Contains(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<TItem>(value) && Contains((TItem)value!);
    }

    /// <inheritdoc/>
    int IList.IndexOf(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<TItem>(value) ? IndexOf((TItem)value!) : -1;
    }

    /// <inheritdoc/>
    void IList.Insert(int index, object? value)
    {
        Insert(index, GetInterfaceValue(value));
    }

    /// <inheritdoc/>
    void IList.Remove(object? value)
    {
        if (CollectionInterfaceBridge.IsCompatible<TItem>(value))
            Remove((TItem)value!);
        else
        {
            using var operation = _lifecycle.Enter();
        }
    }

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index)
    {
        CollectionInterfaceBridge.CopyTo(SourceList, array, index);
    }
    #endregion
}
