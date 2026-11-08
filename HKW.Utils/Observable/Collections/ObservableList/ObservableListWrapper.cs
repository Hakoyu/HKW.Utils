using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.DebugViews;
using HKW.HKWUtils.Exceptions;
using HKW.HKWUtils.Extensions;

namespace HKW.HKWUtils.Observable;

/// <summary>
/// 可观测列表包装器
/// </summary>
/// <typeparam name="TItem">项类型</typeparam>
/// <typeparam name="TList">列表类型</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(IEnumerableDebugView))]
public class ObservableListWrapper<TItem, TList>
    : IObservableList<TItem>,
        IReadOnlyObservableList<TItem>,
        ICollectionNotificationState,
        IList
    where TList : IList<TItem>
{
    /// <inheritdoc/>
    public ObservableListWrapper(TList list)
    {
        ArgumentNullException.ThrowIfNull(list);
        SourceList = list;
    }

    /// <inheritdoc/>
    protected TList SourceList { get; }

    private readonly CollectionNotificationScope<
        ObservableListChangingEventHandler<TItem>,
        ObservableListChangedEventHandler<TItem>
    > _notifications = new();

    bool ICollectionNotificationState.IsNotifying => _notifications.IsActive;

    private protected CollectionNotificationScope<
        ObservableListChangingEventHandler<TItem>,
        ObservableListChangedEventHandler<TItem>
    >.Lease BeginOperation()
    {
        return _notifications.Enter(ListChanging, ListChanged, CollectionChanged, PropertyChanged);
    }

    #region IListT

    /// <inheritdoc/>
    public int Count => SourceList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => SourceList.IsReadOnly;

    #region Change

    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => SourceList[index];
        set
        {
            using var operation = BeginOperation();
            var oldValue = SourceList[index];
            if (EqualityComparer<TItem>.Default.Equals(oldValue, value) is true)
                return;
            var args = OnListReplacing(value, oldValue, index);
            SourceList[index] = value;
            OnListReplaced(args, value, oldValue, index);
        }
    }

    /// <inheritdoc/>
    public void Add(TItem item)
    {
        using var operation = BeginOperation();
        var index = SourceList.Count;
        var args = OnListAdding(item, index);
        SourceList.Add(item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        using var operation = BeginOperation();
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);
        var args = OnListAdding(item, index);
        SourceList.Insert(index, item);
        OnListAdded(args, item, index);
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        using var operation = BeginOperation();
        var index = SourceList.IndexOf(item);
        if (index >= 0)
        {
            RemoveAtCore(index);
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        using var operation = BeginOperation();
        RemoveAtCore(index);
    }

    private void RemoveAtCore(int index)
    {
        var item = SourceList[index];
        var args = OnListRemoving(item, index);
        SourceList.RemoveAt(index);
        OnListRemoved(args, item, index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var operation = BeginOperation();
        if (Count == 0)
            return;
        OnListClearing();
        SourceList.Clear();
        OnListCleared();
    }

    #endregion Change

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
    public int IndexOf(TItem item)
    {
        return SourceList.IndexOf(item);
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        return SourceList.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SourceList).GetEnumerator();
    }

    #endregion IListT

    #region IList
    private TItem GetInterfaceValue(object? value)
    {
        if (_notifications.IsActive)
            throw new InvalidOperationException(
                "Reentrant writes to the same collection are not allowed during notification."
            );
        return CollectionInterfaceBridge.GetValue<TItem>(value, nameof(value));
    }

    bool IList.IsFixedSize => CollectionInterfaceBridge.IsFixedSize(SourceList);
    bool ICollection.IsSynchronized => CollectionInterfaceBridge.IsSynchronized(SourceList);
    object ICollection.SyncRoot => CollectionInterfaceBridge.SyncRoot(SourceList, this);
    object? IList.this[int index]
    {
        get => SourceList[index];
        set => this[index] = GetInterfaceValue(value);
    }

    int IList.Add(object? value)
    {
        Add(GetInterfaceValue(value));
        return Count - 1;
    }

    void IList.Insert(int index, object? value)
    {
        Insert(index, GetInterfaceValue(value));
    }

    void IList.Remove(object? value)
    {
        if (CollectionInterfaceBridge.IsCompatible<TItem>(value))
            Remove((TItem)value!);
        else
        {
            using var operation = BeginOperation();
        }
    }

    bool IList.Contains(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<TItem>(value) && Contains((TItem)value!);
    }

    int IList.IndexOf(object? value)
    {
        return CollectionInterfaceBridge.IsCompatible<TItem>(value) ? IndexOf((TItem)value!) : -1;
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CollectionInterfaceBridge.CopyTo(SourceList, array, index);
    }

    #endregion

    #region ListChanging

    /// <summary>
    /// 列表添加项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListAdding(TItem item, int index)
    {
        if (_notifications.Changing is not null)
            return OnListChanging(new(ListChangeAction.Add, item, index));
        return null;
    }

    /// <summary>
    /// 列表删除项目前
    /// </summary>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListRemoving(TItem item, int index)
    {
        if (_notifications.Changing is not null)
            return OnListChanging(new(ListChangeAction.Remove, item, index));
        return null;
    }

    /// <summary>
    /// 列表改变项目前
    /// </summary>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem>? OnListReplacing(
        TItem newItem,
        TItem oldItem,
        int index
    )
    {
        if (_notifications.Changing is not null)
            return OnListChanging(new(ListChangeAction.Replace, newItem, oldItem, index));
        return null;
    }

    /// <summary>
    /// 列表清理前
    /// </summary>
    protected virtual void OnListClearing()
    {
        if (_notifications.Changing is not null)
            OnListChanging(NotifyListChangeEventArgs<TItem>.Cache_Clear);
    }

    /// <summary>
    /// 列表改变前
    /// </summary>
    /// <param name="args">参数</param>
    /// <returns>事件参数</returns>
    protected virtual NotifyListChangeEventArgs<TItem> OnListChanging(
        NotifyListChangeEventArgs<TItem> args
    )
    {
        _notifications.Changing?.Invoke(this, args);
        return args;
    }

    /// <inheritdoc/>
    public event ObservableListChangingEventHandler<TItem>? ListChanging;

    #endregion ListChanging

    #region ListChanged

    /// <summary>
    /// 列表添加项目后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListAdded(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (_notifications.Changed is not null)
            OnListChanged(args ?? new(ListChangeAction.Add, item, index));
        if (_notifications.Collection is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
    }

    /// <summary>
    /// 列表删除项目后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="item">项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListRemoved(
        NotifyListChangeEventArgs<TItem>? args,
        TItem item,
        int index
    )
    {
        if (_notifications.Changed is not null)
            OnListChanged(args ?? new(ListChangeAction.Remove, item, index));
        if (_notifications.Collection is not null)
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
    }

    /// <summary>
    /// 列表项目改变后
    /// </summary>
    /// <param name="args">事件参数</param>
    /// <param name="newItem">新项目</param>
    /// <param name="oldItem">旧项目</param>
    /// <param name="index">索引</param>
    protected virtual void OnListReplaced(
        NotifyListChangeEventArgs<TItem>? args,
        TItem newItem,
        TItem oldItem,
        int index
    )
    {
        if (_notifications.Changed is not null)
            OnListChanged(args ?? new(ListChangeAction.Replace, newItem, oldItem, index));
        if (_notifications.Collection is not null)
            OnCollectionChanged(
                new(NotifyCollectionChangedAction.Replace, newItem, oldItem, index)
            );
        _notifications.Property?.InvokeIndexer(this);
    }

    /// <summary>
    /// 列表清理后
    /// </summary>
    protected virtual void OnListCleared()
    {
        if (_notifications.Changed is not null)
            OnListChanged(NotifyListChangeEventArgs<TItem>.Cache_Clear);
        if (_notifications.Collection is not null)
            OnCollectionChanged(NotifyCollectionChangedEventArgs.Cache_Reset);
        OnPropertyChanged(PropertyChangedArgsCache.Count);
        _notifications.Property?.InvokeIndexer(this);
    }

    /// <summary>
    /// 当列表改变后
    /// </summary>
    /// <param name="args">参数</param>
    protected virtual void OnListChanged(NotifyListChangeEventArgs<TItem> args)
    {
        _notifications.Changed?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public event ObservableListChangedEventHandler<TItem>? ListChanged;

    #endregion ListChanged

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
